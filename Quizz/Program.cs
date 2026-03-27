using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quizz.Interfaces;
using Quizz.Services;

namespace Quizz;

internal abstract class Program
{
    public static async Task Main()
    {
        // To display emotes in console
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<IConsole, ConsoleService>();
        builder.Services.AddSingleton<ContentService>();
        builder.Services.AddSingleton<RuleEngineService>();
        builder.Services.AddSingleton<NodeBuilderService>();
        builder.Services.AddSingleton<NodeParserService>();

        var host = builder.Build();
        var nodeParser = host.Services.GetRequiredService<NodeParserService>();
        await nodeParser.StartAsync();
    }
}
