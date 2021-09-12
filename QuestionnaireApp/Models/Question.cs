using System.Collections.Generic;
using QuestionnaireApp.Controllers;

namespace QuestionnaireApp.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string Choices { get; set; }
        public QuestionTypes Type { get; set; }
        public string InvalidChoices { get; set; }
    }
}