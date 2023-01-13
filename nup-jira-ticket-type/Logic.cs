using Atlassian.Jira;
using DotNet.GitHubAction.GithubActionModels;
using DotNet.GitHubAction.JiraLogic;
using MoreLinq;
using Newtonsoft.Json;
using Octokit.GraphQL;

namespace DotNet.GitHubAction;

public class Logic
{
    private readonly ILogger _logger;
    private readonly ActionInputs _options;
    private readonly GithubActionContext_pullrequest _githubContext;
    private JiraAbstraction _jiraAbstraction;
    private string _currentBranchName;
    private List<string> _ignoreJiraStates;

    public Logic(ILogger logger, ActionInputs options, GithubActionContext_pullrequest? githubContext)
    {
        _logger = logger;
        _options = options;
        _githubContext = githubContext;
        _currentBranchName = _githubContext.HeadRef.Split("/").Last();
        _jiraAbstraction = new JiraAbstraction(_logger, options.JiraUrl, options.JiraUser, options.JiraApiKey);
    }

    public async Task DoDaThing()
    {
        _logger.LogInformation($"Branch name: {_currentBranchName}");

        if (_githubContext.EventName != "pull_request")
        {
            _logger.LogInformation($"exit due to event is {_githubContext.EventName}");
            return;   
        }
        if (new List<string> { "dev", "release", "main" }.Contains(_currentBranchName.ToLowerInvariant()))
        {
            _logger.LogInformation($"exit due to current branch is {_currentBranchName}");
            return;
        }
        var ids = JiraIssueStringSearcher.FindIds(_currentBranchName);
        var jiraIssues = await _jiraAbstraction.findJiraIssues(ids.ToArray());
        _logger.LogInformation(
            $"Found the following Ids in Jira: {JsonConvert.SerializeObject(jiraIssues.Select(x => x.Key), Formatting.Indented)}");

        if (jiraIssues.Any() == false || !jiraIssues.All(x => x.Value.Type.IsSubTask))
        {
            throw new Exception("Jira ticket is not of type subtask");
        }

        _logger.LogInformation(
            $"Branch is associated with the subtask: {jiraIssues.First().Value.Key}");
    }
}