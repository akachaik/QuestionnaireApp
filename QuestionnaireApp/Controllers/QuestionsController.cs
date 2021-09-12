using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionnaireApp.Data;
using QuestionnaireApp.Models;
using QuestionnaireApp.Services;

namespace QuestionnaireApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IQuestionValidator _questionValidator;
        private readonly AppDbContext _appDbContext;
        
        public QuestionsController(ISessionService sessionService, IQuestionValidator questionValidator, AppDbContext appDbContext)
        {
            _sessionService = sessionService;
            _questionValidator = questionValidator;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<Question>> Get()
        {

            var currentQuestionId = _sessionService.GetCurrentQuestionId();
            var questions = await _appDbContext.Questions.ToListAsync();

            return Ok(questions.FirstOrDefault(q => q.Id == currentQuestionId));
        }

        [HttpPost]
        public async Task<ActionResult> Post(Answer answer)
        {

            var currentQuestionId = _sessionService.GetCurrentQuestionId();

            if (currentQuestionId != answer.QuestionId)
            {
                return BadRequest($"Invalid question id: {answer.QuestionId}");
            }

            // validate
            var questions = await _appDbContext.Questions.ToListAsync();
            var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
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

            var response = new Response()
            {
                UserId = _sessionService.GetCurrentUserId(),
                QuestionId = question.Id,
                AnswerText = answer.AnswerText
            };

            _appDbContext.Responses.Add(response);
            await _appDbContext.SaveChangesAsync();

            // Increment current question id
            var nextQuestionId = currentQuestionId + 1;
            if (nextQuestionId > questions.Max(q => q.Id))
            {
                nextQuestionId = 1;
            }

            // _httpContextAccessor.HttpContext?.Session.SetString("currentQuestionId", nextQuestionId.ToString());
            _sessionService.SetCurrentQuestionId(nextQuestionId);

            return Ok();
        }
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