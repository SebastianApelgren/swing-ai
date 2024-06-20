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

        public bool HasModelLoaded => caffeModel != null;

        public Net CaffeModel
        {
            get
            {
                if (caffeModel == null) throw new Exception("Model not loaded, call LoadModel() first!");
                return caffeModel;
            }
        }

        private Net? caffeModel;

        public async Task LoadModelAsync()
        {
            using (MemoryStream prototxt = await EmbeddedResourceHelper.GetEmbeddedResource("pose_deploy.prototxt"))
            {
                using (MemoryStream model = await EmbeddedResourceHelper.GetEmbeddedResource("pose_iter_584000.caffemodel"))
                {
                    caffeModel = DnnInvoke.ReadNetFromCaffe(prototxt.ToArray(), model.ToArray());
                }
            }
        }

        public TrackingResult Track(IFormFile file, int index)
        {
            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                byte[] bytes = new byte[file.Length];
                reader.BaseStream.Read(bytes, 0, (int)file.Length);
                return Track(bytes, index);
            }
        }

        public TrackingResult Track(byte[] jpgImageBytes, int index)
        {
            using (Mat img = new Mat())
            {
                CvInvoke.Imdecode(jpgImageBytes, ImreadModes.Color, img);

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
                BodyPart bodyPart = new BodyPart((BodyPartType)i, points[i].X, points[i].Y, points[i].IsEmpty);
                bodyParts[i] = bodyPart;
            }

            //DrawSkeleton(points, image);

            TrackingResult result = new TrackingResult((int)time, bodyParts, index, Constants.PointPairs);
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


    }
}