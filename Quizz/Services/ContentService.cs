namespace Quizz.Services;

public class ContentService
{
    public string RootContentPath => Path.Combine(AppContext.BaseDirectory, "Content");
    
    public string MainPath => Path.Combine(RootContentPath, "main.json");
    
    public string ChildPath(string parentPath, string fileName) => Path.Combine(parentPath, $"{fileName.ToLowerInvariant()}.json");
}