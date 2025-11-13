using Quizz.Services;

namespace Quizz;

internal abstract class Program
{
    public static async Task Main()
    {
        // To display emotes in console
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var console = new ConsoleService();
        var contentService = new ContentService();
        var ruleEngineService = new RuleEngineService();
        var nodeBuilderService = new NodeBuilderService(console, contentService, ruleEngineService);
        
        var rootNode = await nodeBuilderService.BuildAsync();
        var nodeParser = new NodeParserService(console, rootNode);
        nodeParser.Parse();
    }
}