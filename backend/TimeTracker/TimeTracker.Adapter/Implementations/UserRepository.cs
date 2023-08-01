using System;
using System.Net;
using AutoMapper;
using UserDB = TimeTracker.DataBase.DBModels.User;
using Microsoft.EntityFrameworkCore;
using TimeTracker.Common;
using TimeTracker.DataBase;
using TimeTracker.Interfaces;
using UserProject = TimeTracker.Models.User;
using TimeTracker.Adapter.Mapper;

namespace TimeTracker.Adapter
{
	public class UserRepository : BaseRepository, IUserRepository
    {
		public UserRepository(Context context, IMapper mapper) : base(context, mapper) { }

        public async Task SetTokenUser(string? login, string? token, string? refreshToken = null)
        {
            try
            {
                if (string.IsNullOrEmpty(login) ||
                    string.IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException();
                }

                var userDb = await _context.User.FirstOrDefaultAsync(x => x.Login == login);

                if (userDb == null)
                {
                    throw new ArgumentNullException();
                }

                userDb.Token = token;
                userDb.RefreshToken = refreshToken;

                _context.User.Update(userDb);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException exception)
            {
                throw new ArgumentNullException("Token is required.", exception);
            }
            catch (InvalidOperationException exception)
            {
                throw new InvalidOperationException("An error occurred while saving the user token.", exception);
            }
        }

        public async Task<UserProject?> GetTokenUser(string? login)
        {
            var user = new UserProject(); ;

            try
            {
                var userDb = await _context.User.FirstOrDefaultAsync(x => x.Login == login);

                if (userDb == null)
                    throw new ArgumentNullException();

                user = _mapper.Map<UserProject>(userDb);
            }
            catch (InvalidOperationException exception)
            {
                throw new InvalidOperationException("An error occurred while retrieving the user token: " + exception.Message);
            }
            catch (ArgumentNullException exception)
            {
                throw new ArgumentNullException("An error occurred while retrieving the user token: " + exception.Message);
            }
            catch (OperationCanceledException exception)
            {
                throw new OperationCanceledException("An error occurred while retrieving the user token: " + exception.Message);
            }

            return user;
        }

        public async Task RegisterUserInDB(string? login, string? password = "authorization-via-gitlab")
        {
            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                    throw new ArgumentException("Login and password cannot be null or empty.");

                var userExists = await _context.User.AnyAsync(x => x.Login == login);
                if (userExists)
                    throw new ArgumentException("User with the same login already exists.");

                var newUser = new UserDB()
                {
                    Login = login,
                    Password = Hasher.HashPassword(password)
                };

                _context.User.Add(newUser);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException("An error occurred while registering the user: " + exception.Message);
            }
            catch (OperationCanceledException exception)
            {
                throw new OperationCanceledException("An error occurred while registering the user:  " + exception.Message);
            }
            catch (Exception exception)
            {
                throw new Exception("An error occurred while registering the user: " + exception.Message);
            }
        }

        public async Task<bool> Authorization(string? login, string? password)
        {
            bool isSuccessfulAuthorization = false;

            try
            {
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                    throw new ArgumentException("Login and password cannot be null or empty.");

                bool userExists = await _context.User.AnyAsync(x => x.Login == login);
                if (!userExists)
                    throw new ArgumentException("User does not exist.");

                var user = await _context.User.FirstAsync(x => x.Login == login);

                if (user.Password == "authorization-via-gitlab" &&
                    !string.IsNullOrEmpty(user.RefreshToken))
                {
                    throw new OperationCanceledException();
                }

                var hashedPassword = user.Password;

                if (string.IsNullOrEmpty(hashedPassword))
                {
                    throw new ArgumentNullException();
                }

                if (Hasher.VerifyPassword(password, hashedPassword))
                    isSuccessfulAuthorization = true;
                else
                    throw new Exception("Invalid password.");
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException("Authorization failed due to invalid arguments.", exception);
            }
            catch (OperationCanceledException exception)
            {
                throw new OperationCanceledException("Authorization operation was canceled.", exception);
            }
            catch (Exception exception)
            {
                throw new Exception("Authorization failed due to an unexpected error.", exception);
            }

            return isSuccessfulAuthorization;
        }

        public async Task<bool> AuthorizationViaGitLab(string? login)
        {
            bool isSuccessfulAuthorization = false;

            try
            {
                if (string.IsNullOrEmpty(login))
                    throw new ArgumentException("Login cannot be null or empty.");

                bool userExists = await _context.User.AnyAsync(x => x.Login == login);

                if (userExists)
                {
                    isSuccessfulAuthorization = true;
                }
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException("Authorization failed due to invalid arguments.", exception);
            }
            catch (OperationCanceledException exception)
            {
                throw new OperationCanceledException("Authorization operation was canceled.", exception);
            }
            catch (Exception exception)
            {
                throw new Exception("Authorization failed due to an unexpected error.", exception);
            }

            return isSuccessfulAuthorization;
        }
    }
}