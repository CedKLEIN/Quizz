using System.Text.Json;
using Quizz.Enums;
using Quizz.Interfaces;
using Quizz.Models;

namespace Quizz.Services;

public class NodeBuilderService(IConsole console, ContentService contentService, RuleEngineService ruleEngineService)
{
    public async Task<Node> BuildAsync()
    {
        var mainJson = contentService.MainPath;
        var mainContent = await File.ReadAllTextAsync(mainJson);
        var node = JsonSerializer.Deserialize<Node>(mainContent);

        if (node is null)
        {
            throw new NullReferenceException("Content is null");
        }
        await ParseChildAsync(node, Path.Combine(contentService.RootContentPath, node.Name));
        return node;
    }
    
    private async Task ParseChildAsync(Node node, string path)
    {
        node.Children ??= await GetChildrenFromFile(node, path);

        if (node.Type == PageType.Menu)
        {
            foreach (var child in node.Children)
            {
                await ParseChildAsync(child, Path.Combine(path, child.Name));
            }
        }
    }
    
    private async Task<List<Node>> GetChildrenFromFile(Node node, string path)
    {
        var childrenPath = contentService.ChildPath(path, node.Name);
        if (!File.Exists(childrenPath))
        {
            console.WriteLine($"File {childrenPath} does not exist");
            return [];
        }
        
        var childrenContent = await File.ReadAllTextAsync(childrenPath);
        var children = JsonSerializer.Deserialize<List<Node>>(childrenContent);
            
        if (children is null)
        {
            console.WriteLine($"Children for path {path} is null");
            return [];
        }
        
        var conditions = node.Conditions;
        if (conditions is not null)
        {
            children = children.Where(n => ruleEngineService.EvaluateConditions(conditions, n)).ToList();
        }

        return children;
    }

    private void DisplayNode(Node node, int level = 1)
    {
        var startLine = new string(' ', level);
        console.WriteLine($"{startLine}Starting level {level} for {node.Name}");
        console.WriteLine($"{startLine}name: {node.Name}");
        console.WriteLine($"{startLine}label: {node.Label}");
        console.WriteLine($"{startLine}Type: {node.Type.ToString()}");

        foreach (var child in node.Children)
        {
            DisplayNode(child, level + 1);
        }
    }
}