using System.Text;
using System.Text.Json;
using Quizz.Enums;
using Quizz.Models;

namespace Quizz.Services;

public class App(ContentService contentService)
{
    private Node? _content;
    
    public async Task InitAsync()
    {
        var mainJson = contentService.MainPath;
        var mainContent = await File.ReadAllTextAsync(mainJson);
        _content = JsonSerializer.Deserialize<Node>(mainContent);

        if (_content is null)
        {
            throw new NullReferenceException("Content is null");
        }
        await ParseChildAsync(_content, Path.Combine(contentService.RootContentPath, _content.Title));
        
        //DisplayNode(_content);
    }

    public void Start()
    {
        if (_content is null)
        {
            throw new NullReferenceException("Content is null");
        }

        while (true)
        {
            if (!ParseNode(_content))
                break;
        }
    }

    private bool ParseNode(Node node)
    {
        Console.Clear();
        Console.WriteLine(node.Label);
        Console.WriteLine();
        for (var i = 0; i < node.Children.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {node.Children[i].Label}");
        }
        
        Console.WriteLine("0. Exit");
        
        var input = Console.ReadLine();
        if (!int.TryParse(input, out var choice) || choice < 0 || choice > node.Children.Count)
        {
            Console.WriteLine("Invalid choice. Press any key to try again...");
            Console.ReadKey();
            return true;
        }

        if (choice == 0) return false;
            
        var selectedChild = node.Children[choice - 1];
        switch (selectedChild.Type)
        {
            case PageType.Menu:
                ParseNode(selectedChild);
                break;
            case PageType.Quiz:
                ParseQuiz(selectedChild);
                break;
            case PageType.Question:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return true;
    }

    private void ParseQuiz(Node node)
    {
        Console.Clear();
        Console.WriteLine("(Use 'exit()' to leave the quiz)");
        
        var rnd = new Random();
        var children = node.Children.OrderBy(c => rnd.Next()).ToList();
        var successAnswer = 0;
        for(var i = 0; i < children.Count; i++)
        {
            Console.WriteLine($"{i+1}/{children.Count+1} - {children[i].Question} ({children[i].Get<string>("continent")}) ?");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid choice. Press any key to try again...");
                Console.ReadKey();
                return;
            }

            if (input == "exit()")
            {
                return;
            }

            if (input == children[i].Answer)
            {
                successAnswer++;
                Console.WriteLine("\n✅ - Correct!\n");
            }
            else
            {
                Console.WriteLine($"\n❌ - Incorrect, the answer is '{children[i].Answer}'\n");
            }

            var comment = children[i].Get<string>("comment");
            if (!string.IsNullOrEmpty(comment))
            {
                Console.WriteLine($"{comment}\n");
            }
        }
        
        Console.WriteLine($"Score final: {successAnswer}/{children.Count+1} réponses correctes !");
        Console.ReadKey();
    }

    private async Task ParseChildAsync(Node node, string path)
    {
        var title = node.Title;
        var childrenPath = contentService.ChildPath(path, title);

        if (!File.Exists(childrenPath))
        {
            Console.WriteLine($"File {childrenPath} does not exist");
            return;
        }
        
        var childrenContent = await File.ReadAllTextAsync(childrenPath);
        var children = JsonSerializer.Deserialize<List<Node>>(childrenContent);
            
        if (children is null)
        {
            Console.WriteLine($"Children for path {path} is null");
            return;
        }
            
        node.Children = children;
        if (node.Type == PageType.Menu)
        {
            foreach (var child in children)
            {
                await ParseChildAsync(child, Path.Combine(path, child.Title));
            }
        }
    }
    
    private void DisplayNode(Node node, int level = 1)
    {
        var startLine = new string(' ', level);
        Console.WriteLine($"{startLine}Starting level {level} for {node.Title}");
        Console.WriteLine($"{startLine}title: {node.Title}");
        Console.WriteLine($"{startLine}label: {node.Label}");
        Console.WriteLine($"{startLine}Type: {node.Type.ToString()}");

        foreach (var child in node.Children)
        {
            DisplayNode(child, level + 1);
        }
    }
}