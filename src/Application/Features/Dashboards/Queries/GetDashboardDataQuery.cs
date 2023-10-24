using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services.Identity;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Application.Features.Dashboards.Queries.GetData;
using BlazorHero.CleanArchitecture.Contracts.Dashboard;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Dashboards.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetDashboardDataQuery : IQuery<Result<DashboardDataResponse>>;

internal sealed class GetDashboardDataQueryHandler : IQueryHandler<GetDashboardDataQuery, Result<DashboardDataResponse>>
{
    private readonly IStringLocalizer<GetDashboardDataQueryHandler> _localizer;
    private readonly IRoleService _roleService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUserService _userService;

    public GetDashboardDataQueryHandler(
        IUnitOfWork<int> unitOfWork,
        IUserService userService,
        IRoleService roleService,
        IStringLocalizer<GetDashboardDataQueryHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _roleService = roleService;
        _localizer = localizer;
    }

    public async Task<Result<DashboardDataResponse>> Handle(
        GetDashboardDataQuery query,
        CancellationToken cancellationToken)
    {
        DashboardDataResponse response = new()
        {
            ProductCount = await _unitOfWork.Repository<Product>().Entities.CountAsync(cancellationToken),
            BrandCount = await _unitOfWork.Repository<Brand>().Entities.CountAsync(cancellationToken),
            DocumentCount = await _unitOfWork.Repository<Document>().Entities.CountAsync(cancellationToken),
            DocumentTypeCount = await _unitOfWork.Repository<DocumentType>().Entities.CountAsync(cancellationToken),
            UserCount = await _userService.GetCountAsync(),
            RoleCount = await _roleService.GetCountAsync()
        };

        var selectedYear = DateTime.Now.Year;
        var productsFigure = new double[13];
        var brandsFigure = new double[13];
        var documentsFigure = new double[13];
        var documentTypesFigure = new double[13];
        for (var month = 1; month <= 12; month++)
        {
            DateTime filterStartDate = new(selectedYear, month, 01, 0, 0, 0, DateTimeKind.Utc);
            DateTime filterEndDate =
                new(selectedYear,
                    month,
                    DateTime.DaysInMonth(selectedYear, month),
                    23,
                    59,
                    59,
                    DateTimeKind.Utc); // Monthly Based

            productsFigure[month - 1] = await _unitOfWork.Repository<Product>().Entities
                .Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate)
                .CountAsync(cancellationToken);
            brandsFigure[month - 1] = await _unitOfWork.Repository<Brand>().Entities
                .Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate)
                .CountAsync(cancellationToken);
            documentsFigure[month - 1] = await _unitOfWork.Repository<Document>().Entities
                .Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate)
                .CountAsync(cancellationToken);
            documentTypesFigure[month - 1] = await _unitOfWork.Repository<DocumentType>().Entities
                .Where(x => x.CreatedOn >= filterStartDate && x.CreatedOn <= filterEndDate)
                .CountAsync(cancellationToken);
        }

        response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Products"], Data = productsFigure });
        response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Brands"], Data = brandsFigure });
        response.DataEnterBarChart.Add(new ChartSeries { Name = _localizer["Documents"], Data = documentsFigure });
        response.DataEnterBarChart.Add(new ChartSeries
        {
            Name = _localizer["Document Types"], Data = documentTypesFigure
        });

        return await Result<DashboardDataResponse>.SuccessAsync(response);
    }
}
