using System.Reflection;

namespace ImageTrackingApi.Helpers
{
    public class EmbeddedResourceHelper
    {
        public static async Task<MemoryStream> GetTestFileAsync(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(filename));

            MemoryStream memoryStream = new MemoryStream();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
            {
                using (StreamReader reader = new StreamReader(stream!))
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream;
                }
            }
        }
    }
}
