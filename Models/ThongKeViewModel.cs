namespace Lapzone_test.Models;

public class ThongKeViewModel
{
    public int Year { get; set; }
    public List<int> Years { get; set; } = new();

    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalItemsSold { get; set; }
    public decimal AverageOrder { get; set; }

    // 12 phần tử, tương ứng tháng 1 đến 12
    public List<decimal> MonthlyRevenue { get; set; } = new();
    public List<int> MonthlyOrders { get; set; } = new();

    public List<TopProductItem> TopProducts { get; set; } = new();
    public List<Order> RecentOrders { get; set; } = new();
}

public class TopProductItem
{
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
}