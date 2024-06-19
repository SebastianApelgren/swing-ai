namespace ImageTrackingApi.Tracking.Models
{
    public class BodyPart
    {
        public BodyPartType Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public BodyPart(BodyPartType type, float x, float y)
        {
            Type = type;
            X = x;
            Y = y;
        }
    }
}
