using System.Text.Json.Serialization;

namespace ImageTrackingApi.Tracking.Models
{
    public class TrackingResult
    {
        public int TimeConsumed { get; set; }
        public BodyPart[] BodyParts { get; set; }
        public int Index { get; set; }

        [JsonIgnore]
        public int[,] PointPairs { get; set; }

        public TrackingResult(int timeConsumed, BodyPart[] bodyParts, int index, int[,] pointPairs)
        {
            TimeConsumed = timeConsumed;
            BodyParts = bodyParts;
            Index = index;
            PointPairs = pointPairs;
        }
    }
}
