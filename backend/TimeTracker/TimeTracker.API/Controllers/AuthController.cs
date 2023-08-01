using System;
using Azure.Core;
using TimeTracker.DataBase;
using GitLabServices;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Adapter;
using TimeTracker.Interfaces;
using TimeTracker.Interfaces.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using Newtonsoft.Json.Linq;
using TimeTracker.Models.Models;
using TimeTracker.Dto;
using AutoMapper;
using Newtonsoft.Json;
using static TimeTracker.Controllers.authController;
using TimeTracker.Models;
using TimeTracker.DTOModels;

namespace TimeTracker.Controllers
{
    /// <summary>
    /// Контроллер, отвечающий за авторизацию пользователей.
    /// </summary>
    [ApiController]
    [Route(template: "api/[controller]")]
    public class authController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IGitLabService _gitLabService;

        private readonly IConfiguration _configuration;

        protected readonly IMapper _mapper;

        public class RegisterRequest
        {
            [JsonProperty("login")]
            public string? Login { get; set; }

            [JsonProperty("password")]
            public string? Password { get; set; }
        }

        public authController(IUserRepository userRepository, IGitLabService gitLabService, IMapper mapper)
        {
            _userRepository = userRepository;

            _gitLabService = gitLabService;

            _mapper = mapper;
        }

