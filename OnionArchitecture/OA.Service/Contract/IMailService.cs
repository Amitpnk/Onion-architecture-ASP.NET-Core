using OA.Domain;
using System.Threading.Tasks;

namespace OA.Service.Contract
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);

    }
}
