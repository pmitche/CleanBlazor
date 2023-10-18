namespace BlazorHero.CleanArchitecture.Application.Responses.Identity;

public class ChatHistoryResponse
{
    public long Id { get; set; }
    public string FromUserId { get; set; }
    public string FromUserImageUrl { get; set; }
    public string FromUserFullName { get; set; }
    public string ToUserId { get; set; }
    public string ToUserImageUrl { get; set; }
    public string ToUserFullName { get; set; }
    public string Message { get; set; }
    public DateTime CreatedDate { get; set; }
}
