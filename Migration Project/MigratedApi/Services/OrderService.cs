using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MigratedApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly ShopDbContext _context;

        public OrderService(OrderRepository orderRepository, ShopDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<List<OrderResponseDto>> CreateOrderAsync(OrderRequestDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));
            if (orderDto.UserId <= 0 || orderDto.Products == null || !orderDto.Products.Any())
                throw new ArgumentException("Invalid Order Request Data");

            var orders = new List<Orders>();

            foreach (var item in orderDto.Products)
            {
                if (item.ProductId <= 0 || item.Quantity <= 0)
                    throw new ArgumentException("Invalid product data in order");

                var newOrder = new Orders
                {
                    UserId = orderDto.UserId,
                    ProductId = item.ProductId,
                    Status = orderDto.Status,
                    Address = orderDto.Address,
                    PhoneNumber = orderDto.PhoneNumber,
                    PaymentStatus = orderDto.PaymentStatus,
                    PaymentMethod = orderDto.PaymentMethod,
                    OrderDate = orderDto.OrderDate,
                    Quantity = item.Quantity,
                    TotalAmount = item.TotalAmount > 0 ? item.TotalAmount : 0
                };

                orders.Add(newOrder);
            }

            await _orderRepository.AddRangeAsync(orders);
            await _context.SaveChangesAsync();

            return orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                ProductId = order.ProductId,
                Status = order.Status,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate,
                Quantity = order.Quantity,
                TotalAmount = order.TotalAmount
            }).ToList();
        }


        public async Task<bool> DeleteOrderAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Order ID");
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found");
            await _orderRepository.DeleteAsync(order.Id);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                UserId = o.UserId,
                ProductId = o.ProductId,
                Status = o.Status,
                Address = o.Address,
                PhoneNumber = o.PhoneNumber,
                PaymentStatus = o.PaymentStatus,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.OrderDate,
                Quantity = o.Quantity,
                TotalAmount = o.TotalAmount
            });
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Order ID");
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found");

            return new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                ProductId = order.ProductId,
                Status = order.Status,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                OrderDate = order.OrderDate,
                Quantity = order.Quantity,
                TotalAmount = order.TotalAmount
            };
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Status cannot be null or empty");

            var orders = await _orderRepository.GetByStatusAsync(status);
            return orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                UserId = o.UserId,
                ProductId = o.ProductId,
                Status = o.Status,
                Address = o.Address,
                PhoneNumber = o.PhoneNumber,
                PaymentStatus = o.PaymentStatus,
                PaymentMethod = o.PaymentMethod,
                OrderDate = o.OrderDate,
                Quantity = o.Quantity,
                TotalAmount = o.TotalAmount
            });
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid User ID");
            var orders = await _context.Orders
                        .Include(o => o.User)
                        .Include(o => o.Product)
                            .ThenInclude(p => p.Category)
                        .Include(o => o.Product)
                            .ThenInclude(p => p.Color)
                        .Include(o => o.Product)
                            .ThenInclude(p => p.Model)
                        .Where(o => o.UserId == userId)
                        .Select(o => new OrderResponseDto
                        {
                            Id = o.Id,
                            UserId = o.UserId,
                            UserName = o.User.Username,
                            ProductId = o.ProductId,
                            ProductName = o.Product.ProductName,
                            ProductImage = o.Product.Image,
                            Status = o.Status,
                            Address = o.Address,
                            PhoneNumber = o.PhoneNumber,
                            PaymentStatus = o.PaymentStatus,
                            PaymentMethod = o.PaymentMethod,
                            OrderDate = o.OrderDate,
                            Quantity = o.Quantity,
                            TotalAmount = o.TotalAmount
                        })
                        .ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<OrdersResponseDto>> GetPaginatedOrdersAsync(OrdersPaginationDto paginationDto)
        {
            if (paginationDto == null)
                throw new ArgumentException("All Input Parameters cannot be null");
            if (paginationDto.PageNumber <= 0 || paginationDto.PageSize <= 0)
                throw new ArgumentException("Page number and size must be greater than zero");
            var query = _context.Orders
                        .Include(o => o.User)
                        .Include(o => o.Product)
                            .ThenInclude(p => p.Category)
                        .Include(o => o.Product)
                            .ThenInclude(p => p.Color)
                        .Include(o => o.Product)
                            .ThenInclude(p => p.Model)
                        .AsQueryable();

            if (!string.IsNullOrEmpty(paginationDto.SearchTerm))
            {
                query = query.Where(o => o.User.Username.Contains(paginationDto.SearchTerm) ||
                                         o.Product.ProductName.Contains(paginationDto.SearchTerm) ||
                                         o.Status.Contains(paginationDto.SearchTerm) ||
                                         o.Address.Contains(paginationDto.SearchTerm) ||
                                         o.PhoneNumber.Contains(paginationDto.SearchTerm));
            }
            if (!string.IsNullOrEmpty(paginationDto.Status))
            {
                query = query.Where(o => o.Status == paginationDto.Status);
            }
            if (!string.IsNullOrEmpty(paginationDto.UserName))
            {
                query = query.Where(o => o.User.Username == paginationDto.UserName);
            }
            if (!string.IsNullOrEmpty(paginationDto.SortBy))
            {
                if (paginationDto.SortDirection?.ToLower() == "desc")
                {
                    query = query.OrderByDescending(o => EF.Property<object>(o, paginationDto.SortBy));
                }
                else
                {
                    query = query.OrderBy(o => EF.Property<object>(o, paginationDto.SortBy));
                }
            }

            var orders = await query
                .Skip((paginationDto.PageNumber - 1) * paginationDto.PageSize)
                .Take(paginationDto.PageSize)
                .Select(o => new OrdersResponseDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    UserName = o.User.Username,
                    ProductId = o.ProductId,
                    ProductName = o.Product.ProductName,
                    Image = o.Product.Image,
                    Price = o.Product.Price,
                    CategoryId = o.Product.Category.CategoryId,
                    CategoryName = o.Product.Category.Name,
                    ColorId = o.Product.Color.ColorId,
                    ColorName = o.Product.Color.ColorName,
                    ModelId = o.Product.Model.ModelId,
                    ModelName = o.Product.Model.ModelName,
                    Status = o.Status,
                    Address = o.Address,
                    PhoneNumber = o.PhoneNumber,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    OrderDate = o.OrderDate,
                    Quantity = o.Quantity,
                    TotalAmount = o.TotalAmount
                })
                .ToListAsync();
            return orders;
        }

        public async Task<bool> UpdateOrderAsync(int id, OrderRequestDto orderDto)
        {
            throw new NotImplementedException("UpdateOrderAsync method is not implemented yet.");
        }
    }
}