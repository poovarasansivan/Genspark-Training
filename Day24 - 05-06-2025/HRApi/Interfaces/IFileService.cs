using HRApi.Models;

namespace HRApi.Interfaces
{
    public interface IFileService
    {
        Task<FileModel> SaveFileAsync(IFormFile file);
        Task<FileModel?> GetFileAsync(int id);
    }
}
