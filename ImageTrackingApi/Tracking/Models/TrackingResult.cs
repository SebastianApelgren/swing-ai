namespace ImageTrackingApi.Tracking.Models
{
    public class TrackingResult
    {
        public int TimeConsumed { get; set; }
        public BodyPart[] BodyParts { get; set; }
        public int Index { get; set; }

        public TrackingResult(int timeConsumed, BodyPart[] bodyParts, int index)
        {
            TimeConsumed = timeConsumed;
            BodyParts = bodyParts;
            Index = index;
        }
    }
}
