namespace RVM.MenuNaMao.Application.DTOs;

public record DashboardDto(
    int PendingOrders,
    int PreparingOrders,
    int TodayOrders,
    decimal TodayRevenue,
    List<StockAlertDto> StockAlerts);

public record StockAlertDto(
    Guid StockItemId,
    string Name,
    decimal Quantity,
    decimal MinQuantity);
