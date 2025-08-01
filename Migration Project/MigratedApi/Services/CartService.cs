using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Models;
using Microsoft.EntityFrameworkCore;


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

        public async Task<List<CartResponseDto>> AddToCartAsync(List<CartRequestDto> cartDtos)
        {
            if (cartDtos == null || !cartDtos.Any())
                throw new ArgumentNullException(nameof(cartDtos), "Cart request list cannot be null or empty.");

            var responseList = new List<CartResponseDto>();

            foreach (var cartDto in cartDtos)
            {
                if (cartDto.UserId <= 0 || cartDto.ProductId <= 0 || cartDto.Quantity <= 0)
                    continue; 

                var newCart = new Cart
                {
                    UserId = cartDto.UserId,
                    ProductId = cartDto.ProductId,
                    Quantity = cartDto.Quantity,
                    Price = cartDto.Price,
                    CreatedDate = cartDto.CreatedDate
                };

                await _cartRepository.AddAsync(newCart);

                responseList.Add(new CartResponseDto
                {
                    Id = newCart.Id,
                    UserId = newCart.UserId,
                    ProductId = newCart.ProductId,
                    Quantity = newCart.Quantity,
                    Price = newCart.Price,
                    CreatedDate = newCart.CreatedDate
                });
            }
            await _context.SaveChangesAsync();
            return responseList;
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

            var carts = await _context.Carts
                        .Include(c => c.User)
                        .Include(c => c.Product)
                            .ThenInclude(p => p.Category)
                        .Include(c => c.Product)
                            .ThenInclude(p => p.Color)
                        .Include(c => c.Product)
                            .ThenInclude(p => p.Model)
                        .Where(c => c.UserId == userId)
                        .Select(c => new CartResponseDto
                        {
                            Id = c.Id,
                            UserId = c.UserId,
                            Username = c.User != null ? c.User.Username : null,
                            ProductId = c.ProductId,
                            ProductName = c.Product != null ? c.Product.ProductName : null,
                            Image = c.Product != null ? c.Product.Image : null,
                            Price = c.Product != null ? c.Product.Price : 0,
                            CategoryId = c.Product != null ? c.Product.CategoryId : 0,
                            CategoryName = c.Product != null && c.Product.Category != null ? c.Product.Category.Name : null,
                            ColorId = c.Product != null ? c.Product.ColorId : 0,
                            ColorName = c.Product != null && c.Product.Color != null ? c.Product.Color.ColorName : null,
                            ModelId = c.Product != null ? c.Product.ModelId : 0,
                            ModelName = c.Product != null && c.Product.Model != null ? c.Product.Model.ModelName : null,
                            Quantity = c.Quantity,
                            CreatedDate = c.CreatedDate,
                        })
                        .ToListAsync();

            return carts;
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