using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.DocumentTypes.Commands;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;

namespace BlazorHero.CleanArchitecture.Application.Configuration.Mappings;

public class DocumentTypeProfile : Profile
{
    public DocumentTypeProfile()
    {
        CreateMap<AddEditDocumentTypeCommand, DocumentType>().ReverseMap();
        CreateMap<GetDocumentTypeByIdResponse, DocumentType>().ReverseMap();
        CreateMap<GetAllDocumentTypesResponse, DocumentType>().ReverseMap();
    }
}
