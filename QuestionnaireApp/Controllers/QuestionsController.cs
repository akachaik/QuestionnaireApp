using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using QuestionnaireApp.Services;

namespace QuestionnaireApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IQuestionValidator _questionValidator;
        private readonly List<Question> _questions;
        
        public QuestionsController(ISessionService sessionService, IQuestionValidator questionValidator)
        {
            _sessionService = sessionService;
            _questionValidator = questionValidator;

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

            var currentQuestionId = _sessionService.GetCurrentQuestionId();

            return Ok(_questions.FirstOrDefault(q => q.Id == currentQuestionId));
        }

        [HttpPost]
        public ActionResult Post(Answer answer)
        {

            var currentQuestionId = _sessionService.GetCurrentQuestionId();

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

            if (!_questionValidator.Validate(question, answer))
            {
                return BadRequest("Answer is invalid");
            }

            // Save answer to db


            // Increment current question id
            var nextQuestionId = currentQuestionId + 1;
            if (nextQuestionId > _questions.Max(q => q.Id))
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

        public List<string> InvalidChoices { get; set; }
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
}