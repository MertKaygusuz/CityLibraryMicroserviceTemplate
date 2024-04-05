namespace BookReservationReportApi.AppSettings
{
    public class AppSetting
    {
        public DbConnection DbConnection { get; set; }

        public TokenOptions TokenOptions { get; set; }
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
}