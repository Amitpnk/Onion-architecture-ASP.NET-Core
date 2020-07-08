using Microsoft.AspNetCore.Mvc;
using OA.Domain;
using OA.Service.Contract;
using System.Threading.Tasks;

namespace OA.Controllers
{
    [Route("api/Mail")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService mailService;
        public MailController(IMailService mailService)
        {
            this.mailService = mailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequest request)
        {
            await mailService.SendEmailAsync(request);
            return Ok();
        }

    }
}