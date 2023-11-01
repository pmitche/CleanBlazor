﻿using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Dashboard;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using ChartSeries = MudBlazor.ChartSeries;

namespace BlazorHero.CleanArchitecture.Client.Pages.Content;

public partial class Dashboard
{
    private readonly List<ChartSeries> _dataEnterBarChartSeries = new();

    private readonly string[] _dataEnterBarChartXAxisLabels =
    {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    private bool _loaded;

    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [Parameter] public int ProductCount { get; set; }
    [Parameter] public int BrandCount { get; set; }
    [Parameter] public int DocumentCount { get; set; }
    [Parameter] public int DocumentTypeCount { get; set; }
    [Parameter] public int UserCount { get; set; }
    [Parameter] public int RoleCount { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        _loaded = true;
        HubConnection = HubConnection.TryInitialize(NavigationManager, LocalStorage);
        HubConnection.On(ApplicationConstants.SignalR.ReceiveUpdateDashboard,
            async () =>
            {
                await LoadDataAsync();
                StateHasChanged();
            });
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }

    private async Task LoadDataAsync()
    {
        var result = await HttpClient.GetFromJsonAsync<Result<DashboardDataResponse>>(DashboardEndpoints.GetData);
        result.HandleWithSnackBar(SnackBar, (_, dashboard) =>
        {
            ProductCount = dashboard.ProductCount;
            BrandCount = dashboard.BrandCount;
            DocumentCount = dashboard.DocumentCount;
            DocumentTypeCount = dashboard.DocumentTypeCount;
            UserCount = dashboard.UserCount;
            RoleCount = dashboard.RoleCount;
            foreach (Application.Features.Dashboards.Queries.GetData.ChartSeries item in dashboard.DataEnterBarChart)
            {
                _dataEnterBarChartSeries
                    .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                _dataEnterBarChartSeries.Add(new ChartSeries { Name = item.Name, Data = item.Data });
            }
        });
    }
}
