using InfluencerAlert.Services.Interfaces;
using Newtonsoft.Json;

namespace InfluencerAlert.Services.Feed
{
    public class FeedService : IFeedService
    {
        public InfluenderAlert.Model.Feed GetFeed()
        {
            string allText = System.IO.File.ReadAllText(@"C:\Development\InfluencerAlert\InfluencerAlert\Data\data.json");

            return JsonConvert.DeserializeObject<InfluenderAlert.Model.Feed>(allText);
        }
    }
}