        /// <summary>
        /// Регистрирует пользователя с указанным логином и паролем.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Объект IActionResult, представляющий ответ на регистрацию.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status400BadRequest">Выбрасывается, когда предоставленные аргументы недопустимы.</exception>
        /// <exception cref="Status408RequestTimeout">Выбрасывается, когда процесс регистрации отменен из-за превышения времени ожидания.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе регистрации.</exception>
        [HttpPost("sign_up")]
        public async Task<IActionResult> Register()
        {
            IActionResult response;

            try
            {
                var registerRequest = new RegisterRequest();

                using (var reader = new StreamReader(Request.Body))
                {
                    var requestBody = reader.ReadToEndAsync().Result;

                    registerRequest = JsonConvert.DeserializeObject<RegisterRequest>(requestBody);
                }

                if (registerRequest == null)
                {
                    throw new ArgumentNullException();
                }

                if (string.IsNullOrEmpty(registerRequest.Login) ||
                    string.IsNullOrEmpty(registerRequest.Password))
                {
                    throw new ArgumentNullException();
                }

                await _userRepository.RegisterUserInDB(registerRequest.Login, registerRequest.Password);

                var jwtToken = GenerateJwtToken(registerRequest.Login);

                Response.Cookies.Append("auth_token", jwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddHours(720)
                });

                response = Ok();
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (OperationCanceledException)
            {
                response = StatusCode(StatusCodes.Status408RequestTimeout);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Авторизует пользователя с указанным логином и паролем.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>Объект IActionResult, представляющий ответ на авторизацию.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status400BadRequest">Выбрасывается, когда предоставленные аргументы недопустимы.</exception>
        /// <exception cref="Status408RequestTimeout">Выбрасывается, когда процесс авторизации отменен из-за превышения времени ожидания.</exception>
        /// <exception cref="Status500InternalServerError">Выбрасывается, когда происходит внутренняя ошибка сервера в процессе авторизации.</exception>
        [HttpPost("sign_in")]
        public async Task<IActionResult> Authorization()
        {
            IActionResult response;

            try
            {
                var registerRequest = new RegisterRequest();

                using (var reader = new StreamReader(Request.Body))
                {
                    var requestBody = reader.ReadToEndAsync().Result;

                    registerRequest = JsonConvert.DeserializeObject<RegisterRequest>(requestBody);
                }

                if (registerRequest == null)
                {
                    throw new ArgumentNullException();
                }

                if (string.IsNullOrEmpty(registerRequest.Login) ||
                    string.IsNullOrEmpty(registerRequest.Password))
                {
                    throw new ArgumentNullException();
                }

                var isValid = await _userRepository.Authorization(registerRequest.Login, registerRequest.Password);

                if (isValid)
                {
                    var jwtToken = GenerateJwtToken(registerRequest.Login);

                    Response.Cookies.Append("auth_token", jwtToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddHours(720)
                    });

                    response = Ok();
                }
                else
                {
                    response = StatusCode(StatusCodes.Status401Unauthorized);
                }
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (OperationCanceledException)
            {
                response = StatusCode(StatusCodes.Status408RequestTimeout);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Выход пользователя.
        /// </summary>
        /// <returns>Результат операции выхода.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status400BadRequest">Ошибка запроса.</exception>
        [HttpPost("sign_out")]
        public IActionResult Logout()
        {
            IActionResult response;

            try
            {
                Response.Cookies.Delete("auth_token");

                response = Ok();
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }

            return response;
        }

        /// <summary>
        /// Проверка токена авторизации.
        /// </summary>
        /// <returns>Результат операции проверки токена.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status400BadRequest">Ошибка запроса.</exception>
        [HttpGet()]
        public async Task<IActionResult> Cookies()
        {
            IActionResult response;

            try
            {
                var authToken = HttpContext.Request.Cookies["auth_token"];

                var isAuthenticated = ValidateAuthToken(authToken);

                var user = new User();

                if (isAuthenticated)
                {
                    var login = GetLoginFromJwtToken(authToken);

                    user = await _userRepository.GetTokenUser(login);

                    if (!string.IsNullOrEmpty(user.RefreshToken))
                    {
                        var tokens = await _gitLabService.RefreshAccessToken(user.RefreshToken);

                        await _userRepository.SetTokenUser(user.Login, tokens.access_token, tokens.refresh_token);

                        user = await _userRepository.GetTokenUser(user.Login);
                    }
                }

                response = Ok(_mapper.Map<UserDto>(user));
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }

            return response;
        }

        private string GenerateJwtToken(string login)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login),
            };

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(720),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetLoginFromJwtToken(string token)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var loginClaim = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            return loginClaim?.Value;
        }

        private bool ValidateAuthToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var config = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json")
                                 .Build();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                return principal != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Добавляет токен пользователя в базу данных.
        /// </summary>
        /// <param name="token">Токен пользователя.</param>
        /// <returns>Результат добавления токена.</returns>
        /// <exception cref="Status200OK">Успешное выполнение операции.</exception>
        /// <exception cref="Status400BadRequest">Ошибка: некорректный или отсутствующий токен.</exception>
        /// <exception cref="Status500InternalServerError">Ошибка сервера: операция была отменена.</exception>
        [HttpPost("add_token")]
        public async Task<IActionResult> AddToken(string? login)
        {
            IActionResult response;

            try
            {
                var authToken = HttpContext.Request.Cookies["auth_token"];
                var token = HttpContext.Request.Headers["token"];

                Console.WriteLine("authToken: " + authToken);
                Console.WriteLine("token: " + token);

                if (string.IsNullOrEmpty(authToken))
                    throw new ArgumentNullException(nameof(authToken), $"Can't be null {nameof(authToken)}");
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentNullException(nameof(token), $"Can't be null {nameof(token)}");

                var isAuthenticated = ValidateAuthToken(authToken);

                if (!isAuthenticated)
                {
                    throw new SecurityTokenException();
                }

                var user = _gitLabService.GetCurrentUser(token);

                await _userRepository.SetTokenUser(login, token);

                response = Ok();
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest, "Некорректный или отсутствующий токен.");
            }
            catch (OperationCanceledException)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера: операция была отменена.");
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера.");
            }

