namespace UserServiceApi.AppSettings
{
    public class AppSetting
    {
        public TokenOptions TokenOptions { get; set; }

        public string DbConnectionString { get; set; }

        public string ReservationServiceBaseUrl { get; set; }

        public string BookReservationGrpcEndPoint { get; set; }
    }

    public class TokenOptions
    {
        public string Audience { get; set; }
        
        public string Issuer { get; set; }

        public string SecurityKey { get; set; }
    }

   
}
