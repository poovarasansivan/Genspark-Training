using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs.TaskFiles;

namespace TaskManagementAPI.Interfaces
{
    public interface ITaskFileService
    {
        Task <TaskFileDto>  UploadFileAsync(Guid taskId, IFormFile file, Guid userId);
        Task<IEnumerable<TaskFileDto>> GetFilesAsync(Guid taskId, Guid userId);
        Task<FileStreamResult> DownloadFileAsync(Guid fileId,Guid userId);
    }
}
