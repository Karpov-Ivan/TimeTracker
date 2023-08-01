using System;
using System.Net;
using TimeTracker.Interfaces.Services;
using Newtonsoft.Json;
using TimeTracker.Models;
using GitLabServices.GitLabModels;
using GitLabServices.Mapper;
using GitLabProject = GitLabServices.GitLabModels.GitLabProject;
using GitLabDeveloper = GitLabServices.GitLabModels.GitLabDeveloper;
using GitLabIssue = GitLabServices.GitLabModels.GitLabIssue;
using TimeTracker.Common;
using Newtonsoft.Json.Linq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using RestSharp;
using TimeTracker.Models.Models;
using AutoMapper;
using AngleSharp;
using AngleSharp.Dom;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GitLabServices
{
    public class GitLabService : IGitLabService
    {
        private readonly string? _clientId;
        private readonly string? _clientSecret;
        private readonly string? _redirectUri;
        private readonly string? _gitLabBaseUrl;
        private readonly string? _gitLabBaseUrlGlobal;

        public GitLabService(IConfiguration configuration)
        {
            _clientId = configuration.GetSection("ClientId").Value;
            _clientSecret = configuration.GetSection("ClientSecret").Value;
            _redirectUri = configuration.GetSection("RedirectUri").Value;
            _gitLabBaseUrl = configuration.GetSection("GitLabBaseUrl").Value;
            _gitLabBaseUrlGlobal = configuration.GetSection("GitLabBaseUrlGlobal").Value;
        }

        public async Task<User> GetCurrentUser(string gitLabToken)
        {
            try
            {
                if (string.IsNullOrEmpty(gitLabToken))
                {
                    throw new ArgumentNullException();
                }

                var uriAdress = $"{_gitLabBaseUrl}" +
                                "/api/v4/user?access_token=" +
                                $"{gitLabToken}";

                var dataJson = await new Parser().GetJson(uriAdress);

                var gitLabUser = JsonConvert.DeserializeObject<GitLabUser>(dataJson);

                var mapper = MapperGitLabData.Initialize();

                var user = mapper.Map<User>(gitLabUser);

                user.Token = gitLabToken;

                return user;
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<int> GetCountPage(string gitLabToken)
        {
            try
            {
                if (string.IsNullOrEmpty(_gitLabBaseUrl) ||
                    string.IsNullOrEmpty(gitLabToken))
                {
                    throw new ArgumentNullException();
                }

                var page = 1;
                var countProject = 0;
                var projects = new List<GitLabProject>();
                do
                {
                    var uriAdress = $"{_gitLabBaseUrl}" +
                                    "/api/v4" +
                                    "/projects/?page=" +
                                    $"{page.ToString()}" +
                                    "&per_page=100&access_token=" +
                                    $"{gitLabToken}";

                    var dataJson = await new Parser().GetJson(uriAdress);

                    projects = JsonConvert.DeserializeObject<List<GitLabProject>>(dataJson);

                    if (projects == null)
                        throw new ArgumentNullException();

                    countProject += projects.Count();

                    page++;

                } while (projects.Count() != 0);

                return countProject;
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<List<Project>?> GetListOfUserProjects(int page, string gitLabToken)
        {
            try
            {
                Console.WriteLine("_gitLabBaseUrl: " + _gitLabBaseUrl);
                Console.WriteLine("gitLabToken: " + gitLabToken);
                
                if (string.IsNullOrEmpty(_gitLabBaseUrl))
                    throw new ArgumentNullException(nameof(_gitLabBaseUrl), $"Can't be null {nameof(_gitLabBaseUrl)}");
                if (page <= 0)
                    throw new ArgumentNullException(nameof(page), $"Can't be {page}");

                var gitLabProject = await GetProjects(page, gitLabToken);

                if (gitLabProject == null)
                {
                    throw new ArgumentNullException(nameof(gitLabProject), $"Can't be null {nameof(gitLabProject)}");
                }

                await PopulateDevelopersForProjects(gitLabProject, gitLabToken);
                await PopulateIssuesForProjects(gitLabProject, gitLabToken);
                await PopulateMergeRequestsForProjects(gitLabProject, gitLabToken);
                await PopulateCommentIMRProject(gitLabProject, gitLabToken);

                var mapper = MapperGitLabData.Initialize();
                var projects = mapper.Map<List<Project>>(gitLabProject);

                return projects;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        private async Task<List<GitLabProject>?> GetProjects(int page, string gitLabToken)
        {
            try
            {
                Console.WriteLine("_gitLabBaseUrl: " + _gitLabBaseUrl);
                Console.WriteLine("gitLabToken: " + gitLabToken);
                
                if (string.IsNullOrEmpty(_gitLabBaseUrl))
                    throw new ArgumentNullException(nameof(_gitLabBaseUrl), $"Can't be null {nameof(_gitLabBaseUrl)}");
                if (string.IsNullOrEmpty(gitLabToken))
                    throw new ArgumentNullException(nameof(gitLabToken), $"Can't be null {nameof(gitLabToken)}");
                
                var uriAdress = $"{_gitLabBaseUrl}" +
                                "/api/v4" +
                                "/projects/?page=" +
                                $"{page.ToString()}" +
                                "&per_page=5&access_token=" +
                                $"{gitLabToken}";

                var dataJson = await new Parser().GetJson(uriAdress);

                var projects = JsonConvert.DeserializeObject<List<GitLabProject>>(dataJson);

                return projects;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        private async Task PopulateDevelopersForProjects(List<GitLabProject> projects, string gitLabToken)
        {
            try
            {
                foreach (var project in projects)
                {
                    if (string.IsNullOrEmpty(project.Id.ToString()) ||
                        string.IsNullOrEmpty(_gitLabBaseUrl) ||
                        string.IsNullOrEmpty(gitLabToken) ||
                        !int.TryParse(project.Id.ToString(), out int intValue))
                    {
                        throw new ArgumentNullException();
                    }

                    var uriAdress = $"{_gitLabBaseUrl}" +
                                    "/api/v4/projects/" +
                                    $"{project.Id.ToString()}" +
                                    "/members/?per_page=100&access_token=" +
                                    $"{gitLabToken}";

                    var dataJson = await new Parser().GetJson(uriAdress);

                    var developers = JsonConvert.DeserializeObject<List<GitLabDeveloper>>(dataJson);

                    project.Developers = developers;
                }
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        private async Task PopulateIssuesForProjects(List<GitLabProject> projects, string gitLabToken)
        {
            try
            {
                foreach (var project in projects)
                {
                    if (string.IsNullOrEmpty(project.Id.ToString()) ||
                        string.IsNullOrEmpty(_gitLabBaseUrl) ||
                        string.IsNullOrEmpty(gitLabToken) ||
                        !int.TryParse(project.Id.ToString(), out int intValue))
                    {
                        throw new ArgumentNullException();
                    }

                    var adress = $"{_gitLabBaseUrl}" +
                                    "/api/v4/projects/" +
                                    $"{project.Id.ToString()}" +
                                    "/issues/?per_page=100&access_token=" +
                                    $"{gitLabToken}";

                    var dataJson = await new Parser().GetJson(adress);

                    var issues = JsonConvert.DeserializeObject<List<GitLabIssue>>(dataJson);

                    project.Issues = issues;
                }
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        private async Task PopulateMergeRequestsForProjects(List<GitLabProject> projects, string gitLabToken)
        {
            try
            {
                foreach (var project in projects)
                {
                    if (string.IsNullOrEmpty(project.Id.ToString()) ||
                        string.IsNullOrEmpty(_gitLabBaseUrl) ||
                        string.IsNullOrEmpty(gitLabToken) ||
                        !int.TryParse(project.Id.ToString(), out int intValue))
                    {
                        throw new ArgumentNullException();
                    }

                    var adress = $"{_gitLabBaseUrl}" +
                                 "/api/v4/projects/" +
                                 $"{project.Id.ToString()}" +
                                 "/merge_requests/?per_page=100&access_token=" +
                                 $"{gitLabToken}";

                    var dataJson = await new Parser().GetJson(adress);

                    var mergeRequests = JsonConvert.DeserializeObject<List<GitLabMergeRequest>>(dataJson);

                    project.MergeRequests = mergeRequests;
                }
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        private async Task PopulateCommentIMRProject(List<GitLabProject> projects, string gitLabToken)
        {
            try
            {
                foreach (var project in projects)
                {
                    if (string.IsNullOrEmpty(project.Id.ToString()) ||
                        string.IsNullOrEmpty(_gitLabBaseUrl) ||
                        string.IsNullOrEmpty(gitLabToken) ||
                        !int.TryParse(project.Id.ToString(), out int intValue))
                    {
                        throw new ArgumentNullException();
                    }

                    var commentsGitLab = new List<GitLabComment>();

                    var mapper = MapperGitLabData.Initialize();

                    if (project.MergeRequests != null)
                    {
                        foreach (var mergeRequest in project.MergeRequests)
                        {
                            commentsGitLab.AddRange(await GetCommentMRById(project.Id.ToString(), mapper.Map<MergeRequest>(mergeRequest), gitLabToken));
                        }
                    }

                    if (project.Issues != null)
                    {
                        foreach (var issue in project.Issues)
                        {
                            commentsGitLab.AddRange(await GetCommentIssueById(project.Id.ToString(), mapper.Map<Issue>(issue), gitLabToken));
                        }
                    }

                    project.Comments = commentsGitLab;
                }
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        public async Task<List<Project>?> GetUserProjectById(string projectId, string gitLabToken)
        {
            try
            {
                if (string.IsNullOrEmpty(gitLabToken) ||
                    string.IsNullOrEmpty(projectId))
                {
                    throw new ArgumentNullException();
                }

                var gitLabProjects = await GetProjectById(projectId, gitLabToken);

                if (gitLabProjects == null)
                {
                    throw new ArgumentNullException();
                }

                var project = gitLabProjects.FirstOrDefault(p => p.Id.ToString() == projectId);

                if (project == null)
                {
                    throw new ArgumentNullException();
                }

                var gitLabProject = new List<GitLabProject> { project };

                await PopulateDevelopersForProjects(gitLabProject, gitLabToken);
                await PopulateIssuesForProjects(gitLabProject, gitLabToken);
                await PopulateMergeRequestsForProjects(gitLabProject, gitLabToken);

                var mapper = MapperGitLabData.Initialize();
                var projects = mapper.Map<List<Project>>(gitLabProject);

                return projects;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        private async Task<List<GitLabProject>?> GetProjectById(string projectId, string gitLabToken)
        {
            try
            {
                if (string.IsNullOrEmpty(_gitLabBaseUrl) ||
                    string.IsNullOrEmpty(gitLabToken) ||
                    string.IsNullOrEmpty(projectId))
                {
                    throw new ArgumentNullException();
                }
                var adress = $"{_gitLabBaseUrl}" +
                                "/api/v4/projects/" +
                                $"{projectId.ToString()}" +
                                "/?access_token=" +
                                $"{gitLabToken}";

                var dataJson = await new Parser().GetJson(adress);

                var project = JsonConvert.DeserializeObject<GitLabProject>(dataJson);

                if (project == null)
                {
                    throw new ArgumentNullException();
                }

                var projectList = new List<GitLabProject> { project };

                return projectList;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        public async Task<List<Issue>?> GetProjectIssuesById(string projectId, string gitLabToken)
        {
            try
            {
                if (string.IsNullOrEmpty(gitLabToken) ||
                    string.IsNullOrEmpty(projectId))
                {
                    throw new ArgumentNullException();
                }

                var gitLabProject = await GetProjectById(projectId, gitLabToken);

                if (gitLabProject == null)
                {
                    throw new ArgumentNullException();
                }

                await PopulateIssuesForProjects(gitLabProject, gitLabToken);

                var mapper = MapperGitLabData.Initialize();
                var ussues = mapper.Map<List<Issue>>(gitLabProject[0].Issues);

                return ussues;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        public async Task<List<MergeRequest>> GetProjectMergeRequestsById(string projectId, string gitLabToken)
        {
            try
            {
                if (string.IsNullOrEmpty(gitLabToken) ||
                    string.IsNullOrEmpty(projectId))
                {
                    throw new ArgumentNullException();
                }

                var gitLabProject = await GetProjectById(projectId, gitLabToken);

                if (gitLabProject == null)
                {
                    throw new ArgumentNullException();
                }

                await PopulateMergeRequestsForProjects(gitLabProject, gitLabToken);

                var mapper = MapperGitLabData.Initialize();
                var mergeRequests = mapper.Map<List<MergeRequest>>(gitLabProject[0].MergeRequests);

                return mergeRequests;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        public async Task<List<Comment>> GetCommentIMRProjectById(string projectId, string gitLabToken)
        {
            try
            {
                if (string.IsNullOrEmpty(gitLabToken) ||
                    string.IsNullOrEmpty(projectId))
                {
                    throw new ArgumentNullException();
                }

                var gitLabProject = await GetProjectById(projectId, gitLabToken);

                if (gitLabProject == null)
                {
                    throw new ArgumentNullException();
                }

                var project = gitLabProject.FirstOrDefault(p => p.Id.ToString() == projectId);

                if (project == null)
                {
                    throw new ArgumentNullException();
                }

                var commentsGitLab = new List<GitLabComment>();

                var mergeRequests = await GetProjectMergeRequestsById(project.Id.ToString(), gitLabToken);

                var issues = await GetProjectIssuesById(project.Id.ToString(), gitLabToken);

                if (mergeRequests == null || issues == null)
                {
                    throw new ArgumentNullException();
                }

                foreach (var mergeRequest in mergeRequests)
                {
                    commentsGitLab.AddRange(await GetCommentMRById(project.Id.ToString(), mergeRequest, gitLabToken));
                }

                foreach (var issue in issues)
                {
                    commentsGitLab.AddRange(await GetCommentIssueById(project.Id.ToString(), issue, gitLabToken));
                }

                var mapper = MapperGitLabData.Initialize();
                var comments = mapper.Map<List<Comment>>(commentsGitLab);

                comments.OrderByDescending(c => c.CreatedAt);

                return comments;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        private async Task<List<GitLabComment>> GetCommentMRById(string projectId, MergeRequest mergeRequest, string gitLabToken)
        {
            try
            {
                if (string.IsNullOrEmpty(gitLabToken) ||
                    string.IsNullOrEmpty(mergeRequest.Id.ToString()))
                {
                    throw new ArgumentNullException();
                }

                var uriAdress = $"{_gitLabBaseUrl}" +
                                "/api/v4/projects/" +
                                $"{projectId.ToString()}" +
                                "/merge_requests/" +
                                $"{mergeRequest.Id.ToString()}" +
                                "/notes?per_page=100&access_token=" +
                                $"{gitLabToken}";

                var dataJson = await new Parser().GetJson(uriAdress);

                List<GitLabComment> comments;

                try
                {
                    comments = JsonConvert.DeserializeObject<List<GitLabComment>>(dataJson);
                }
                catch (JsonSerializationException)
                {
                    comments = new List<GitLabComment>();
                }

                var filteredComments = comments.Where(c => string.IsNullOrEmpty(c.Body) == false &&
                    ((c.Body.Contains("added", StringComparison.OrdinalIgnoreCase) &&
                     c.Body.Contains("of time spent", StringComparison.OrdinalIgnoreCase)) ||
                    (c.Body.Contains("subtracted", StringComparison.OrdinalIgnoreCase) &&
                     c.Body.Contains("of time spent", StringComparison.OrdinalIgnoreCase)))).ToList();

                foreach (var comment in filteredComments)
                {
                    comment.IdIOrMR = Convert.ToInt32(mergeRequest.Id.ToString());
                    comment.Type = "MR";
                    comment.Title = mergeRequest.Title;

                    if (string.IsNullOrEmpty(comment.Body) == false)
                        comment.TotalTimeStats = TimeConverter.GetTimeInSeconds(comment.Body);
                }

                return filteredComments;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        private async Task<List<GitLabComment>> GetCommentIssueById(string projectId, Issue issue, string gitLabToken)
        {
            try
            {
                if (string.IsNullOrEmpty(gitLabToken) ||
                    string.IsNullOrEmpty(issue.Id.ToString()))
                {
                    throw new ArgumentNullException();
                }

                var uriAdress = $"{_gitLabBaseUrl}" +
                                "/api/v4/projects/" +
                                $"{projectId.ToString()}" +
                                "/issues/" +
                                $"{issue.Id.ToString()}" +
                                "/notes?per_page=100&access_token=" +
                                $"{gitLabToken}";

                var dataJson = await new Parser().GetJson(uriAdress);

                List<GitLabComment> comments;

                try
                {
                    comments = JsonConvert.DeserializeObject<List<GitLabComment>>(dataJson);
                }
                catch (JsonSerializationException)
                {
                    comments = new List<GitLabComment>();
                }

                var filteredComments = comments.Where(c => string.IsNullOrEmpty(c.Body) == false &&
                    ((c.Body.Contains("added", StringComparison.OrdinalIgnoreCase) &&
                     c.Body.Contains("of time spent", StringComparison.OrdinalIgnoreCase)) ||
                    (c.Body.Contains("subtracted", StringComparison.OrdinalIgnoreCase) &&
                     c.Body.Contains("of time spent", StringComparison.OrdinalIgnoreCase)))).ToList();
    
                foreach (var comment in filteredComments)
                {
                    comment.IdIOrMR = Convert.ToInt32(issue.Id.ToString());
                    comment.Type = "Issue";
                    comment.Title = issue.Title;

                    if (string.IsNullOrEmpty(comment.Body) == false)
                        comment.TotalTimeStats = TimeConverter.GetTimeInSeconds(comment.Body);
                }

                return filteredComments;
            }
            catch (ArgumentException exception)
            {
                Console.WriteLine(exception);
                throw new ArgumentException(exception.Message);
            }
            catch (UriFormatException exception)
            {
                Console.WriteLine(exception);
                throw new UriFormatException(exception.Message);
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine(exception);
                throw new HttpRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception(exception.Message);
            }
        }

        public Task<string> GetAuthorizationUrl()
        {
            try
            {
                var authorizationEndpoint = $"{_gitLabBaseUrlGlobal}/oauth/authorize";
                var parameters = new
                {
                    client_id = _clientId,
                    redirect_uri = _redirectUri,
                    response_type = "code",
                    scope = "api"
                };

                var url =
                    $"{authorizationEndpoint}?client_id={parameters.client_id}" +
                    $"&redirect_uri={WebUtility.UrlEncode(parameters.redirect_uri)}" +
                    $"&response_type={parameters.response_type}" +
                    $"&scope={parameters.scope}";

                return Task.FromResult(url);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new Exception("Failed to get authorization URL", exception);
            }
        }

        public async Task<TokenResponse?> ExchangeCodeForAccessToken(string code)
        {
            var tokens = new TokenResponse();

            try
            {
                var uriAdress = $"{_gitLabBaseUrl}" +
                                "/oauth/token/?" +
                                "client_id=" +
                                $"{_clientId}" +
                                "&client_secret=" +
                                $"{_clientSecret}" +
                                "&code=" +
                                $"{code}" +
                                "&grant_type=authorization_code" +
                                "&redirect_uri=" +
                                $"{_redirectUri}";

                using (HttpClient client = new HttpClient())
                {
                    var response = client.PostAsync(uriAdress, null).Result;
                    var datasJson = await response.Content.ReadAsStringAsync();
                    var gitLabTokenResponse = JsonConvert.DeserializeObject<GitLabTokenResponse>(datasJson);

                    if (gitLabTokenResponse != null)
                    {
                        var mapper = MapperGitLabData.Initialize();
                        tokens = mapper.Map<TokenResponse>(gitLabTokenResponse);
                    }
                }
            }
            catch (RestSharp.DeserializationException exception)
            {
                throw new Exception("Failed to exchange code for access token", exception);
            }

            return tokens;
        }

        public async Task<TokenResponse?> RefreshAccessToken(string refreshToken)
        {
            var tokens = new TokenResponse();

            try
            {
                var uriAdress = $"{_gitLabBaseUrl}/oauth/token/?" +
                $"client_id={_clientId}&" +
                $"client_secret={_clientSecret}&" +
                $"refresh_token={refreshToken}&" +
                "grant_type=refresh_token&" +
                $"redirect_uri={_redirectUri}";

                using (HttpClient client = new HttpClient())
                {
                    var response = client.PostAsync(uriAdress, null).Result;
                    var datasJson = await response.Content.ReadAsStringAsync();
                    var gitLabTokenResponse = JsonConvert.DeserializeObject<GitLabTokenResponse>(datasJson);

                    if (gitLabTokenResponse != null)
                    {
                        var mapper = MapperGitLabData.Initialize();
                        tokens = mapper.Map<TokenResponse>(gitLabTokenResponse);
                    }
                }
            }
            catch (RestSharp.DeserializationException exception)
            {
                throw new Exception("Failed to exchange code for access token", exception);
            }

            return tokens;
        }
    }
}