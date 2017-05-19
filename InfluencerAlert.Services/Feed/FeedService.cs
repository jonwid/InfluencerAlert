using InfluencerAlert.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace InfluencerAlert.Services.Feed
{
    public class FeedService : IFeedService
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FeedService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public InfluenderAlert.Model.Feed GetFeed()
        {
            var allText = System.IO.File.ReadAllText(string.Format("{0}\\data\\data.json", _hostingEnvironment.ContentRootPath));

            return JsonConvert.DeserializeObject<InfluenderAlert.Model.Feed>(allText);
        }
    }
}
