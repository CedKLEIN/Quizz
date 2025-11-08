using Quizz.Services;

namespace Quizz;

internal abstract class Program
{
    public static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        var contentService = new ContentService();
        var app = new App(contentService);
        await app.InitAsync();
        app.Start();
    }
}