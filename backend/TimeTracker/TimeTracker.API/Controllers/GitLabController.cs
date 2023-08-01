using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using GitLabServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TimeTracker.Dto;
using TimeTracker.Interfaces.Services;
using TimeTracker.Models.Models;

namespace TimeTracker.Controllers
{
    /// <summary>
    /// Контроллер для работы с GitLab API.
    /// </summary>
    [ApiController]
    [Route(template: "api/[controller]")]
    public class projectsController : Controller
	{
        private readonly IGitLabService _gitLabService;

        protected readonly IMapper _mapper;

        private readonly IConfiguration _configuration;

        [JsonObject]
        public class ProjectListResponse
        {
            [JsonPropertyName("total_count")]
            public int TotalCount { get; set; }

            [JsonPropertyName("projects")]
            public List<ProjectDto> Projects { get; set; }
        }

        public projectsController(IGitLabService gitLabService, IMapper mapper)
		{
            _gitLabService = gitLabService;

            _mapper = mapper;
        }

        /// <summary>
        /// Получает количество страниц.
        /// </summary>
        /// <param name="page">Страница проектов.</param>
        /// <param name="token">Токен пользователя.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <exception cref="Status200OK">Успешное выполнение запроса.</exception>
        /// <exception cref="Status400BadRequest">Некорректный аргумент.</exception>
        /// <exception cref="Status500InternalServerError">Внутренняя ошибка сервера.</exception>
        [HttpGet("count_projects")]
        public async Task<IActionResult> GetCountProjects(string? token)
        {
            IActionResult response;

            try
            {
                var authToken = HttpContext.Request.Cookies["auth_token"];

                if (string.IsNullOrEmpty(authToken) ||
                    string.IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException();
                }

                var isAuthenticated = ValidateAuthToken(authToken);

                if (!isAuthenticated)
                {
                    throw new SecurityTokenException();
                }

                var countPage = await _gitLabService.GetCountPage(token);

                response = Ok(countPage);
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (UriFormatException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (HttpRequestException)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Получает все проекты пользователя.
        /// </summary>
        /// <param name="page">Страница проектов.</param>
        /// <param name="token">Токен пользователя.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <exception cref="Status200OK">Успешное выполнение запроса.</exception>
        /// <exception cref="Status400BadRequest">Некорректный аргумент.</exception>
        /// <exception cref="Status500InternalServerError">Внутренняя ошибка сервера.</exception>
        [HttpGet]
        public async Task<IActionResult> GetAllProjects(int page)
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
                if (page <= 0)
                    throw new ArgumentNullException(nameof(page), $"Can't be {page}");

                var isAuthenticated = ValidateAuthToken(authToken);

                if (!isAuthenticated)
                {
                    throw new SecurityTokenException();
                }

                var projects = await _gitLabService.GetListOfUserProjects(page, token);

                var totalCount = -1;

                if (page == 1)
                {
                    totalCount = await _gitLabService.GetCountPage(token);
                }

                var projectListResponse = new ProjectListResponse
                {
                    TotalCount = totalCount,

                    Projects = _mapper.Map<List<ProjectDto>>(projects)
                };

                response = Ok(projectListResponse);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine(ex);
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex);
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Получает проект пользователя по id.
        /// </summary>
        /// <param name="projectId">Id проекта.</param>
        /// <param name="token">Токен пользователя.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <exception cref="Status200OK">Успешное выполнение запроса.</exception>
        /// <exception cref="Status400BadRequest">Некорректный аргумент.</exception>
        /// <exception cref="Status500InternalServerError">Внутренняя ошибка сервера.</exception>
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById(string? projectId)
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
                if (string.IsNullOrEmpty(projectId))
                    throw new ArgumentNullException(nameof(projectId), $"Can't be null {nameof(projectId)}");
                if (!int.TryParse(projectId, out int intValue))
                    throw new ArgumentException(nameof(projectId), $"Can't be number {nameof(projectId)}");

                var isAuthenticated = ValidateAuthToken(authToken);

                if (!isAuthenticated)
                {
                    throw new SecurityTokenException();
                }

                var projects = await _gitLabService.GetUserProjectById(projectId, token);

                if (projects == null)
                {
                    throw new ArgumentNullException();
                }

                response = Ok(_mapper.Map<ProjectDto>(projects[0]));
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (UriFormatException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (HttpRequestException)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
         }

        /// <summary>
        /// Получает issues пользователя по id проекта.
        /// </summary>
        /// <param name="projectId">Id проекта.</param>
        /// <param name="token">Токен пользователя.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <exception cref="Status200OK">Успешное выполнение запроса.</exception>
        /// <exception cref="Status400BadRequest">Некорректный аргумент.</exception>
        /// <exception cref="Status500InternalServerError">Внутренняя ошибка сервера.</exception>
        [HttpGet("{projectId}/issues")]
        public async Task<IActionResult> GetProjectIssuesById(string? projectId, string? token)
        {
            IActionResult response;

            try
            {
                var authToken = HttpContext.Request.Cookies["auth_token"];

                if (string.IsNullOrEmpty(projectId) ||
                    string.IsNullOrEmpty(token) ||
                    string.IsNullOrEmpty(authToken))
                {
                    throw new ArgumentNullException();
                }

                if (!int.TryParse(projectId, out int intValue))
                {
                    throw new ArgumentException();
                }

                var isAuthenticated = ValidateAuthToken(authToken);

                if (!isAuthenticated)
                {
                    throw new SecurityTokenException();
                }

                var issues = await _gitLabService.GetProjectIssuesById(projectId, token);

                response = Ok(_mapper.Map<List<IssueDto>>(issues));
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (UriFormatException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (HttpRequestException)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Получает merge requests пользователя по id проекта.
        /// </summary>
        /// <param name="projectId">Id проекта.</param>
        /// <param name="token">Токен пользователя.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <exception cref="Status200OK">Успешное выполнение запроса.</exception>
        /// <exception cref="Status400BadRequest">Некорректный аргумент.</exception>
        /// <exception cref="Status500InternalServerError">Внутренняя ошибка сервера.</exception>
        [HttpGet("{projectId}/merge_requests")]
        public async Task<IActionResult> GetProjectMergeRequestsById(string? projectId, string? token)
        {
            IActionResult response;

            try
            {
                var authToken = HttpContext.Request.Cookies["auth_token"];

                if (string.IsNullOrEmpty(projectId) ||
                    string.IsNullOrEmpty(token) ||
                    string.IsNullOrEmpty(authToken))
                {
                    throw new ArgumentNullException();
                }

                if (!int.TryParse(projectId, out int intValue))
                {
                    throw new ArgumentException();
                }

                var isAuthenticated = ValidateAuthToken(authToken);

                if (!isAuthenticated)
                {
                    throw new SecurityTokenException();
                }

                var issues = await _gitLabService.GetProjectMergeRequestsById(projectId, token);

                response = Ok(_mapper.Map<List<MergeRequestDto>>(issues));
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (UriFormatException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (HttpRequestException)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        /// <summary>
        /// Получает комментарии issues и merge requests пользователя по id проекта.
        /// </summary>
        /// <param name="projectId">Id проекта.</param>
        /// <param name="token">Токен пользователя.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <exception cref="Status200OK">Успешное выполнение запроса.</exception>
        /// <exception cref="Status400BadRequest">Некорректный аргумент.</exception>
        /// <exception cref="Status500InternalServerError">Внутренняя ошибка сервера.</exception>
        [HttpGet("{projectId}/comments")]
        public async Task<IActionResult> GetCommentsByProjectId(string? projectId)
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
                if (string.IsNullOrEmpty(projectId))
                    throw new ArgumentNullException(nameof(projectId), $"Can't be null {nameof(projectId)}");
                if (!int.TryParse(projectId, out int intValue))
                    throw new ArgumentException(nameof(projectId), $"Can't be number {nameof(projectId)}");

                var isAuthenticated = ValidateAuthToken(authToken);

                if (!isAuthenticated)
                {
                    throw new SecurityTokenException();
                }

                var comments = await _gitLabService.GetCommentIMRProjectById(projectId, token);

                response = Ok(_mapper.Map<List<CommentDto>>(comments));
            }
            catch (ArgumentException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (UriFormatException)
            {
                response = StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (HttpRequestException)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
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
    }
}