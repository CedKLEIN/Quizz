namespace Quizz.Interfaces;

public interface IConsole
{
    public void Clear();
    public void Write(string text = "");
    public void WriteLine(string text = "");
    public void ReadKey();
    public string? ReadLine();
}