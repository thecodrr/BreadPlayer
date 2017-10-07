using BreadPlayer.Helpers;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.SentryAPI
{
    public class BPSentryUserFactory : SentryUserFactory
    {
        protected override SentryUser OnCreate(SentryUser user)
        {
            return new SentryUser(UserInfoHelper.GetUsernameAsync().Result);
        }
    }
}
