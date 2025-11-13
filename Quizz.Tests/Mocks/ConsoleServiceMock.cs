using System.Text;
using Quizz.Interfaces;

namespace Quizz.Tests.Mocks;

public class ConsoleServiceMock: IConsole
{
    private readonly StringBuilder _consoleText = new();
    
    private readonly Queue<string> _consoleLines = new();
    
    public string ConsoleText => _consoleText.ToString();
    
    public void AddLine(string line) => _consoleLines.Enqueue(line);
    
    public void Clear() => _consoleText.AppendLine("*Console.Clear()*");

    public IConsole Write(string text = "")
    {
        _consoleText.Append(text);
        return this;
    }

    public IConsole WriteLine(string text = "")
    {
        _consoleText.AppendLine(text);
        return this;
    }

    public void ReadKey() => _consoleText.AppendLine("*Console.ReadKey()*");

    public string ReadLine()
    {
        var input = _consoleLines.Dequeue();
        _consoleText.AppendLine(input);
        return input;
    }

    public IConsole BreakLine()
    {
        _consoleText.AppendLine();
        return this;
    }
}