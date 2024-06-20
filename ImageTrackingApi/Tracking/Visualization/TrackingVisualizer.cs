using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV;
using ImageTrackingApi.Tracking.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageTrackingApi.Tracking.Visualization
{
    public class TrackingVisualizer
    {
        public static async Task<byte[]> DrawResult(TrackingResult result, IFormFile file)
        {
            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                byte[] bytes = new byte[file.Length];
                reader.BaseStream.Read(bytes, 0, (int)file.Length);
                return await DrawResult(result, bytes);
            }
        }

        public static async Task<byte[]> DrawResult(TrackingResult result, byte[] jpgImageBytes)
        {
            await Task.CompletedTask;

            using (Mat mat = new Mat())
            {
                CvInvoke.Imdecode(jpgImageBytes, ImreadModes.Color, mat);

                Image<Bgr, byte> image = mat.ToImage<Bgr, byte>();
                DrawSkeleton(result, image);

                using (MemoryStream stream = new MemoryStream())
                {
                    image.ToBitmap().Save(stream, ImageFormat.Jpeg);
                    return stream.ToArray();
                }
            }
        }

        private static void DrawSkeleton(TrackingResult result, Image<Bgr, byte> image)
        {
            List<Point> points = new List<Point>();

            // display points on image
            foreach (BodyPart bodyPart in result.BodyParts)
            {
                Point p = new Point((int)bodyPart.X, (int)bodyPart.Y);
                points.Add(p);
                if (p != Point.Empty)
                {
                    CvInvoke.Circle(image, p, 5, new MCvScalar(0, 255, 0), -1);
                    CvInvoke.PutText(image, bodyPart.BodyPartTypeName, p, FontFace.HersheySimplex, 0.8, new MCvScalar(0, 0, 255), 1, LineType.AntiAlias);
                }
            }

            // draw skeleton
            for (int i = 0; i < result.PointPairs.GetLongLength(0); i++)
            {
                int startIndex = result.PointPairs[i, 0];
                int endIndex = result.PointPairs[i, 1];

                if (points.Contains(points[startIndex]) && points.Contains(points[endIndex]))
                {
                    CvInvoke.Line(image, points[startIndex], points[endIndex], new MCvScalar(255, 0, 0), 2);
                }
            }
        }
    }
}
