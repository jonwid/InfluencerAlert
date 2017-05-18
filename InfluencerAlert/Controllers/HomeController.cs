using InfluencerAlert.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InfluencerAlert.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFeedService _feedService;

        public HomeController(IFeedService feedService)
        {
            _feedService = feedService;
        }

        public IActionResult Index()
        {
            var feed = _feedService.GetFeed();

            return View(feed);
        }
        
    }
}
