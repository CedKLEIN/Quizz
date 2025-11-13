using Quizz.Interfaces;

namespace Quizz.Services;

public class ConsoleService : IConsole
{
    public void Clear() => Console.Clear();

    public IConsole Write(string text = "")
    {
        Console.Write(text);
        return this;
    }

    public IConsole WriteLine(string text = "")
    {
        Console.WriteLine(text);
        return this;
    }
    
    public void ReadKey() => Console.ReadKey();
    public string? ReadLine() => Console.ReadLine();
    public IConsole BreakLine()
    {
        Console.WriteLine();
        return this;
    }
}