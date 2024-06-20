using ImageTrackingApi.Tracking.Models;
using ImageTrackingApi.Tracking.Pose25;
using ImageTrackingApi.Tracking.Visualization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ImageTrackingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrackingController : ControllerBase
    {
        [HttpPost("get-results-for-image")]
        [ProducesResponseType(typeof(List<TrackingResult>), (int)HttpStatusCode.OK)]
        public async Task<ObjectResult> Track(IFormFile file, int index = 0)
        {
            if (!TrackingHelper.Instance.HasModelLoaded)
                await TrackingHelper.Instance.LoadModelAsync();

            TrackingResult result = TrackingHelper.Instance.Track(file, index);

            return Ok(result);
        }

        [HttpPost("get-image")]
        [ProducesResponseType(typeof(List<TrackingResult>), (int)HttpStatusCode.OK)]
        public async Task<FileContentResult> TrackAndDisplay(IFormFile file, int index = 0)
        {
            if (!TrackingHelper.Instance.HasModelLoaded)
                await TrackingHelper.Instance.LoadModelAsync();

            TrackingResult result = TrackingHelper.Instance.Track(file, index);
            byte[] displayResult = await TrackingVisualizer.DrawResult(result, file);

            return new FileContentResult(displayResult, "image/jpeg");
        }
    }
}
