using BlazorHero.CleanArchitecture.Application.Interfaces.Messaging;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services.Identity;
using BlazorHero.CleanArchitecture.Contracts.Dashboard;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Domain.Entities.ExtendedAttributes;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Dashboards.Queries.GetData;

public record GetDashboardDataQuery : IQuery<Result<DashboardDataResponse>>;

internal class GetDashboardDataQueryHandler : IQueryHandler<GetDashboardDataQuery, Result<DashboardDataResponse>>
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
            DocumentExtendedAttributeCount =
                await _unitOfWork.Repository<DocumentExtendedAttribute>().Entities.CountAsync(cancellationToken),
            UserCount = await _userService.GetCountAsync(),
            RoleCount = await _roleService.GetCountAsync()
        };

        var selectedYear = DateTime.Now.Year;
        var productsFigure = new double[13];
        var brandsFigure = new double[13];
        var documentsFigure = new double[13];
        var documentTypesFigure = new double[13];
        var documentExtendedAttributesFigure = new double[13];
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
            documentExtendedAttributesFigure[month - 1] = await _unitOfWork.Repository<DocumentExtendedAttribute>().Entities
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
        response.DataEnterBarChart.Add(new ChartSeries
        {
            Name = _localizer["Document Extended Attributes"], Data = documentExtendedAttributesFigure
        });

        return await Result<DashboardDataResponse>.SuccessAsync(response);
    }
}
