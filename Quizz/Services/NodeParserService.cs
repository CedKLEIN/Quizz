using Quizz.Enums;
using Quizz.Interfaces;
using Quizz.Models;

namespace Quizz.Services;

public class NodeParserService(IConsole console, Node content)
{
    private readonly List<string> _yesAnswers = ["yes", "y", "oui", "ui", "o"];


    private enum State
    {
        Success,
        Failed,
        Exit
    }
    private class Result
    {
        public State State { get; set; } = State.Failed;
        
        public bool Success => State == State.Success;
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
            result.State = State.Success;
            return result;
        }

        if (choice == 0) return result;
            
        var selectedChild = node.Children[choice - 1];
        Parse(selectedChild);
        
        return new Result { State = State.Success }; // true to not exist the application
    }

    private Result QuestionMultipleResponses(Node node)
    {
        console.WriteLine($"Number of answers: {node.MultipleAnswer.Count} (click on enter between each response)");
                    
        var answers = new List<string>();
        var count = 1;
        foreach (var unused in node.MultipleAnswer)
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
                return new Result { State = State.Exit };
            }
            answers.Add(inputMultiple);
            
        }
        
        var result = new Result();
        var areEqual = node.MultipleAnswer.OrderBy(x => x).SequenceEqual(answers.OrderBy(x => x));
        if (areEqual)
        {
            result.State = State.Success;
            console.BreakLine().WriteLine("✅ - Correct, all response are good!")
                .BreakLine();
        }
        else
        {
            result.State = State.Failed;
            console.BreakLine()
                .Write("Missing responses:'")
                .Write("❌  ");
            foreach (var goodAnswer in node.MultipleAnswer)
            {
                if (!answers.Contains(goodAnswer))
                {
                    console.Write($" {goodAnswer} - ");
                }
            }
        }
        
        // console.WriteLine();
        // console.WriteLine($"Score final: {successAnswer}/{children.Count} réponses correctes !");
        // console.WriteLine();
        // console.ReadKey();
        
        return result;
    }

    private Result ParseQuestion(Node node)
    {
        var input = console.ReadLine();
        while (string.IsNullOrEmpty(input))
        {
            console.WriteLine("Invalid choice. Please try again...");
            input = console.ReadLine();
        }

        if (input == "exit()")
        {
            return new Result { State = State.Exit };
        }

        if (input == node.Answer)
        {
            console.BreakLine()
                .WriteLine("✅ - Correct!").BreakLine();
            
            return new Result { State = State.Success };
        }
        else
        {
            console.BreakLine()
                .WriteLine($"❌ - Incorrect, the answer is '{node.Answer}'").BreakLine();
            
            return new Result { State = State.Failed };
        }
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
                var result = ParseQuestion(children[i]);

                if(result.State == State.Success) successAnswer++;
                else if (result.State == State.Failed) wrongAnswers.Add(node.Children[i]);
                else if (result.State == State.Exit) break;
                else throw new Exception($"Unhandle state: {result.State}");
            }
            else if(node.Children[i].Type == PageType.QuestionMultipleResponses)
            {
                    console.WriteLine($"{children[i].MultipleAnswer.Count} answers needed (click on enter between each response)");
                    
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
                        wrongAnswers.Add(node.Children[i]);
                        console.BreakLine()
                            .Write("Missing responses:'")
                            .Write("❌  ");

                        for (var j = 0; j < children[i].MultipleAnswer.Count; j++)
                        {
                            if (!answers.Contains(children[i].MultipleAnswer[j]))
                            {
                                console.Write($"{children[i].MultipleAnswer[j]}");
                                if (j != children[i].MultipleAnswer.Count - 1)
                                {
                                    console.Write(" ; ");
                                }
                            }
                        }
                        console.BreakLine().BreakLine();
                    }
            }
            else
            {
                console.WriteLine($"The type of {node.Children[i].Type} is not supported").BreakLine();
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
                    console.WriteLine($"❌ - {wrongAnswer.Question} - {wrongAnswer.Answer ?? string.Join(" ; ", wrongAnswer.MultipleAnswer)}");
                }
                console.WriteLine();
                console.ReadKey();
            }
        }

        return new Result { State = State.Success };
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