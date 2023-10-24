using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Dashboard;
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
    [Inject] private IDashboardManager DashboardManager { get; set; }

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
        IResult<DashboardDataResponse> response = await DashboardManager.GetDataAsync();
        if (response.Succeeded)
        {
            ProductCount = response.Data.ProductCount;
            BrandCount = response.Data.BrandCount;
            DocumentCount = response.Data.DocumentCount;
            DocumentTypeCount = response.Data.DocumentTypeCount;
            UserCount = response.Data.UserCount;
            RoleCount = response.Data.RoleCount;
            foreach (Application.Features.Dashboards.Queries.GetData.ChartSeries item in
                     response.Data.DataEnterBarChart)
            {
                _dataEnterBarChartSeries
                    .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                _dataEnterBarChartSeries.Add(new ChartSeries { Name = item.Name, Data = item.Data });
            }
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
    }
}
