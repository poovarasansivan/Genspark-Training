using MigratedApi.Models;
using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task AddProductAsync(ProductRequestDto product);
        Task UpdateProductAsync(ProductRequestDto product);
        Task DeleteProductAsync(int id);
    }
}