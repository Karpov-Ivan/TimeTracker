using System;
using TimeTracker.Models;
using UserProject = TimeTracker.Models.User;

namespace TimeTracker.Interfaces
{
	public interface IUserRepository
	{
        public Task SetTokenUser(string? login, string? token, string? refreshToken = null);

        public Task<UserProject?> GetTokenUser(string? login);

        public Task RegisterUserInDB(string? login, string? password = "authorization-via-gitlab");

        public Task<bool> Authorization(string? login, string? password);

        public Task<bool> AuthorizationViaGitLab(string? login);
    }
}