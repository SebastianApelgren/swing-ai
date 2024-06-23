using ImageTrackingApi.Tracking.Models;

namespace ImageTrackingApi.Tracking
{
    public interface IPoseEstimator
    {
        public Task InitializeAsync();

        public async Task<TrackingResult> TrackAsync(IFormFile file, int index)
        {
            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                byte[] bytes = new byte[file.Length];
                reader.BaseStream.Read(bytes, 0, (int)file.Length);
                return await TrackAsync(bytes, index);
            }
        }

        public Task<TrackingResult> TrackAsync(byte[] encodedImage, int index = 0);
    }
}
