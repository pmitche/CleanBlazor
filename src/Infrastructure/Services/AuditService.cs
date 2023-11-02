using System.Globalization;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Contracts.Audit;
using BlazorHero.CleanArchitecture.Infrastructure.Data;
using BlazorHero.CleanArchitecture.Infrastructure.Models.Audit;
using BlazorHero.CleanArchitecture.Infrastructure.Specifications;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<AuditService> _localizer;
    private readonly IMapper _mapper;

    public AuditService(
        IMapper mapper,
        ApplicationDbContext context,
        IExcelService excelService,
        IStringLocalizer<AuditService> localizer)
    {
        _mapper = mapper;
        _context = context;
        _excelService = excelService;
        _localizer = localizer;
    }

    public async Task<Result<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId)
    {
        List<Audit> trails = await _context.AuditTrails
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Id)
            .Take(250)
            .ToListAsync();
        var mappedLogs = _mapper.Map<List<AuditResponse>>(trails);
        return mappedLogs;
    }

    public async Task<Result<string>> ExportToExcelAsync(
        string userId,
        string searchString = "",
        bool searchInOldValues = false,
        bool searchInNewValues = false)
    {
        var auditSpec = new AuditFilterSpecification(userId, searchString, searchInOldValues, searchInNewValues);
        List<Audit> trails = await _context.AuditTrails
            .Specify(auditSpec)
            .OrderByDescending(a => a.DateTime)
            .ToListAsync();
        var data = await _excelService.ExportAsync(trails,
            sheetName: _localizer["Audit trails"],
            mappers: new Dictionary<string, Func<Audit, object>>
            {
                { _localizer["Table Name"], item => item.TableName },
                { _localizer["Type"], item => item.Type },
                {
                    _localizer["Date Time (Local)"],
                    item => DateTime.SpecifyKind(item.DateTime, DateTimeKind.Utc).ToLocalTime()
                        .ToString("G", CultureInfo.CurrentCulture)
                },
                { _localizer["Date Time (UTC)"], item => item.DateTime.ToString("G", CultureInfo.CurrentCulture) },
                { _localizer["Primary Key"], item => item.PrimaryKey },
                { _localizer["Old Values"], item => item.OldValues },
                { _localizer["New Values"], item => item.NewValues }
            });

        return data;
    }
}
