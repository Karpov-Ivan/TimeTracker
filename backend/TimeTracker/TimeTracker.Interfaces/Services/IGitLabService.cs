using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using TimeTracker.Models;
using TimeTracker.Models.Models;

namespace TimeTracker.Interfaces.Services;

public interface IGitLabService
{
    public Task<TokenResponse?> ExchangeCodeForAccessToken(string code);

    public Task<TokenResponse?> RefreshAccessToken(string refreshToken);

    public Task<string> GetAuthorizationUrl();

    public Task<User> GetCurrentUser(string gitLabToken);

    public Task<int> GetCountPage(string gitLabToken);

    public Task<List<Project>?> GetListOfUserProjects(int page, string gitLabToken);

    public Task<List<Project>?> GetUserProjectById(string projectId, string gitLabToken);

    public Task<List<Issue>?> GetProjectIssuesById(string projectId, string gitLabToken);

    public Task<List<MergeRequest>> GetProjectMergeRequestsById(string projectId, string gitLabToken);

    public Task<List<Comment>> GetCommentIMRProjectById(string projectId, string gitLabToken);
}