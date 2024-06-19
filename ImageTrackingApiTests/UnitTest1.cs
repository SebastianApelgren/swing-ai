using ImageTrackingApi.Tracking.Pose25;

namespace ImageTrackingApiTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task Track()
        {
            byte[] bytes = File.ReadAllBytes("C:\\users\\adam\\desktop\\test.jpg");
            
            await TrackingHelper.Instance.LoadModel();
            TrackingHelper.Instance.Track(bytes, 1267, 846);

            
        }
    }
}