using ImageTrackingApi.Tracking.Models;
using ImageTrackingApi.Tracking.Pose25;

namespace ImageTrackingApiTests
{
    [TestClass]
    public class UnitTest1
    {
        [ClassInitialize]
        public static async Task Initialize(TestContext context)
        {
            await TrackingHelper.Instance.LoadModelAsync();
        }

        [TestMethod]
        public async Task Track()
        {
            await Task.CompletedTask;

            byte[] bytes = File.ReadAllBytes("C:\\users\\adam\\desktop\\test.jpg");

            TrackingResult result = TrackingHelper.Instance.Track(bytes, 0);
        }
    }
}