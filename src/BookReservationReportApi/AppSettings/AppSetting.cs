namespace BookReservationReportApi.AppSettings
{
    public class AppSetting
    {
        public DbConnection DbConnection { get; set; }

        public TokenOptions TokenOptions { get; set; }

        public RabbitMq RabbitMqOptions { get; set; }
    }

    public class DbConnection
    {
        public string ConnectionString { get; set; }

        public string DbName { get; set; }
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
        public string BookUpdatedConsumerUri { get; set; }
    }
}