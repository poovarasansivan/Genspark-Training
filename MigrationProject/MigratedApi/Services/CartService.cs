using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Models;

namespace MigratedApi.Services
{
    public class CartService : ICartService
    {
        private readonly CartRepository _cartRepository;
        private readonly ShopDbContext _context;

        public CartService(CartRepository cartRepository, ShopDbContext context)
        {
            _cartRepository = cartRepository;
            _context = context;
        }

        public async Task<CartResponseDto> AddToCartAsync(CartRequestDto cartDto)
        {
            if (cartDto == null)
                throw new ArgumentNullException(nameof(CartRequestDto));

            if (cartDto.UserId <= 0 || cartDto.ProductId <= 0 || cartDto.Quantity <= 0)
                throw new ArgumentException("Invalid Cart Request Data");

            var newCart = new Cart
            {
                UserId = cartDto.UserId,
                ProductId = cartDto.ProductId,
                Quantity = cartDto.Quantity,
                Price = cartDto.Price,
                CreatedDate = cartDto.CreatedDate
            };

            await _cartRepository.AddAsync(newCart);
            await _context.SaveChangesAsync();

            return new CartResponseDto
            {
                Id = newCart.Id,
                UserId = newCart.UserId,
                ProductId = newCart.ProductId,
                Quantity = newCart.Quantity,
                Price = newCart.Price,
                CreatedDate = newCart.CreatedDate
            };
        }

        public async Task<IEnumerable<CartResponseDto>> GetAllCartsAsync()
        {
            var carts = await _cartRepository.GetAllAsync();
            return carts.Select(c => new CartResponseDto
            {
                Id = c.Id,
                UserId = c.UserId,
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Price = c.Price,
                CreatedDate = c.CreatedDate
            });
        }

        public async Task<CartResponseDto> GetCartByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Cart ID");
            var cart = await _cartRepository.GetByIdAsync(id);
            if (cart == null)
                throw new KeyNotFoundException($"Cart with ID {id} not found");
            return new CartResponseDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                ProductId = cart.ProductId,
                Quantity = cart.Quantity,
                Price = cart.Price,
                CreatedDate = cart.CreatedDate
            };
        }

        public async Task<double> GetTotalPriceAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid User ID");

            var carts = await _cartRepository.GetByUserIdAsync(userId);
            if (carts == null || !carts.Any())
                throw new KeyNotFoundException($"No carts found for User ID {userId}");

            return carts.Sum(c => c.Price * c.Quantity);
        }

        public async Task<IEnumerable<CartResponseDto>> GetCartsByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid User ID");

            var carts = await _cartRepository.GetByUserIdAsync(userId);
            return carts.Select(c => new CartResponseDto
            {
                Id = c.Id,
                UserId = c.UserId,
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Price = c.Price,
                CreatedDate = c.CreatedDate
            });
        }
        public async Task<bool> RemoveFromCartAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Cart ID");

            var cart = await _cartRepository.GetByIdAsync(id);
            if (cart == null)
                throw new KeyNotFoundException($"Cart with ID {id} not found");

            await _cartRepository.DeleteAsync(cart.Id);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CartResponseDto> UpdateCartAsync(int id, CartRequestDto cartDto)
        {

            if (id <= 0)
                throw new ArgumentException("Invalid Cart ID");

            if (cartDto == null)
                throw new ArgumentNullException(nameof(CartRequestDto));

            var cart = await _cartRepository.GetByIdAsync(id);
            if (cart == null)
                throw new KeyNotFoundException($"Cart with ID {id} not found");

            cart.UserId = cartDto.UserId;
            cart.ProductId = cartDto.ProductId;
            cart.Quantity = cartDto.Quantity;
            cart.Price = cartDto.Price;
            cart.CreatedDate = cartDto.CreatedDate;

            await _cartRepository.UpdateAsync(cart);
            await _context.SaveChangesAsync();

            return new CartResponseDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                ProductId = cart.ProductId,
                Quantity = cart.Quantity,
                Price = cart.Price,
                CreatedDate = cart.CreatedDate
            };
        }

        public async Task<IEnumerable<CartResponseDto>> GetCartByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid User ID");

            var cartItem = await _cartRepository.GetByUserIdAsync(userId);
            return cartItem.Select(c => new CartResponseDto
            {
                Id = c.Id,
                UserId = c.UserId,
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Price = c.Price,
                CreatedDate = c.CreatedDate
            });
        }
    }
}