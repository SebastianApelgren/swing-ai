using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using ImageTrackingApi.Helpers;
using System.Runtime.InteropServices;

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
            using (MemoryStream prototxt = await EmbeddedResourceHelper.GetTestFileAsync("pose_deploy.prototxt"))
            {
                using (MemoryStream model = await EmbeddedResourceHelper.GetTestFileAsync("pose_iter_584000.caffemodel"))
                {
                    caffeModel = DnnInvoke.ReadNetFromCaffe(prototxt.ToArray(), model.ToArray());
                }
            }
        }

        public void Track(byte[] imageBytes, int width, int height)
        {
            GCHandle? pinnedArray = null;

            try
            {
                Image<Bgr, byte> image = new Image<Bgr, byte>(width, height); // width * 3 is because of three bytes per pixel
                image.Bytes = imageBytes;

                Track(image);
            }
            catch { throw; }
            finally
            {
                if (pinnedArray != null)
                {
                    pinnedArray.Value.Free();
                }
            }
        }

        public void Track(Image<Bgr, byte> image)
        {
            // for openopse
            int inWidth = 368;
            int inHeight = 368;
            float threshold = 0.1f;
            int nPoints = 25;

            Net net = CaffeModel;

            int imgHeight = image.Height;
            int imgWidth = image.Width;

            Mat blob = DnnInvoke.BlobFromImage(image, 1.0 / 255.0, new Size(inWidth, inHeight), new MCvScalar(0, 0, 0));

            net.SetInput(blob);
            net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);

            Mat output = net.Forward();

            int H = output.SizeOfDimension[2];
            int W = output.SizeOfDimension[3];
            Array HeatMap = output.GetData();

            List<Point> points = new List<Point>();

            for (int i = 0; i < nPoints; i++)
            {
                Matrix<float> matrix = new Matrix<float>(H, W);
                for (int row = 0; row < H; row++)
                {
                    for (int col = 0; col < W; col++)
                    {
                        matrix[row, col] = (float)HeatMap.GetValue(0, i, row, col);
                    }
                }

                double minVal = 0, maxVal = 0;
                Point minLoc = default, maxLoc = default;

                CvInvoke.MinMaxLoc(matrix, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                int x = image.Width * maxLoc.X / W;
                int y = image.Height * maxLoc.Y / H;

                if (maxVal > threshold)
                {
                    points.Add(new Point(x, y));
                }
                else
                {
                    points.Add(Point.Empty);
                }
            }

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
        }
    }
}