using OA.Domain.Settings;
using OA.Service.Contract;
using System.Collections.Concurrent;

namespace OA.Test.Integration;

/// <summary>
/// Replaces the SMTP-backed mail service so tests never send real email,
/// while still letting them inspect what would have been sent.
/// </summary>
public class FakeEmailService : IEmailService
{
    public ConcurrentQueue<MailRequest> SentMails { get; } = new();

    public Task SendEmailAsync(MailRequest mailRequest)
    {
        SentMails.Enqueue(mailRequest);
        return Task.CompletedTask;
    }
}
