using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Interface
{    public enum QueryType
    {
        None, Song, Album, Artist
    }
    public interface IQueryResult
    {      
        //抽象一个公共的方法显示名称
        String GetName();

        //抽象一个公共的方法显示type
        QueryType GetSearchResultType();
    }
}
