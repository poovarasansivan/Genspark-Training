using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
        Task<OrderResponseDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderDto);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByStatusAsync(string status);
        Task<bool> UpdateOrderAsync(int id, OrderRequestDto orderDto);
        Task<bool> DeleteOrderAsync(int id);
    }
}