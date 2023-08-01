using System;

namespace TimeTracker.DataBase.DBModels
{
    public class User
    {
        public int Id { get; set; }

        public string? Login { get; set; }

        public string? Password { get; set; }

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }
    }
}

