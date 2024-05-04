namespace BookServiceApi.AppSettings
{
    public class AppSetting
    {
        public TokenOptions TokenOptions { get; set; }

        public string DbConnectionString { get; set; }

        public string ReservationServiceBaseUrl { get; set; }

        public string BookReservationGrpcEndPoint { get; set; }

        public RabbitMq RabbitMqOptions { get; set; }

        public string CurrentApplicationUrl { get; set; }
    }

    public class TokenOptions
    {
        public string Audience { get; set; }
        
        public string Issuer { get; set; }

        public string SecurityKey { get; set; }
    }

    public class RabbitMq
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BookUpdatedSenderUri { get; set; }
        public string UserCreatedConsumerUri { get; set; }
    }
   
}
