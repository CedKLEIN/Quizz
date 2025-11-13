using Quizz.Enums;
using Quizz.Services;

namespace Quizz.Tests.Services;

public class NodeBuilderServiceTests
{
    [Test]
    public async Task Build()
    {
        var nodeBuilderService = new NodeBuilderService(new ConsoleService(), new ContentService("TestContent"), new RuleEngineService());
        var rootNode = await nodeBuilderService.BuildAsync();
        
        Assert.That(rootNode, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(rootNode.Name, Is.EqualTo("Menu"));
            Assert.That(rootNode.Label, Is.EqualTo("Which theme for the quiz ?"));
            Assert.That(rootNode.Type, Is.EqualTo(PageType.Menu));
            Assert.That(rootNode.Children, Has.Count.EqualTo(1));
            Assert.That(rootNode.Children[0].Name, Is.EqualTo("Geography"));
            Assert.That(rootNode.Children[0].Label, Is.EqualTo("Geography"));
            Assert.That(rootNode.Children[0].Children, Has.Count.EqualTo(1));
            Assert.That(rootNode.Children[0].Type, Is.EqualTo(PageType.Menu));
            Assert.That(rootNode.Children[0].Children[0].Name, Is.EqualTo("Capitals"));
            Assert.That(rootNode.Children[0].Children[0].Label, Is.EqualTo("Capitals"));
            Assert.That(rootNode.Children[0].Children[0].Type, Is.EqualTo(PageType.Menu));
            Assert.That(rootNode.Children[0].Children[0].Children, Has.Count.EqualTo(1));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Name, Is.EqualTo("Monde"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Label, Is.EqualTo("Toutes les capitales"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Type, Is.EqualTo(PageType.Quiz));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children, Has.Count.EqualTo(3));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[0].Question, Is.EqualTo("Afghanistan"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[0].Answer, Is.EqualTo("Kaboul"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[0].Type, Is.EqualTo(PageType.Question));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[0].Get<string>("continent"), Is.EqualTo("Asie"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[1].Question, Is.EqualTo("Afrique du Sud"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[1].Answer, Is.EqualTo("Pretoria"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[1].Type, Is.EqualTo(PageType.Question));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[1].Get<string>("continent"), Is.EqualTo("Afrique"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[2].Question, Is.EqualTo("Albanie"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[2].Answer, Is.EqualTo("Tirana"));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[2].Type, Is.EqualTo(PageType.Question));
            Assert.That(rootNode.Children[0].Children[0].Children[0].Children[2].Get<string>("continent"), Is.EqualTo("Europe"));
            
        });
    }
}