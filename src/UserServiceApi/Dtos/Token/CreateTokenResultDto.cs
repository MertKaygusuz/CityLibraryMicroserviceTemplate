namespace UserServiceApi.Dtos.Token
{
    public class CreateTokenResultDto
    {
        public string AccessToken { get; set; }

        public DateTime AccessTokenExpiration { get; set; }

        public string RefreshTokenKey { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }

        public string ClientIp { get; set; }

        public string ClientAgent { get; set; }

        public string UserName { get; set; }
    }
}
