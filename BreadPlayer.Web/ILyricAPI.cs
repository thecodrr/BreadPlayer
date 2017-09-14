using BreadPlayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web
{
    public interface ILyricAPI
    {
        Task<string> FetchLyrics(Mediafile mediaFile);
    }
}
