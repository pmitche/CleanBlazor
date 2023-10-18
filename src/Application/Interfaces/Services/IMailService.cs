using BlazorHero.CleanArchitecture.Application.Requests.Mail;

namespace BlazorHero.CleanArchitecture.Application.Interfaces.Services;

public interface IMailService
{
    Task SendAsync(MailRequest request);
}
