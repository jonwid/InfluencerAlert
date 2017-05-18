using System;
using System.Collections.Generic;
using System.Text;
using InstaSharp;

namespace InfluencerAlert.Services.Interfaces.Instagram
{
    public interface IHashTagService
    {
        string GetAccessLink(InstagramConfig config);
    }
}
