using QuestionnaireApp.Controllers;

namespace QuestionnaireApp.Services
{
    public interface IQuestionValidator
    {
        bool Validate(Question question, Answer answer);
    }
}