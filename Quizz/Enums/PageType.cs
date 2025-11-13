using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Quizz.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<PageType>))]
public enum PageType
{ 
    [Description("A menu where the user has to make a choice."+
                 "Mean also that the node has childrens.")]
    Menu,
    [Description("Quiz is composed of questions with answers.")]
    Quiz,
    [Description("Usually the child of Quizz, question that need to be answer by the user.")]
    Question,
    [Description("Question that need more that one response")]
    QuestionMultipleResponses
}