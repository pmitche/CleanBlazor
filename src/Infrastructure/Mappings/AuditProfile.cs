﻿using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Responses.Audit;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Audit;

namespace BlazorHero.CleanArchitecture.Infrastructure.Mappings;

public class AuditProfile : Profile
{
    public AuditProfile() => CreateMap<AuditResponse, Audit>().ReverseMap();
}
