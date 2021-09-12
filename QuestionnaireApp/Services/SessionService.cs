using System;
using Microsoft.AspNetCore.Http;

namespace QuestionnaireApp.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentQuestionId()
        {
            var currentQuestionId = _httpContextAccessor.HttpContext?.Session.GetString("currentQuestionId");
            if (string.IsNullOrEmpty(currentQuestionId))
            {
                _httpContextAccessor.HttpContext?.Session.SetString("currentQuestionId", "1");
                currentQuestionId = "1";
            }

            return int.Parse(currentQuestionId);
        }

        public string GetCurrentUserId()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.Session.GetString("currentUserId");
            if (string.IsNullOrEmpty(currentUserId))
            {
                var newUserId = Guid.NewGuid().ToString();
                _httpContextAccessor.HttpContext?.Session.SetString("currentUserId", newUserId);
                currentUserId = newUserId;
            }

            return currentUserId;
        }

        public void SetCurrentQuestionId(int currentQuestionId)
        {
            _httpContextAccessor.HttpContext?.Session.SetString("currentQuestionId", currentQuestionId.ToString());
        }
    }
}