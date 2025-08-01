using MigratedApi.Models;
using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartByIdAsync(int id);
        Task<IEnumerable<CartResponseDto>> GetAllCartsAsync();
        Task<List<CartResponseDto>> AddToCartAsync(List<CartRequestDto> cartDto);
        Task<CartResponseDto> UpdateCartAsync(int id, CartRequestDto cartDto);
        Task<bool> RemoveFromCartAsync(int id);
        Task<double> GetTotalPriceAsync(int userId);
        Task<IEnumerable<CartResponseDto>> GetCartsByUserIdAsync(int userId);
    }
}