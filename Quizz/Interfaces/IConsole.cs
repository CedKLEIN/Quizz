namespace Quizz.Interfaces;

public interface IConsole
{
    public void Clear();
    public IConsole Write(string text = "");
    public IConsole WriteLine(string text = "");
    public void ReadKey();
    public string? ReadLine();
    public IConsole BreakLine();
}