using BlazorHero.CleanArchitecture.Contracts.Mail;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;

public interface IMailService
{
    Task SendAsync(MailRequest request);
}
