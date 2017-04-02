using System;

namespace BreadPlayer.Web.BaiduLyricsAPI.Interface
{    public enum QueryType
    {
        None, Song, Album, Artist
    }
    public interface IQueryResult
    {      
        //抽象一个公共的方法显示名称
        string GetName();

        //抽象一个公共的方法显示type
        QueryType GetSearchResultType();
    }
}
