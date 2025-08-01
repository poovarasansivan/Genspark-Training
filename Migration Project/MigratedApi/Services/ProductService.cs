using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MigratedApi.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ShopDbContext _context;
        public ProductService(ProductRepository productRepository, ShopDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task AddProductAsync(ProductRequestDto product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(ProductRequestDto));

            if (string.IsNullOrEmpty(product.ProductName) || product.Price <= 0)
                throw new ArgumentException("Product Name cannot be null or empty and Price must be greater than zero");

            var imageUrl = product.Image != null
                   ? await SaveImageAsync(product.Image)
                   : null;
            var newProduct = new Product
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Image = imageUrl,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ColorId = product.ColorId,
                ModelId = product.ModelId,
                SellStartDate = product.SellStartDate,
                SellEndDate = product.SellEndDate,
                IsNew = product.IsNew,
                Description = product.Description
            };
            await _productRepository.AddAsync(newProduct);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(ProductRequestDto product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(ProductRequestDto));

            var Image = product.Image != null
                   ? await SaveImageAsync(product.Image)
                   : null;

            var existingProduct = await _productRepository.GetByIdAsync(product.ProductId);
            if (existingProduct == null)
                throw new KeyNotFoundException("Product not found");

            existingProduct.ProductName = product.ProductName ?? existingProduct.ProductName;
            existingProduct.Image = Image ?? existingProduct.Image;
            existingProduct.Description = product.Description ?? existingProduct.Description;
            if (product.Price > 0)
                existingProduct.Price = product.Price;
            if (product.CategoryId > 0)
                existingProduct.CategoryId = product.CategoryId;
            if (product.ColorId > 0)
                existingProduct.ColorId = product.ColorId;
            if (product.ModelId > 0)
                existingProduct.ModelId = product.ModelId;
            if (product.SellStartDate != default(DateTime))
                existingProduct.SellStartDate = product.SellStartDate;
            if (product.SellEndDate.HasValue)
                existingProduct.SellEndDate = product.SellEndDate;
            if (product.IsNew)
                existingProduct.IsNew = product.IsNew;

            await _productRepository.UpdateAsync(existingProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Product ID");
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");
            await _productRepository.DeleteAsync(product.ProductId);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Image = p.Image,
                Price = p.Price,
                CategoryId = p.CategoryId,
                ColorId = p.ColorId,
                ModelId = p.ModelId,
                SellStartDate = p.SellStartDate,
                SellEndDate = p.SellEndDate,
                IsNew = p.IsNew
            });
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Product ID");
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");
            return new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Image = product.Image,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ColorId = product.ColorId,
                ModelId = product.ModelId,
                SellStartDate = product.SellStartDate,
                SellEndDate = product.SellEndDate,
                IsNew = product.IsNew
            };
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file");

            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var fileExtension = Path.GetExtension(imageFile.FileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif" };

            if (!allowedExtensions.Contains(fileExtension.ToLower()))
                throw new InvalidOperationException("Unsupported image format");

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/images/{uniqueFileName}";
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByPagination(ProductPaginationDto productPaginationDto)
        {
            if (productPaginationDto == null)
                throw new ArgumentException("Pagination Parameters are Empty");

            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Color)
                .Include(p => p.Model)
                .AsQueryable();

            // Filter: Category
            if (!string.IsNullOrEmpty(productPaginationDto.Category))
            {
                query = query.Where(p => p.Category.Name.ToLower().Contains(productPaginationDto.Category.ToLower()));
            }

            // Filter: Search Term
            if (!string.IsNullOrEmpty(productPaginationDto.SearchTerm))
            {
                query = query.Where(p => p.ProductName.ToLower().Contains(productPaginationDto.SearchTerm.ToLower()));
            }

            // Filter: Price Range
            if (productPaginationDto.MinPrice > 0)
            {
                query = query.Where(p => p.Price >= productPaginationDto.MinPrice);
            }
            if (productPaginationDto.MaxPrice > 0)
            {
                query = query.Where(p => p.Price <= productPaginationDto.MaxPrice);
            }

            // Pagination
            int skip = (productPaginationDto.PageNumber - 1) * productPaginationDto.PageSize;
            query = query.Skip(skip).Take(productPaginationDto.PageSize);

            // Projection
            var result = await query.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Image = p.Image,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                Name = p.Category != null ? p.Category.Name : null,
                ColorId = p.ColorId,
                ColorName = p.Color != null ? p.Color.ColorName : null,
                ModelId = p.ModelId,
                ModelName = p.Model != null ? p.Model.ModelName : null,
                SellStartDate = p.SellStartDate,
                SellEndDate = p.SellEndDate,
                IsNew = p.IsNew
            }).ToListAsync();

            return result;
        }

    }
}