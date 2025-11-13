using Quizz.Services;
using Quizz.Tests.Mocks;

namespace Quizz.Tests.Services;

public class NodeParserServiceTests
{
    [Test]
    public async Task Menu_LeaveApp()
    {
        var console = new ConsoleServiceMock();
        var nodeBuilderService = new NodeBuilderService(console, new ContentService("TestContent"), new RuleEngineService());
        var rootNode = await nodeBuilderService.BuildAsync();

        var nodeParserService = new NodeParserService(console, rootNode);
        
        console.AddLine("0");
        nodeParserService.Parse();

        Assert.That(console.ConsoleText, Is.EqualTo("""
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?

                                                    1. Geography
                                                    0. Exit

                                                    0
                                                    
                                                    
                                                    """));
    }
    
    [Test]
    public async Task Menu_Return_LeaveApp()
    {
        var console = new ConsoleServiceMock();
        var nodeBuilderService = new NodeBuilderService(console, new ContentService("TestContent"), new RuleEngineService());
        var rootNode = await nodeBuilderService.BuildAsync();

        var nodeParserService = new NodeParserService(console, rootNode);
        
        console.AddLine("1");
        console.AddLine("0");
        console.AddLine("0");
        nodeParserService.Parse();
        
        Assert.That(console.ConsoleText, Is.EqualTo("""
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Geography
                                                    
                                                    1. Capitals
                                                    0. Exit
                                                    
                                                    0
                                                    
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    0
                                                    
                                                    
                                                    """));
    }
    
    [Test]
    public async Task Menu_Empty_Response()
    {
        var console = new ConsoleServiceMock();
        var nodeBuilderService = new NodeBuilderService(console, new ContentService("TestContent"), new RuleEngineService());
        var rootNode = await nodeBuilderService.BuildAsync();

        var nodeParserService = new NodeParserService(console, rootNode);
        
        console.AddLine("1");
        console.AddLine("");
        console.AddLine("0");
        nodeParserService.Parse();

        Console.WriteLine(console.ConsoleText);
        
        Assert.That(console.ConsoleText, Is.EqualTo("""
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Geography
                                                    
                                                    1. Capitals
                                                    0. Exit
                                                    
                                                    
                                                    
                                                    Invalid choice. Press any key to try again...
                                                    *Console.ReadKey()*
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    0
                                                    
                                                    
                                                    """));
    }
    
    [Test]
    public async Task Menu_Quizz_Question_Failed_Response()
    {
        var console = new ConsoleServiceMock();
        var nodeBuilderService = new NodeBuilderService(console, new ContentService("TestContent"), new RuleEngineService());
        var rootNode = await nodeBuilderService.BuildAsync();

        var nodeParserService = new NodeParserService(console, rootNode);
        
        console.AddLine("1");
        console.AddLine("1");
        console.AddLine("1");
        console.AddLine("Wrong answer");
        console.AddLine("Pretoria"); // good answer
        console.AddLine("Wrong answer");
        console.AddLine("Yes");
        console.AddLine("0");
        nodeParserService.Parse();

        Console.WriteLine(console.ConsoleText);
        
        Assert.That(console.ConsoleText, Is.EqualTo("""
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Geography
                                                    
                                                    1. Capitals
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Capitals
                                                    
                                                    1. Toutes les capitales
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Use 'exit()' to leave the quiz
                                                    
                                                    1/3 - Afghanistan ?
                                                    
                                                    Wrong answer
                                                    
                                                    ❌ - Incorrect, the answer is 'Kaboul'
                                                    
                                                    2/3 - Afrique du Sud ?
                                                    
                                                    Pretoria
                                                    
                                                    ✅ - Correct!
                                                    
                                                    Some comment
                                                    
                                                    3/3 - Albanie ?
                                                    
                                                    Wrong answer
                                                    
                                                    ❌ - Incorrect, the answer is 'Tirana'
                                                    
                                                    Score final: 1/3 réponses correctes !
                                                    
                                                    Veux-tu un sommaire des réponses fausses ?
                                                    Yes
                                                    
                                                    Réponses fausses:
                                                    ❌ - Afghanistan - Kaboul
                                                    ❌ - Albanie - Tirana
                                                    
                                                    *Console.ReadKey()*
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    0
                                                    
                                                    
                                                    """));
    }
    
