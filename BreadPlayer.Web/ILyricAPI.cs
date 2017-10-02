using BreadPlayer.Core.Models;
using System.Threading.Tasks;

namespace BreadPlayer.Web
{
    public interface ILyricAPI
    {
        Task<string> FetchLyrics(Mediafile mediaFile);
    }
}