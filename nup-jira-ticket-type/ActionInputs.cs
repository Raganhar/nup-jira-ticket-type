namespace DotNet.GitHubAction;

public class ActionInputs
{
    string _repositoryName = null!;
    string _branchName = null!;

    [Option("jira-url", HelpText = "sample text")]
    public string JiraUrl { get; set; }

    [Option("jira-user", HelpText = "sample text")]
    public string JiraUser { get; set; }

    [Option("jira-api-key", HelpText = "sample text")]
    public string JiraApiKey { get; set; }

    [Option("branch", HelpText = "sample text")]
    public string Branch { get; set; }
}