using Quizz.Enums;
using Quizz.Interfaces;
using Quizz.Models;

namespace Quizz.Services;

public class NodeParserService(IConsole console, NodeBuilderService nodeBuilderService)
{
    private readonly List<string> _yesAnswers = ["yes", "y", "oui", "ui", "o"];

    private enum State
    {
        Success,
        Failed,
        Exit,
    }

    private class Result
    {
        public State State { get; set; } = State.Failed;

        public bool Success => State == State.Success;
    }

    private Result Parse(Node node) =>
        node.Type switch
        {
            PageType.Menu => ParseMenu(node),
            PageType.Quiz => ParseQuiz(node),
            PageType.Question => ParseQuestion(node.Answer),
            PageType.QuestionMultipleResponses => ParseQuestionMultipleResponses(node),
            _ => throw new ArgumentOutOfRangeException(),
        };

    public async Task StartAsync()
    {
        var rootNode = await nodeBuilderService.BuildAsync();
        while (true)
        {
            if (!Parse(rootNode).Success)
                break;
        }
    }

    private Result ParseMenu(Node node)
    {
        var result = new Result();

        console.Clear();
        console.WriteLine(node.Label).BreakLine();

        if (node.Children is null)
        {
            return result;
        }

        if (node.Children.Count == 0)
        {
            console.WriteLine("No menus found").BreakLine();
        }

        for (var i = 0; i < node.Children.Count; i++)
        {
            console.WriteLine($"{i + 1}. {node.Children[i].Label}");
        }

        console.WriteLine("0. Back").BreakLine();

        var input = console.ReadLine();
        console.BreakLine();
        if (!int.TryParse(input, out var choice) || choice < 0 || choice > node.Children.Count)
        {
            console.WriteLine("Invalid choice. Press any key to try again...");
            console.ReadKey();
            result.State = State.Success;
            return result;
        }

        if (choice == 0)
            return result;

        var selectedChild = node.Children[choice - 1];
        Parse(selectedChild);

        return new Result { State = State.Success }; // force true to not exist the application
    }

    private Result ParseQuestionMultipleResponses(Node node)
    {
        console.WriteLine(
            $"{node.MultipleAnswer.Count} answers needed (click on enter between each response)"
        );

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
        var ordering = node.Get<bool?>("ordering") ?? false;
        var areEqual = ordering
            ? node.MultipleAnswer.SequenceEqual(answers)
            : node.MultipleAnswer.OrderBy(x => x).SequenceEqual(answers.OrderBy(x => x));
        if (areEqual)
        {
            result.State = State.Success;
            console.BreakLine().WriteLine("✅ - Correct, all response are good!").BreakLine();
        }
        else
        {
            result.State = State.Failed;
            console.BreakLine().Write("Missing responses:'").Write("❌  ");
            for (var j = 0; j < node.MultipleAnswer.Count; j++)
            {
                if (!answers.Contains(node.MultipleAnswer[j]))
                {
                    console.Write($"{node.MultipleAnswer[j]}");
                    if (j != node.MultipleAnswer.Count - 1)
                    {
                        console.Write(" ; ");
                    }
                }
            }
            console.BreakLine().BreakLine();
        }

        return result;
    }

    private Result ParseQuestion(string? answer)
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

        if (input == answer)
        {
            console.BreakLine().WriteLine("✅ - Correct!").BreakLine();

            return new Result { State = State.Success };
        }

        console
            .BreakLine()
            .WriteLine($"❌ - Incorrect, the answer is '{answer}'")
            .BreakLine();

        return new Result { State = State.Failed };
    }

    private Result ParseQuiz(Node node)
    {
        console.Clear();
        console.WriteLine("Use 'exit()' to leave the quiz").BreakLine();
        
        var children = node.Shuffle ? SortChildrenRandomly(node) : node.Children;
        var successAnswer = 0;
        var wrongAnswers = new List<Node>();
        var i = 1;
        var direction = node.Direction;
        foreach(var child in children)
        {
            var (question, answer) = GetQuestionAnswer(child, direction);
            console.WriteLine($"{i}/{children.Count} - {question} ?").BreakLine();

            var result = child.Type == PageType.Question ? ParseQuestion(answer) : ParseQuestionMultipleResponses(child);
            if (result.State == State.Success)
                successAnswer++;
            else if (result.State == State.Failed)
                wrongAnswers.Add(child);
            else if (result.State == State.Exit)
                break;
            else
                throw new Exception($"Unhandle state: {result.State}");

            HandleCommentIfAny(child);
            i++;
        }
        console.WriteLine($"Score final: {successAnswer}/{children.Count} réponses correctes !");

        if (wrongAnswers.Count > 0)
        {
            console.WriteLine();
            console.WriteLine("Veux-tu un sommaire des réponses fausses ?");
            var responses = console.ReadLine();
            if (
                string.IsNullOrEmpty(responses)
                || _yesAnswers.Contains(responses.ToLowerInvariant())
            )
            {
                console.WriteLine();
                console.WriteLine("Réponses fausses:");
                foreach (var wrongAnswer in wrongAnswers)
                {
                    console.WriteLine(
                        $"❌ - {wrongAnswer.Question} - {wrongAnswer.Answer ?? string.Join(" ; ", wrongAnswer.MultipleAnswer)}"
                    );
                }
                console.WriteLine();
                console.ReadKey();
            }
        }

        return new Result { State = State.Success };
    }

    private (string? Question, string? Answer) GetQuestionAnswer(Node child, Direction direction)
    {
        while (true)
        {
            switch (direction)
            {
                case Direction.Forward:
                    return (child.Question, child.Answer);
                case Direction.Reverse:
                    return (child.Answer, child.Question);
                case Direction.Random:
                {
                    var isForward = new Random().Next() % 2 == 0;
                    direction = isForward ? Direction.Forward : Direction.Reverse;
                    continue;
                }
                default:
                    direction = Direction.Forward;
                    continue;
            }
        }
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
