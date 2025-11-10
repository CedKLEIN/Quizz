namespace Quizz.Services;

public class ContentService(string baseDirectoryName = "Content")
{
    public string RootContentPath => Path.Combine(AppContext.BaseDirectory, baseDirectoryName);
    
    public string MainPath => Path.Combine(RootContentPath, "main.json");
    
    public string ChildPath(string parentPath, string fileName) => Path.Combine(parentPath, $"{fileName.ToLowerInvariant()}.json");
}