namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class BaseResponse
    {
        public const int ErrorCodeOk = 22000;
        public const int ErrorCodeError = 22001;
        public int error_code;
        public int errorCode;


        public int ErrorCode => error_code != 0 ? error_code : errorCode;

        public bool IsValid => error_code == 0 || error_code == ErrorCodeOk;
    }
}
