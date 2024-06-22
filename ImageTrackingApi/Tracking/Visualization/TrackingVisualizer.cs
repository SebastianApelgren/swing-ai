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
                    return CvInvoke.Imencode(".jpg", image);
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
                }
            }

            Dictionary<BodyPartType, BodyPart> bodyPartDict = result.BodyParts.ToDictionary(x => x.Type);
            List<Tuple<BodyPartType, BodyPartType>> pairs =
            [
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.Nose, BodyPartType.Neck),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.Neck, BodyPartType.RightShoulder),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.RightShoulder, BodyPartType.RightElbow),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.RightElbow, BodyPartType.RightWrist),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.Neck, BodyPartType.LeftShoulder),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.LeftShoulder, BodyPartType.LeftElbow),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.LeftElbow, BodyPartType.LeftWrist),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.Neck, BodyPartType.MidHip),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.MidHip, BodyPartType.RightHip),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.RightHip, BodyPartType.RightKnee),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.RightKnee, BodyPartType.RightAnkle),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.MidHip, BodyPartType.LeftHip),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.LeftHip, BodyPartType.LeftKnee),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.LeftKnee, BodyPartType.LeftAnkle),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.RightAnkle, BodyPartType.RightHeel),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.RightAnkle, BodyPartType.RightBigToe),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.RightBigToe, BodyPartType.RightSmallToe),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.LeftAnkle, BodyPartType.LeftHeel),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.LeftAnkle, BodyPartType.LeftBigToe),
                new Tuple<BodyPartType, BodyPartType>(BodyPartType.LeftBigToe, BodyPartType.LeftSmallToe),
            ];

            // draw skeleton
            foreach (Tuple<BodyPartType, BodyPartType> pair in pairs)
            {
                if (bodyPartDict.ContainsKey(pair.Item1) && bodyPartDict.ContainsKey(pair.Item2))
                {
                    Point start = new Point((int)bodyPartDict[pair.Item1].X, (int)bodyPartDict[pair.Item1].Y);
                    Point end = new Point((int)bodyPartDict[pair.Item2].X, (int)bodyPartDict[pair.Item2].Y);

                    if (start != Point.Empty && end != Point.Empty)
                    {
                        CvInvoke.Line(image, start, end, new MCvScalar(255, 0, 0), 2);
                    }
                }
            }
        }
    }
}
