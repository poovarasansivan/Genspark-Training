using MigratedApi.Models.Dtos;
using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Repositories;
using MigratedApi.Models;

namespace MigratedApi.Services
{
    public class NewsService : INewsService
    {
        private readonly NewsRepository _newsRepository;
        private readonly ShopDbContext _context;

        public NewsService(NewsRepository newsRepository, ShopDbContext context)
        {
            _newsRepository = newsRepository;
            _context = context;
        }

        public async Task<NewsResponseDto> CreateNewsAsync(NewsRequestDto news)
        {
            if (news == null)
                throw new ArgumentNullException(nameof(news));

            Console.WriteLine($"Creating news with title: {news.Title}");

            if (string.IsNullOrEmpty(news.Title) || string.IsNullOrEmpty(news.Content))
                throw new ArgumentException("Title and Content cannot be null or empty");

            var imageUrl = news.Image != null
                   ? await SaveImageAsync(news.Image)
                   : null;

            var newNews = new News
            {
                UserId = news.UserId,
                Title = news.Title,
                ShortDescription = news.ShortDescription,
                Content = news.Content,
                CreatedDate = news.CreatedDate,
                Status = news.Status,
                Image = imageUrl
            };

            await _newsRepository.AddAsync(newNews);
            return new NewsResponseDto
            {
                UserId = newNews.UserId,
                Title = newNews.Title,
                ShortDescription = newNews.ShortDescription,
                Content = newNews.Content,
                CreatedDate = newNews.CreatedDate,
                Status = newNews.Status,
                Image = newNews.Image
            };
        }

        public async Task<bool> DeleteNewsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("News ID must be greater than zero");

            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null)
                throw new KeyNotFoundException($"News with ID {id} not found");
            await _newsRepository.DeleteAsync(news.NewsId);
            return true;
        }

        public async Task<IEnumerable<NewsResponseDto>> GetAllNewsAsync()
        {
            var newsList = await _newsRepository.GetAllAsync();
            if (newsList == null || !newsList.Any())
                return Enumerable.Empty<NewsResponseDto>();
            return newsList.Select(n => new NewsResponseDto
            {
                NewsId = n.NewsId,
                UserId = n.UserId,
                Title = n.Title,
                ShortDescription = n.ShortDescription,
                Content = n.Content,
                CreatedDate = n.CreatedDate,
                Status = n.Status,
                Image = n.Image
            });
        }

        public async Task<NewsResponseDto?> GetNewsByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("News ID must be greater than zero");

            var news = await _newsRepository.GetByIdAsync(id);
            if (news == null)
                return null;

            return new NewsResponseDto
            {
                NewsId = news.NewsId,
                UserId = news.UserId,
                Title = news.Title,
                ShortDescription = news.ShortDescription,
                Content = news.Content,
                CreatedDate = news.CreatedDate,
                Status = news.Status,
                Image = news.Image
            };
        }

        public async Task<NewsResponseDto?> UpdateNewsAsync(NewsRequestDto news)
        {
            if (news.NewsId <= 0)
                throw new ArgumentException("News ID must be greater than zero");

            if (news == null)
                throw new ArgumentNullException(nameof(news));

            var existingNews = await _newsRepository.GetByIdAsync(news.NewsId);
            if (existingNews == null)
                throw new KeyNotFoundException($"News with ID {news.NewsId} not found");

            existingNews.Title = news.Title ?? existingNews.Title;
            existingNews.ShortDescription = news.ShortDescription ?? existingNews.ShortDescription;
            existingNews.Content = news.Content ?? existingNews.Content;
            existingNews.CreatedDate = news.CreatedDate;
            existingNews.Status = news.Status ?? existingNews.Status;
            if (news.Image != null)
            {
                existingNews.Image = await SaveImageAsync(news.Image);
            }
            else
            {
                existingNews.Image = existingNews.Image;
            }

            await _newsRepository.UpdateAsync(existingNews);

            return new NewsResponseDto
            {
                NewsId = existingNews.NewsId,
                UserId = existingNews.UserId,
                Title = existingNews.Title,
                ShortDescription = existingNews.ShortDescription,
                Content = existingNews.Content,
                CreatedDate = existingNews.CreatedDate,
                Status = existingNews.Status,
                Image = existingNews.Image
            };
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Invalid image file");

            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BlogImages");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
            var fileExtension = Path.GetExtension(image.FileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif" };

            if (!allowedExtensions.Contains(fileExtension.ToLower()))
                throw new InvalidOperationException("Unsupported image format");

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return $"/BlogImages/{uniqueFileName}";
        }
    }
}