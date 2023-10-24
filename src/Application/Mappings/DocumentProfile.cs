using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.Documents.Commands;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;

namespace BlazorHero.CleanArchitecture.Application.Mappings;

public class DocumentProfile : Profile
{
    public DocumentProfile()
    {
        CreateMap<AddEditDocumentCommand, Document>().ReverseMap();
        CreateMap<GetDocumentByIdResponse, Document>().ReverseMap();
    }
}
