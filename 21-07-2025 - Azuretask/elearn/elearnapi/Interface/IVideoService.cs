using ElearnApi.Models;
using ElearnApi.Model.DTOs;
namespace ElearnApi.Interface
{
    public interface IVideoService
    {
        Task<VideoModel> UploadVideoAsync(UploadVideoDto uploadVideoDto, IFormFile videoFile, IFormFile thumbnailFile);
        Task<List<VideoModel>> GetAllVideosAsync();
        Task<VideoModel?> GetVideoByIdAsync(int id);
    }
}