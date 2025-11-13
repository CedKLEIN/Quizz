using Quizz.Interfaces;

namespace Quizz.Services;

public class ConsoleService : IConsole
{
    public void Clear() => Console.Clear();
    public void Write(string text = "") => Console.Write(text);
    public void WriteLine(string text = "") => Console.WriteLine(text);
    public void ReadKey() => Console.ReadKey();
    public string? ReadLine() => Console.ReadLine();
}