            return response;
        }

        /// <summary>
        /// Получает токен пользователя из базы данных.
        /// </summary>
        /// <returns>Токен.</returns>
        /// <exception cref="Status200OK">Успешное выполнение операции.</exception>
        /// <exception cref="Status400BadRequest">Ошибка: некорректный или отсутствующий токен.</exception>
        /// <exception cref="Status500InternalServerError">Ошибка сервера: операция была отменена.</exception>
        [HttpGet("get_token")]
        public async Task<IActionResult> GetToken(string? login)
        {
            IActionResult response;

            try
            {
                var authToken = HttpContext.Request.Cookies["auth_token"];

                if (string.IsNullOrEmpty(authToken))
                {
                    throw new ArgumentNullException();
                }

                var isAuthenticated = ValidateAuthToken(authToken);

                if (!isAuthenticated)
                {
                    throw new SecurityTokenException();
                }

                var user = await _userRepository.GetTokenUser(login);

                response = Ok(user.Token);
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest, "Некорректный или отсутствующий токен.");
            }
            catch (OperationCanceledException)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера: операция была отменена.");
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера.");
            }

            return response;
        }

        /// <summary>
        /// Обрабатывает запрос на авторизацию пользователя через GitLab.
        /// </summary>
        /// <returns>Перенаправление пользователя на страницу авторизации GitLab.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status400BadRequest">Ошибка входных данных: код авторизации отсутствует или некорректный.</exception>
        [HttpGet("get_link")]
        public async Task<IActionResult> GetLink()
        {
            IActionResult response;

            try
            {
                var authorizationUrl = await _gitLabService.GetAuthorizationUrl();

                response = Ok(authorizationUrl);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }

            return response;
        }

        /// <summary>
        /// Обрабатывает обратный вызов (callback) после успешной авторизации пользователя через GitLab.
        /// </summary>
        /// <param name="code">Код авторизации, полученный от GitLab.</param>
        /// <returns>Результат обработки обратного вызова.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status400BadRequest">Код авторизации не может быть пустым или некорректные входные данные.</exception>
        /// <exception cref="Status401Unauthorized">Ошибка авторизации: отсутствуют или недействительны аутентификационные данные.</exception>
        [HttpGet("access_token")]
        public async Task<IActionResult> GetAccessToken(string code)
        {
            IActionResult response;

            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new ArgumentNullException(nameof(code), "Код авторизации не может быть пустым.");
                }

                var tokens = await _gitLabService.ExchangeCodeForAccessToken(code);

                if (string.IsNullOrEmpty(tokens.access_token) ||
                    string.IsNullOrEmpty(tokens.refresh_token))
                {
                    throw new ArgumentNullException("Отсутствует токен");
                }

                var user = await _gitLabService.GetCurrentUser(tokens.access_token);

                if (user == null)
                {
                    throw new ArgumentNullException();
                }

                if (!await _userRepository.AuthorizationViaGitLab(user.Username))
                {
                    await _userRepository.RegisterUserInDB(user.Username);
                }

                await _userRepository.SetTokenUser(user.Username, tokens.access_token, tokens.refresh_token);

                var useri = await _userRepository.GetTokenUser(user.Username);

                if (useri == null)
                {
                    throw new ArgumentNullException();
                }

                var jwtToken = GenerateJwtToken(useri.Login);

                Response.Cookies.Append("auth_token", jwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddHours(720)
                });

                response = Ok(_mapper.Map<UserDto>(useri));
            }
            catch (ArgumentNullException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest, "Код авторизации не может быть пустым.");
            }
            catch (InvalidOperationException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest, "Некорректные входные данные.");
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status401Unauthorized, "Ошибка авторизации: отсутствуют или некорректны аутентификационные данные.");
            }

            return response;
        }

        /// <summary>
        /// Обновляет токен доступа.
        /// </summary>
        /// <param name="refreshToken">Код  обновления, полученный от GitLab.</param>
        /// <returns>Результат обработки обратного вызова.</returns>
        /// <exception cref="Status200OK">Успешное выполнение обратного вызова.</exception>
        /// <exception cref="Status400BadRequest">Токен обновления не может быть пустым или некорректные входные данные.</exception>
        /// <exception cref="Status401Unauthorized">Ошибка авторизации: отсутствуют или недействительны аутентификационные данные.</exception>
        [HttpGet("get_refresh_token")]
        public async Task<IActionResult> GetRefreshToken(string refreshToken)
        {
            IActionResult response;

            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    throw new ArgumentNullException(nameof(refreshToken), "Токен обновления не может быть пустым.");
                }

                var tokens = await _gitLabService.RefreshAccessToken(refreshToken);

                if (string.IsNullOrEmpty(tokens.access_token) ||
                    string.IsNullOrEmpty(tokens.refresh_token))
                {
                    throw new ArgumentNullException("Отсутствует токен");
                }

                var user = await _gitLabService.GetCurrentUser(tokens.access_token);

                if (user == null)
                {
                    throw new ArgumentNullException();
                }

                await _userRepository.SetTokenUser(user.Username, tokens.access_token, tokens.refresh_token);

                response = Ok(_mapper.Map<TokenResponseDto>(tokens));
            }
            catch (ArgumentNullException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest, "Токен обновления не может быть пустым.");
            }
            catch (InvalidOperationException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest, "Некорректные входные данные.");
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status401Unauthorized, "Ошибка авторизации: отсутствуют или некорректны аутентификационные данные.");
            }

            return response;
        }
    }
}