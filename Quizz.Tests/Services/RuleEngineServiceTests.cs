using System.Text.Json;
using Quizz.Models;
using Quizz.Services;

namespace Quizz.Tests.Services;

public class RuleEngineServiceTests
{
    private const string JsonCapitals = """
                                        [
                                            {"question": "Afghanistan", "answer": "Kaboul", "continent": "Asie", "type": "question"},
                                            {"question": "Afrique du Sud", "answer": "Pretoria", "comment":"Pretoria (administrative), Le Cap (législative), Bloemfontein (judiciaire)", "continent": "Afrique", "type": "question"},
                                            {"question": "Albanie", "answer": "Tirana", "continent": "Europe", "type": "question"},
                                            {"question": "Algérie", "answer": "Alger", "continent": "Afrique", "type": "question"},
                                            {"question": "Allemagne", "answer": "Berlin", "continent": "Europe", "type": "question"}
                                        ]
                                        """;
    
    [Test]
    public void EvaluateConditions_Equals()
    {
        const string jsonConditions = """
                             {
                                 "name": "Monde",
                                 "label": "Capitales d'Europe",
                                 "type": "quiz",
                                 "conditions": {
                                   "operator": "Or",
                                   "items": [
                                     { "field": "continent", "value": "Europe", "rule": "Equals" }
                                   ]
                                 }
                             }
                             """;

        var nodeConditions = JsonSerializer.Deserialize<Node>(jsonConditions);
        Assert.That(nodeConditions, Is.Not.Null);
        Assert.That(nodeConditions.Conditions, Is.Not.Null);

        var nodes = JsonSerializer.Deserialize<List<Node>>(JsonCapitals);
        Assert.That(nodes, Is.Not.Null);
        Assert.That(nodes, Has.Count.EqualTo(5));

        var ruleEngineService = new RuleEngineService();

        var nodesFiltered = nodes.Where(n => ruleEngineService.EvaluateConditions(nodeConditions.Conditions, n)).ToList();
        Assert.That(nodesFiltered, Is.Not.Null);
        Assert.That(nodesFiltered, Has.Count.EqualTo(2));
        Assert.That(nodesFiltered.Select(n => n.Question), 
            Is.EquivalentTo(new[] { "Albanie", "Allemagne" }));
        
    }
    
    [Test]
    public void EvaluateConditions_Contains()
    {
        const string jsonConditions = """
                                      {
                                          "name": "Monde",
                                          "label": "Capitales d'Europe",
                                          "type": "quiz",
                                          "conditions": {
                                            "operator": "Or",
                                            "items": [
                                              { "field": "question", "value": "ie", "rule": "Contains" }
                                            ]
                                          }
                                      }
                                      """;

        var nodeConditions = JsonSerializer.Deserialize<Node>(jsonConditions);
        Assert.That(nodeConditions, Is.Not.Null);
        Assert.That(nodeConditions.Conditions, Is.Not.Null);

        var nodes = JsonSerializer.Deserialize<List<Node>>(JsonCapitals);
        Assert.That(nodes, Is.Not.Null);
        Assert.That(nodes, Has.Count.EqualTo(5));

        var ruleEngineService = new RuleEngineService();

        var nodesFiltered = nodes.Where(n => ruleEngineService.EvaluateConditions(nodeConditions.Conditions, n)).ToList();
        Assert.That(nodesFiltered, Is.Not.Null);
        Assert.That(nodesFiltered, Has.Count.EqualTo(2));
        Assert.That(nodesFiltered.Select(n => n.Question), 
            Is.EquivalentTo(new[] { "Albanie", "Algérie" }));
    }
    
    [Test]
    public void EvaluateConditions_Complex()
    {
        const string jsonConditions = """
                                      {
                                          "name": "Monde",
                                          "label": "Capitales d'Europe",
                                          "type": "quiz",
                                          "conditions": {
                                            "operator": "And",
                                            "items": [
                                              { "field": "question", "value": "l", "rule": "Contains" },
                                              { "field": "answer", "value": "B", "rule": "StartsWith" }
                                            ]
                                          }
                                      }
                                      """;

        var nodeConditions = JsonSerializer.Deserialize<Node>(jsonConditions);
        Assert.That(nodeConditions, Is.Not.Null);
        Assert.That(nodeConditions.Conditions, Is.Not.Null);

        var nodes = JsonSerializer.Deserialize<List<Node>>(JsonCapitals);
        Assert.That(nodes, Is.Not.Null);
        Assert.That(nodes, Has.Count.EqualTo(5));

        var ruleEngineService = new RuleEngineService();

        var nodesFiltered = nodes.Where(n => ruleEngineService.EvaluateConditions(nodeConditions.Conditions, n)).ToList();
        Assert.That(nodesFiltered, Is.Not.Null);
        Assert.That(nodesFiltered, Has.Count.EqualTo(1));
        Assert.That(nodesFiltered.Select(n => n.Question), 
            Is.EquivalentTo(new[] { "Allemagne" }));
    }
}