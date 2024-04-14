using CityLibrary.Shared.DbBase.SQL;

namespace BookServiceApi.Entities
{
    public class User : TableBase
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
    }
}