namespace BlazorHero.CleanArchitecture.Contracts.Documents;

public class GetAllPagedDocumentsRequest : PagedRequest
{
    public string SearchString { get; set; }
}
