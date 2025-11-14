using Quizz.Enums;
using Quizz.Interfaces;
using Quizz.Models;

namespace Quizz.Services;

public class NodeParserService(IConsole console, Node content)
{
    private readonly List<string> _yesAnswers = ["yes", "y", "oui", "ui", "o"];

    private class Result
    {
        public bool Success { get; set; }
    }
    
    private Result Parse(Node node)
    {
        switch (node.Type)
        {
            case PageType.Menu:
                return ParseNode(node);
            case PageType.Quiz:
                return ParseQuiz(node);
            case PageType.Question:
            case PageType.QuestionMultipleResponses:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void Parse()
    {
        if (content is null)
        {
            throw new NullReferenceException("Content is null");
        }

        while (true)
        {
            if (!ParseNode(content).Success)
                break;
        }
    }

    private Result ParseNode(Node node)
    {
        var result = new Result();
        
        console.Clear();
        console.WriteLine(node.Label)
            .BreakLine();

        if (node.Children is null)
        {
            return result;
        }
        
        for (var i = 0; i < node.Children.Count; i++)
        {
            console.WriteLine($"{i + 1}. {node.Children[i].Label}");
        }
        
        console.WriteLine("0. Back")
            .BreakLine();
        
        var input = console.ReadLine();
        console.BreakLine();
        if (!int.TryParse(input, out var choice) || choice < 0 || choice > node.Children.Count)
        {
            console.WriteLine("Invalid choice. Press any key to try again...");
            console.ReadKey();
            result.Success = true;
            return result;
        }

        if (choice == 0) return result;
            
        var selectedChild = node.Children[choice - 1];
        Parse(selectedChild);
        
        return new Result { Success = true }; // true to not exist the application
    }

    private Result ParseQuiz(Node node)
    {
        console.Clear();
        console.WriteLine("Use 'exit()' to leave the quiz")
            .BreakLine();

        var sorted = node.Get<bool?>("sorted") ?? true;
        var children =  sorted ? SortChildrenRandomly(node) : node.Children;
        var successAnswer = 0;
        var wrongAnswers = new List<Node>();
        for(var i = 0; i < children.Count; i++)
        {
            console.WriteLine($"{i+1}/{children.Count} - {children[i].Question} ?")
                .BreakLine();
            if (node.Children[i].Type == PageType.Question)
            {
                var input = console.ReadLine();
                while (string.IsNullOrEmpty(input))
                {
                    console.WriteLine("Invalid choice. Please try again...");
                    input = console.ReadLine();
                }

                if (input == "exit()")
                {
                    break;
                }

                if (input == children[i].Answer)
                {
                    successAnswer++;
                    console.BreakLine()
                        .WriteLine("✅ - Correct!");
                }
                else
                {
                    wrongAnswers.Add(children[i]);
                    console.BreakLine()
                        .WriteLine($"❌ - Incorrect, the answer is '{children[i].Answer}'");
                }

                console.BreakLine();
            }
            else if(node.Children[i].Type == PageType.QuestionMultipleResponses)
            {
                    console.WriteLine($"Number of answers: {children[i].MultipleAnswer.Count} (click on enter between each response)");
                    
                    var answers = new List<string>();
                    var count = 1;
                    foreach (var unused in children[i].MultipleAnswer)
                    {
                        console.Write($"{count++} - ");
                        var inputMultiple = console.ReadLine();
                        while (string.IsNullOrEmpty(inputMultiple))
                        {
                            console.WriteLine("Invalid choice. Please try again...");
                            inputMultiple = console.ReadLine();
                        }
                        if (inputMultiple == "exit()")
                        {
                            break;
                        }
                        answers.Add(inputMultiple);
                        
                    }
                    var areEqual = children[i].MultipleAnswer.OrderBy(x => x).SequenceEqual(answers.OrderBy(x => x));
                    if (areEqual)
                    {
                        successAnswer++;
                        console.BreakLine().WriteLine("✅ - Correct, all response are good!")
                            .BreakLine();
                    }
                    else
                    {
                        wrongAnswers.Add(children[i]);
                        console.BreakLine()
                            .Write("Missing responses:'")
                            .Write("❌  ");
                        foreach (var goodAnswer in children[i].MultipleAnswer)
                        {
                            if (!answers.Contains(goodAnswer))
                            {
                                console.Write($" {goodAnswer} - ");
                            }
                        }
                    }
                    
                    console.WriteLine();
                    console.WriteLine($"Score final: {successAnswer}/{children.Count} réponses correctes !");
                    console.WriteLine();
                    console.ReadKey();
            }
            else
            {
                console.WriteLine($"The type of {node.Children[i].Type} is not supported");
                console.WriteLine();
            }
            HandleCommentIfAny(children[i]);
        }
        console.WriteLine($"Score final: {successAnswer}/{children.Count} réponses correctes !");

        if (wrongAnswers.Count > 0)
        {
            console.WriteLine();
            console.WriteLine("Veux-tu un sommaire des réponses fausses ?");
            var responses = console.ReadLine();
            if(!string.IsNullOrEmpty(responses) && _yesAnswers.Contains(responses.ToLowerInvariant()))
            {
                console.WriteLine();
                console.WriteLine("Réponses fausses:");
                foreach (var wrongAnswer in wrongAnswers)
                {
                    console.WriteLine($"❌ - {wrongAnswer.Question} - {wrongAnswer.Answer}");
                }
                console.WriteLine();
                console.ReadKey();
            }
        }

        return new Result { Success = true };
    }

    private static List<Node> SortChildrenRandomly(Node node)
    {
        var rnd = new Random();
        var children = node.Children.OrderBy(_ => rnd.Next()).ToList();
        return children;
    }

    private void HandleCommentIfAny(Node node)
    {
        var comment = node.Get<string>("comment");
        if (!string.IsNullOrEmpty(comment))
        {
            console.WriteLine($"{comment}");
            console.WriteLine();
        }
    }
}