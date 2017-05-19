using System;
using System.Collections.Generic;
using System.Text;
using InfluencerAlert.Services.Interfaces.Instagram;
using InstaSharp;

namespace InfluencerAlert.Services.Instagram
{
    public class HashTagService : IHashTagService
    {
        public string GetAccessLink(InstagramConfig config)
        {

            var scopes = new List<OAuth.Scope>();
            scopes.Add(OAuth.Scope.Basic);
            scopes.Add(OAuth.Scope.Public_Content);

            // scopes.Add(InstaSharp.OAuth.Scope.Comments);

            var link = OAuth.AuthLink(config.OAuthUri + "authorize", config.ClientId, config.RedirectUri, scopes, OAuth.ResponseType.Code);

            return link;

        }
    }
}
