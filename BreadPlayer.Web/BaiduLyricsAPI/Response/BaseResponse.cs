using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreadPlayer.Web.BaiduLyricsAPI.Response
{
    public class BaseResponse
    {
        public const int ERROR_CODE_OK = 22000;
        public const int ERROR_CODE_ERROR = 22001;
        public int error_code;
        public int errorCode;


        public int ErrorCode
        {
            get
            {
                return error_code != 0 ? error_code : errorCode;
            }
        }

        public bool IsValid
        {
            get
            {
                return error_code == 0 || error_code == ERROR_CODE_OK;
            }
        }
    }
}
