using CleanBlazor.Application.Features.Dashboards.Queries.GetData;

namespace CleanBlazor.Contracts.Dashboard
{
    public class DashboardDataResponse
    {
        public int ProductCount { get; set; }
        public int BrandCount { get; set; }
        public int DocumentCount { get; set; }
        public int DocumentTypeCount { get; set; }
        public int UserCount { get; set; }
        public int RoleCount { get; set; }
        public List<ChartSeries> DataEnterBarChart { get; set; } = new();
        public Dictionary<string, double> ProductByBrandTypePieChart { get; set; }
    }
}

namespace CleanBlazor.Application.Features.Dashboards.Queries.GetData
{
    public class ChartSeries
    {
        public string Name { get; set; }
        public double[] Data { get; set; }
    }
}
