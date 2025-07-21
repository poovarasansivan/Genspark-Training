using ElearnApi.Models;
using ElearnApi.Repositories;
using ElearnApi.Interface;
using ElearnApi.Context;
using ElearnApi.Model.DTOs;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace ElearnApi.Services
{
    public class VideoService : IVideoService
    {
        private readonly ManageVideoRepository _videoRepository;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly BlobContainerClient _containerClient;

        public VideoService(ManageVideoRepository videoRepository, AppDbContext context, IConfiguration config)
        {
            _videoRepository = videoRepository;
            _context = context;
            _config = config;

            var blobServiceClient = new BlobServiceClient(_config.GetConnectionString("BlobStorage"));
            _containerClient = blobServiceClient.GetBlobContainerClient("videos");
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }


        public async Task<VideoModel> UploadVideoAsync(UploadVideoDto uploadVideoDto, IFormFile videoFile, IFormFile thumbnailFile)
        {
            if (uploadVideoDto == null)
            {
                throw new ArgumentNullException(nameof(uploadVideoDto), "Upload video DTO cannot be null");
            }

            if (videoFile == null || videoFile.Length == 0)
            {
                throw new ArgumentException("Video file cannot be null or empty", nameof(videoFile));
            }

            if (thumbnailFile == null || thumbnailFile.Length == 0)
            {
                throw new ArgumentException("Thumbnail file cannot be null or empty", nameof(thumbnailFile));
            }

            var blobname = $"{Guid.NewGuid()}_{videoFile.FileName}";
            var blobClient = _containerClient.GetBlobClient(blobname);

            var thumbnailBlobName = $"{Guid.NewGuid()}_{thumbnailFile.FileName}";
            var thumbnailBlobClient = _containerClient.GetBlobClient(thumbnailBlobName);

            using (var thumbnailStream = thumbnailFile.OpenReadStream())
            {
                await thumbnailBlobClient.UploadAsync(thumbnailStream, new BlobHttpHeaders { ContentType = thumbnailFile.ContentType });
            }

            using (var stream = videoFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = videoFile.ContentType });
            }

            var videoModel = new VideoModel
            {
                Title = uploadVideoDto.Title,
                Description = uploadVideoDto.Description,
                UploadDate = uploadVideoDto.UploadDate,
                BlobUrl = blobClient.Uri.ToString(),
                Category = uploadVideoDto.Category,
                ThumbnailUrl = thumbnailBlobClient.Uri.ToString(),
            };

            await _videoRepository.AddAsync(videoModel);
            return videoModel;

        }

        public async Task<List<VideoModel>> GetAllVideosAsync()
        {
            return (await _videoRepository.GetAllAsync()).ToList();
        }
        public async Task<VideoModel?> GetVideoByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Video ID must be greater than zero", nameof(id));
            }
            return await _videoRepository.GetByIdAsync(id);
        }

    }
}