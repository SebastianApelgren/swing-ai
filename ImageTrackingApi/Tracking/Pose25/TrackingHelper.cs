using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using ImageTrackingApi.Helpers;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ImageTrackingApi.Tracking.Models;
using Emgu.CV.Mcc;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace ImageTrackingApi.Tracking.Pose25
{
    public class TrackingHelper
    {
        public static TrackingHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new TrackingHelper();

                return instance;
            }
        }

        private static TrackingHelper? instance;

        public Net CaffeModel
        {
            get
            {
                if (caffeModel == null) throw new Exception("Model not loaded, call LoadModel() first!");
                return caffeModel;
            }
        }

        private Net? caffeModel;

        public async Task LoadModel()
        {
            using (MemoryStream prototxt = await EmbeddedResourceHelper.GetEmbeddedResource("pose_deploy.prototxt"))
            {
                using (MemoryStream model = await EmbeddedResourceHelper.GetEmbeddedResource("pose_iter_584000.caffemodel"))
                {
                    caffeModel = DnnInvoke.ReadNetFromCaffe(prototxt.ToArray(), model.ToArray());
                }
            }
        }

        public TrackingResult Track(byte[] imageBytes, int index)
        {
            using (Mat img = new Mat())
            {
                CvInvoke.Imdecode(imageBytes, ImreadModes.Color, img);

                return Track(img.ToImage<Bgr, byte>(), index);
            }
        }

        private TrackingResult Track(Image<Bgr, byte> image, int index)
        {
            // for openopse
            int inWidth = 368;
            int inHeight = 368;

            Net net = CaffeModel;

            Mat blob = DnnInvoke.BlobFromImage(image, 1.0 / 255.0, new Size(inWidth, inHeight), new MCvScalar(0, 0, 0));

            net.SetInput(blob);
            net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);

            Stopwatch stopwatch = Stopwatch.StartNew();

            Mat output = net.Forward();

            stopwatch.Stop();
            long time = stopwatch.ElapsedMilliseconds;

            List<Point> points = GetPointListFromOutput(output, image.Width, image.Height, bodyPartCount: 25, heatmapThreshold: 0.1f);

            BodyPart[] bodyParts = new BodyPart[25];

            for (int i = 0; i < points.Count; i++)
            {
                BodyPart bodyPart = new BodyPart((BodyPartType)i, points[i].X, points[i].Y);
                bodyParts[i] = bodyPart;
            }

            DrawSkeleton(points, image);

            TrackingResult result = new TrackingResult((int)time, bodyParts, index);
            return result;
        }

        private List<Point> GetPointListFromOutput(Mat output, int imageWidth, int imageHeight, int bodyPartCount, float heatmapThreshold = 0.1f)
        {
            int height = output.SizeOfDimension[2];
            int width = output.SizeOfDimension[3];
            Array heatMap = output.GetData();

            List<Point> points = new List<Point>();

            for (int i = 0; i < bodyPartCount; i++)
            {
                Matrix<float> matrix = new Matrix<float>(height, width);
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        object? value = heatMap.GetValue(0, i, row, col);

                        if (value == null) throw new Exception("Value from heatmap is null");

                        matrix[row, col] = (float)value;
                    }
                }

                double minVal = 0, maxVal = 0;
                Point minLoc = default, maxLoc = default;

                CvInvoke.MinMaxLoc(matrix, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                int x = imageWidth * maxLoc.X / width;
                int y = imageHeight * maxLoc.Y / height;

                if (maxVal > heatmapThreshold)
                {
                    points.Add(new Point(x, y));
                }
                else
                {
                    points.Add(Point.Empty);
                }
            }

            return points;
        }

        private void DrawSkeleton(List<Point> points, Image<Bgr, byte> image)
        {
            // display points on image
            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                if (p != Point.Empty)
                {
                    CvInvoke.Circle(image, p, 5, new MCvScalar(0, 255, 0), -1);
                    CvInvoke.PutText(image, i.ToString(), p, FontFace.HersheySimplex, 0.8, new MCvScalar(0, 0, 255), 1, LineType.AntiAlias);
                }
            }

            // draw skeleton
            for (int i = 0; i < Constants.PointPairs.GetLongLength(0); i++)
            {
                int startIndex = Constants.PointPairs[i, 0];
                int endIndex = Constants.PointPairs[i, 1];

                if (points.Contains(points[startIndex]) && points.Contains(points[endIndex]))
                {
                    CvInvoke.Line(image, points[startIndex], points[endIndex], new MCvScalar(255, 0, 0), 2);
                }
            }

            image.Save("c:\\users\\adam\\desktop\\output.jpg");
        }
    }
}