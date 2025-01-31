﻿namespace UserServiceApi.Dtos.User
{
    public class RegistrationDto
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }
    }
}
