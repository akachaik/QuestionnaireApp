using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    public interface IQuestionValidator
    {
        bool Validate(Question question, Answer answer);
    }

    public class QuestionValidator : IQuestionValidator
    {
        public bool Validate(Question question, Answer answer)
        {
            switch (question.Type)
            {
                case QuestionTypes.Text:
                    return ValidateQuestionText(answer.AnswerText);
                case QuestionTypes.Date:
                    return ValidateQuestionDate(answer.AnswerText);
                case QuestionTypes.Choices:
                    return ValidateQuestionChoices(answer.AnswerText, question.InvalidChoices);
                default:
                    throw new Exception("Invalid question type");
            }
        }

        private bool ValidateQuestionChoices(string answerAnswerText, List<string> invalidChoices)
        {
            if (invalidChoices.Contains(answerAnswerText))
            {
                return false;
            }

            return true;
        }

        private bool ValidateQuestionDate(string answerAnswerText)
        {
            var format = "yyyy-MM-dd";
            return DateTime.TryParseExact(answerAnswerText, 
                format, 
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, 
                out var dateTime2);
        }

        private bool ValidateQuestionText(string answerAnswerText)
        {
            return !string.IsNullOrEmpty(answerAnswerText);
        }
    }
}