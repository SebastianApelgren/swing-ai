namespace ImageTrackingApi.Tracking.Models
{
    public class BodyPart
    {
        public BodyPartType Type { get; set; }
        public string BodyPartTypeName => Type.ToString();
        public float X { get; set; }
        public float Y { get; set; }
        public bool MissingPosition { get; set; }

        public BodyPart(BodyPartType type, float x, float y, bool missingPosition)
        {
            Type = type;
            X = x;
            Y = y;
            MissingPosition = missingPosition;
        }
    }
}
