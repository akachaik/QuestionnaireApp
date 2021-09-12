using QuestionnaireApp.Controllers;
using QuestionnaireApp.Models;

namespace QuestionnaireApp.Services
{
    public interface IQuestionValidator
    {
        bool Validate(Question question, Answer answer);
    }
}