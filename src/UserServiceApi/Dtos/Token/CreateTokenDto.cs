namespace UserServiceApi.Dtos.Token
{
    public class CreateTokenDto
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public IEnumerable<string> UserRoleNames { get; set; }
    }
}
