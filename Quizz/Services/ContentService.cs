using Quizz.Configuration;

namespace Quizz.Services;

public class ContentService(QuizzConfiguration configuration)
{
    public string RootContentPath =>
        Path.Combine(AppContext.BaseDirectory, configuration.BaseDirectoryName);

    public string MainPath => Path.Combine(RootContentPath, configuration.JsonFileName);

    public string ChildPath(string parentPath) =>
        Path.Combine(parentPath, configuration.JsonFileName);
}
