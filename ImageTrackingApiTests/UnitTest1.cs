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
            await Pose25Estimator.Instance.InitializeAsync();
        }

        [TestMethod]
        public async Task Track()
        {
            await Task.CompletedTask;

            byte[] bytes = File.ReadAllBytes("C:\\users\\adam\\desktop\\test.jpg");

            TrackingResult result = Pose25Estimator.Instance.TrackAsync(bytes, 0);
        }
    }
}