    [Test]
    public async Task Menu_Quizz_Empty_Response()
    {
        var console = new ConsoleServiceMock();
        var nodeBuilderService = new NodeBuilderService(console, new ContentService("TestContent"), new RuleEngineService());
        var rootNode = await nodeBuilderService.BuildAsync();

        var nodeParserService = new NodeParserService(console, rootNode);
        
        console.AddLine("1");
        console.AddLine("1");
        console.AddLine("1");
        console.AddLine("Wrong answer");
        console.AddLine("");
        console.AddLine("");
        console.AddLine("");
        console.AddLine("");
        console.AddLine("Pretoria"); // good answer
        console.AddLine("Wrong answer");
        console.AddLine("Yes");
        console.AddLine("0");
        nodeParserService.Parse();

        Console.WriteLine(console.ConsoleText);
        
        Assert.That(console.ConsoleText, Is.EqualTo("""
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Geography
                                                    
                                                    1. Capitals
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Capitals
                                                    
                                                    1. Toutes les capitales
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Use 'exit()' to leave the quiz
                                                    
                                                    1/3 - Afghanistan ?
                                                    
                                                    Wrong answer
                                                    
                                                    ❌ - Incorrect, the answer is 'Kaboul'
                                                    
                                                    2/3 - Afrique du Sud ?
                                                    
                                                    
                                                    Invalid choice. Please try again...
                                                    
                                                    Invalid choice. Please try again...
                                                    
                                                    Invalid choice. Please try again...
                                                    
                                                    Invalid choice. Please try again...
                                                    Pretoria
                                                    
                                                    ✅ - Correct!
                                                    
                                                    Some comment
                                                    
                                                    3/3 - Albanie ?
                                                    
                                                    Wrong answer
                                                    
                                                    ❌ - Incorrect, the answer is 'Tirana'
                                                    
                                                    Score final: 1/3 réponses correctes !
                                                    
                                                    Veux-tu un sommaire des réponses fausses ?
                                                    Yes
                                                    
                                                    Réponses fausses:
                                                    ❌ - Afghanistan - Kaboul
                                                    ❌ - Albanie - Tirana
                                                    
                                                    *Console.ReadKey()*
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    0
                                                    
                                                    
                                                    """));
    }
    
    [Test]
    public async Task Menu_Quizz_exit()
    {
        var console = new ConsoleServiceMock();
        var nodeBuilderService = new NodeBuilderService(console, new ContentService("TestContent"), new RuleEngineService());
        var rootNode = await nodeBuilderService.BuildAsync();

        var nodeParserService = new NodeParserService(console, rootNode);
        
        console.AddLine("1");
        console.AddLine("1");
        console.AddLine("1");
        console.AddLine("Wrong answer");
        console.AddLine("exit()");
        console.AddLine("No");
        console.AddLine("0");
        nodeParserService.Parse();

        Console.WriteLine(console.ConsoleText);
        
        Assert.That(console.ConsoleText, Is.EqualTo("""
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Geography
                                                    
                                                    1. Capitals
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Capitals
                                                    
                                                    1. Toutes les capitales
                                                    0. Exit
                                                    
                                                    1
                                                    
                                                    *Console.Clear()*
                                                    Use 'exit()' to leave the quiz
                                                    
                                                    1/3 - Afghanistan ?
                                                    
                                                    Wrong answer
                                                    
                                                    ❌ - Incorrect, the answer is 'Kaboul'
                                                    
                                                    2/3 - Afrique du Sud ?
                                                    
                                                    exit()
                                                    Score final: 0/3 réponses correctes !
                                                    
                                                    Veux-tu un sommaire des réponses fausses ?
                                                    No
                                                    *Console.Clear()*
                                                    Which theme for the quiz ?
                                                    
                                                    1. Geography
                                                    0. Exit
                                                    
                                                    0
                                                    
                                                    
                                                    """));
    }
}