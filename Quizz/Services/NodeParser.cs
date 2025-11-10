using Quizz.Enums;
using Quizz.Models;

namespace Quizz.Services;

public class NodeParser(Node content)
{
    private readonly List<string> _yesAnswers = ["yes", "y", "oui", "ui", "o"];
    
    public void Parse()
    {
        if (content is null)
        {
            throw new NullReferenceException("Content is null");
        }

        while (true)
        {
            if (!ParseNode(content))
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
        Console.WriteLine("Use 'exit()' to leave the quiz\n");
        
        var rnd = new Random();
        var children = node.Children.OrderBy(_ => rnd.Next()).ToList();
        var successAnswer = 0;
        var wrongAnswers = new List<Node>();
        for(var i = 0; i < children.Count; i++)
        {
            Console.WriteLine($"{i+1}/{children.Count+1} - {children[i].Question} ({children[i].Get<string>("continent")}) ?\n");
            var input = Console.ReadLine();
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid choice. Please try again...");
                input = Console.ReadLine();
            }

            if (input == "exit()")
            {
                break;
            }

            if (input == children[i].Answer)
            {
                successAnswer++;
                Console.WriteLine("\n✅ - Correct!\n");
            }
            else
            {
                wrongAnswers.Add(children[i]);
                Console.WriteLine($"\n❌ - Incorrect, the answer is '{children[i].Answer}'\n");
            }

            var comment = children[i].Get<string>("comment");
            if (!string.IsNullOrEmpty(comment))
            {
                Console.WriteLine($"{comment}\n");
            }
        }
        
        Console.WriteLine($"Score final: {successAnswer}/{children.Count+1} réponses correctes !");

        if (wrongAnswers.Count > 0)
        {
            Console.WriteLine("Veux-tu un sommaire des réponses fausses ?");
            var responses = Console.ReadLine();
            if(!string.IsNullOrEmpty(responses) && _yesAnswers.Contains(responses.ToLowerInvariant()))
            {
                Console.WriteLine("Réponses fausses:");
                foreach (var wrongAnswer in wrongAnswers)
                {
                    Console.WriteLine($"❌ - {wrongAnswer.Question} - {wrongAnswer.Answer}");
                }
                Console.ReadKey();
            }
        }
    }
}