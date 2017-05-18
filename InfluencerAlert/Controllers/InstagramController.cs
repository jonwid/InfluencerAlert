using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InfluencerAlert.Configuration;
using InfluencerAlert.Services;
using InfluencerAlert.Services.Interfaces.Instagram;
using InstaSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;


namespace InfluencerAlert.Controllers
{
    public class InstagramController : Controller
    {
        private readonly IHashTagService _hashTagService;
        private readonly InstagramSettings _instagramSettings;
        private readonly InstagramConfig _config;

        public InstagramController(IOptions<InstagramSettings> settings, IHashTagService hashTagService)
        {
            _hashTagService = hashTagService;
            _instagramSettings = settings.Value;

            _config = new InstagramConfig(_instagramSettings.ClientId, _instagramSettings.ClientSecret, _instagramSettings.RedirectURI, string.Empty);

        }

        public IActionResult Login()
        {
            var accessLink = _hashTagService.GetAccessLink(_config);

            return Redirect(accessLink);
        }

        public async Task<IActionResult> LoginSuccess(string code)
        {
            // add this code to the auth object
            var auth = new OAuth(_config);
            
            // now we have to call back to instagram and include the code they gave us
            // along with our client secret
            var oauthResponse = await auth.RequestToken(code);

            // both the client secret and the token are considered sensitive data, so we won't be
            // sending them back to the browser. we'll only store them temporarily.  If a user's session times
            // out, they will have to click on the authenticate button again - sorry bout yer luck.
            HttpContext.Session.SetString("InstaSharp.AuthInfo", JsonConvert.SerializeObject(oauthResponse));


            var users = new InstaSharp.Endpoints.Tags(_config, oauthResponse);
            var recent = await users.RecentMultiplePages("mood");
            

            return View();
        }
    }
}