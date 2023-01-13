﻿using Newtonsoft.Json;

using IHost host = Host.CreateDefaultBuilder(args)
    // .ConfigureServices((_, services) => services.AddGitHubActionServices())
    .Build();

static TService Get<TService>(IHost host)
    where TService : notnull =>
    host.Services.GetRequiredService<TService>();

var logger = Get<ILoggerFactory>(host)
    .CreateLogger("DotNet.GitHubAction.Program");
var parser = Default.ParseArguments<ActionInputs>(() => new(), args);
parser.WithNotParsed(
    errors =>
    {
        logger
            .LogError(
                string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        
        Environment.Exit(2);
    });
var done = false;
await parser.WithParsedAsync(async options =>
{
    var environmentVariable = Environment.GetEnvironmentVariable("GITHUB_CONTEXT");
    var context = JsonConvert.DeserializeObject<GithubActionContext_pullrequest>(environmentVariable);

    logger.LogInformation($"Options: {JsonConvert.SerializeObject(options,Formatting.Indented)}");
    await new Logic(logger, options, context).DoDaThing();
    done = true;
});
logger.LogInformation($"Done: {done}");
Environment.Exit(0);
