using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuestionnaireApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionService _sessionService;
        private readonly List<Question> _questions;
        
        public QuestionsController(IHttpContextAccessor httpContextAccessor, ISessionService sessionService)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;

            _questions = new List<Question>()
            {
                new() { Id = 1, QuestionText = "Title (Mr. Ms. or Mrs.)", Type = QuestionTypes.Choices, Choices = new List<string> { "Mr.", "Ms.", "Mrs."}},
                new() { Id = 2, QuestionText = "First name", Type = QuestionTypes.Text},
                new() { Id = 3, QuestionText = "Last name", Type = QuestionTypes.Text},
                new() { Id = 4, QuestionText = "Date of birth", Type = QuestionTypes.Date},
            };
        }

        [HttpGet]
        public ActionResult<Question> Get()
        {
            //var currentQuestionId = _httpContextAccessor.HttpContext?.Session.GetString("currentQuestionId");
            //if (string.IsNullOrEmpty(currentQuestionId))
            //{
            //    _httpContextAccessor.HttpContext?.Session.SetString("currentQuestionId", "1");
            //    currentQuestionId = "1";
            //}

            var currentQuestionId = _sessionService.GetCurrentQuestionId();

            return Ok(_questions.FirstOrDefault(q => q.Id == currentQuestionId));
        }

        [HttpPost]
        public ActionResult Post(Answer answer)
        {

            // var currentQuestionId = _httpContextAccessor.HttpContext?.Session.GetString("currentQuestionId");

            var currentQuestionId = _sessionService.GetCurrentQuestionId();

            //if (!int.TryParse(currentQuestionId, out int currentQuestionIdValue))
            //{
            //    return BadRequest($"Invalid current question id: {currentQuestionId}");
            //}

            if (currentQuestionId != answer.QuestionId)
            {
                return BadRequest($"Invalid question id: {answer.QuestionId}");
            }

            // validate
            var question = _questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question is null)
            {
                return BadRequest($"Invalid question id: {answer.QuestionId}");
            }

            // validate by question types



            // Save to db


            // Increment current question id
            var nextQuestionId = currentQuestionId + 1;
            if (nextQuestionId > _questions.Count)
            {
                nextQuestionId = 1;
            }

            // _httpContextAccessor.HttpContext?.Session.SetString("currentQuestionId", nextQuestionId.ToString());
            _sessionService.SetCurrentQuestionId(nextQuestionId);

            return Ok();
        }
    }

    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<string> Choices { get; set; }
        public QuestionTypes Type { get; set; }
    }

    public class Answer
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
    }

    public enum QuestionTypes
    {
        Text,
        Choices,
        Date
    }
    public interface ISessionService
    {
        public int GetCurrentQuestionId();
        public void SetCurrentQuestionId(int currentQuestionId);
    }

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

        public void SetCurrentQuestionId(int currentQuestionId)
        {
            _httpContextAccessor.HttpContext?.Session.SetString("currentQuestionId", currentQuestionId.ToString());
        }
    }
}