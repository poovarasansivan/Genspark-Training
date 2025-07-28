using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Models;

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

        public async Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(OrderRequestDto));
            if (orderDto.UserId <= 0 || orderDto.ProductId <= 0 || orderDto.Quantity <= 0)
                throw new ArgumentException("Invalid Order Request Data");
            var newOrder = new Orders
            {
                UserId = orderDto.UserId,
                ProductId = orderDto.ProductId,
                Status = orderDto.Status,
                Address = orderDto.Address,
                PhoneNumber = orderDto.PhoneNumber,
                PaymentStatus = orderDto.PaymentStatus,
                PaymentMethod = orderDto.PaymentMethod,
                OrderDate = orderDto.OrderDate,
                Quantity = orderDto.Quantity
            };

            await _orderRepository.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            return new OrderResponseDto
            {
                Id = newOrder.Id,
                UserId = newOrder.UserId,
                ProductId = newOrder.ProductId,
                Status = newOrder.Status,
                Address = newOrder.Address,
                PhoneNumber = newOrder.PhoneNumber,
                PaymentStatus = newOrder.PaymentStatus,
                PaymentMethod = newOrder.PaymentMethod,
                OrderDate = newOrder.OrderDate,
                Quantity = newOrder.Quantity
            };
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
                Quantity = o.Quantity
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
                Quantity = order.Quantity
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
                Quantity = o.Quantity
            });
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid User ID");
            var orders = await _orderRepository.GetByUserIdAsync(userId);
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
                Quantity = o.Quantity
            });
        }

        public async Task<bool> UpdateOrderAsync(int id, OrderRequestDto orderDto)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Order ID");
            if (orderDto == null)
                throw new ArgumentNullException(nameof(OrderRequestDto));

            var existingOrder = await _orderRepository.GetByIdAsync(id);
            if (existingOrder == null)
                throw new KeyNotFoundException($"Order with ID {id} not found");

            existingOrder.UserId = orderDto.UserId;
            existingOrder.ProductId = orderDto.ProductId;
            existingOrder.Status = orderDto.Status;
            existingOrder.Address = orderDto.Address;
            existingOrder.PhoneNumber = orderDto.PhoneNumber;
            existingOrder.PaymentStatus = orderDto.PaymentStatus;
            existingOrder.PaymentMethod = orderDto.PaymentMethod;
            existingOrder.OrderDate = orderDto.OrderDate;
            existingOrder.Quantity = orderDto.Quantity;

            await _orderRepository.UpdateAsync(existingOrder);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}