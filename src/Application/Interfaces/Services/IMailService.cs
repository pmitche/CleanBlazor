using BlazorHero.CleanArchitecture.Contracts.Mail;

namespace BlazorHero.CleanArchitecture.Application.Interfaces.Services;

public interface IMailService
{
    Task SendAsync(MailRequest request);
}
