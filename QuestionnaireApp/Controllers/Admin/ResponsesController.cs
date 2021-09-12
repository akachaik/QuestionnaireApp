using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionnaireApp.Data;
using QuestionnaireApp.Models;

namespace QuestionnaireApp.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ResponsesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ResponsesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet("Download")]
        public async Task<FileResult> Download(string userId)
        {
            var data = await _appDbContext.Responses
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.QuestionId)
                .ToListAsync();
            
            var bytes = Array.Empty<byte>();

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture))
                {
                    await csvWriter.WriteRecordsAsync<Response>(data);
                } // StreamWriter gets flushed here.

                bytes = memoryStream.ToArray();
            }

            return File(bytes, "application/octet-stream", $"response-user-{data?.FirstOrDefault()?.UserId}.csv");
        }
    }
}