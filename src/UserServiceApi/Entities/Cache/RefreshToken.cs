namespace UserServiceApi.Entities.Cache
{
    public class RefreshToken
    {
        public string TokenKey { get; set; }
        
        public DateTime DueTime { get; set; }
        
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public IEnumerable<string> UserRoleNames { get; set; }

        public string ClientIp { get; set; }

        public string ClientAgent { get; set; }
    }
}
