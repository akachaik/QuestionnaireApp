using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using QuestionnaireApp.Controllers;
using QuestionnaireApp.Models;

namespace QuestionnaireApp.Services
{
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

        private bool ValidateQuestionChoices(string answerAnswerText, string invalidChoices)
        {
            if (string.IsNullOrEmpty(invalidChoices))
            {
                return true;
            }

            if (invalidChoices.Split(',').Contains(answerAnswerText))
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
                out _);
        }

        private bool ValidateQuestionText(string answerAnswerText)
        {
            return !string.IsNullOrEmpty(answerAnswerText);
        }
    }
}