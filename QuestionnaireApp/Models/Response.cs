using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionnaireApp.Models
{
    public class Response
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        
        public int QuestionId { get; set; }

        public string AnswerText { get; set; }
    }
}
