using Newtonsoft.Json;

namespace SearchApi.Models.Error
{
    public enum ApiErrorCode
    {
        // 1~99 : INFO
        NotFoundKakaoUser = 1,
        NotRegisterUser = 2,    //인증된 사용자가 아님
        RegisterDailylimit = 3,
        NotFoundUserFromAPI = 4, //API 에 존재하지 않는 사용자.

        InfoMax = 100,
        // 101~199 : Warn
        InvalidPhonNo = 101,
        NotFoundData = 102,

        WarnMax = 200,
        // 200 ~ 299: Error
        ApiTimeOut = 201,
        ApiUnexpectedError = 202,   //예측되지 않는 에러
        ApiRetractError = 203,      //신공철회 실패
        DbError = 203,              //DB 접속에러..

        ErrorMax = 300
    }

    public class ErrorDetails
    {
        public int status_code { get; set; }
        public int error_code { get; set; }
        public string message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
