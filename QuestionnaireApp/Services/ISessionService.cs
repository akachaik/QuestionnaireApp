namespace QuestionnaireApp.Services
{
    public interface ISessionService
    {
        public int GetCurrentQuestionId();
        public void SetCurrentQuestionId(int currentQuestionId);
    }
}