namespace DomainLogic
{
    public class YandexToken {
        public YandexToken(string oauthToken = "", string jwtToken = "", string refreshToken = "")
        {
            OauthToken = oauthToken;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }

        public string OauthToken { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}