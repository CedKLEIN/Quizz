using Quizz.Services;

namespace Quizz;

internal abstract class Program
{
    public static async Task Main()
    {
        // To display emotes in console
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        var contentService = new ContentService();
        var ruleEngineService = new RuleEngineService();
        var nodeBuilderService = new NodeBuilderService(contentService, ruleEngineService);
        
        var rootNode = await nodeBuilderService.BuildAsync();
        var nodeParser = new NodeParser(rootNode);
        nodeParser.Parse();
    }
}