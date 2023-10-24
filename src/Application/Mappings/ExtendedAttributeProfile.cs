﻿using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Entities.ExtendedAttributes;

namespace BlazorHero.CleanArchitecture.Application.Mappings;

public class ExtendedAttributeProfile : Profile
{
    public ExtendedAttributeProfile()
    {
        CreateMap(typeof(AddEditExtendedAttributeCommand<,,,>), typeof(DocumentExtendedAttribute))
            .ForMember(nameof(DocumentExtendedAttribute.Entity), opt => opt.Ignore())
            .ForMember(nameof(DocumentExtendedAttribute.CreatedBy), opt => opt.Ignore())
            .ForMember(nameof(DocumentExtendedAttribute.CreatedOn), opt => opt.Ignore())
            .ForMember(nameof(DocumentExtendedAttribute.LastModifiedBy), opt => opt.Ignore())
            .ForMember(nameof(DocumentExtendedAttribute.LastModifiedOn), opt => opt.Ignore());

        CreateMap(typeof(GetExtendedAttributeByIdResponse<,>), typeof(DocumentExtendedAttribute)).ReverseMap();
        CreateMap(typeof(GetAllExtendedAttributesResponse<,>), typeof(DocumentExtendedAttribute)).ReverseMap();
        CreateMap(typeof(GetAllExtendedAttributesByEntityIdResponse<,>), typeof(DocumentExtendedAttribute))
            .ReverseMap();
    }
}
