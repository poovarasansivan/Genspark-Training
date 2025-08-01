using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs.TaskFiles;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Services
{
    public class TaskFileService : ITaskFileService
    {
        private readonly ITaskFileRepository _fileRepo;
        private readonly ITaskItemRepository _taskRepo;
        //private readonly IFileBlobService _blobService;

        public TaskFileService(ITaskFileRepository fileRepo, ITaskItemRepository taskRepo)
        {
            _fileRepo = fileRepo;
            _taskRepo = taskRepo;
            //_blobService = blobService;
        }

        public async Task<TaskFileDto> UploadFileAsync(Guid taskId, IFormFile file, Guid userId)
        {
            var task = await _taskRepo.Get(taskId);
            if (task == null || task.IsDeleted)
                throw new ArgumentException("Task not found or deleted");

            if (task.CreatedById != userId)
                throw new UnauthorizedAccessException("Only the manager who created this task can upload files.");

            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("File size exceeds the limit (5 MB).");

            var allowedTypes = new[]
            {
                "image/png",
                "image/jpeg",
                "application/pdf",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            if (!allowedTypes.Contains(file.ContentType))
                throw new ArgumentException("Only PNG, JPEG, PDF, and DOCX files are allowed.");

            var existingFiles = await _fileRepo.GetByTaskIdAsync(taskId);
            if (existingFiles.Any(f => f.FileName.Equals(file.FileName, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("A file with the same name already exists for this task.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var taskFile = new TaskFile
            {
                Id = Guid.NewGuid(),
                TaskItemId = taskId,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Data = ms.ToArray(),
                UploadedAt = DateTime.UtcNow,
            };

            await _fileRepo.AddAsync(taskFile);

            return new TaskFileDto
            {
                Id = taskFile.Id,
                FileName = taskFile.FileName,
                ContentType = taskFile.ContentType,
                UploadedAt = taskFile.UploadedAt,
            };
        }




       public async Task<IEnumerable<TaskFileDto>> GetFilesAsync(Guid taskId, Guid userId)
        {
            var task = await _taskRepo.Get(taskId);
            if (task == null || task.IsDeleted)
                throw new ArgumentException("Task not found or deleted");

            if (task.CreatedById != userId && task.AssignedToId != userId)
                throw new UnauthorizedAccessException("You are not authorized to view files for this task.");

            var files = await _fileRepo.GetByTaskIdAsync(taskId);
            if (files == null || !files.Any())
                throw new ArgumentException("No files found for the given task.");

            return files.Select(f => new TaskFileDto
            {
                Id = f.Id,
                FileName = f.FileName,
                UploadedAt = f.UploadedAt,
                TaskTitle = task.Title 
            });
        }



       public async Task<FileStreamResult> DownloadFileAsync(Guid fileId, Guid userId)
        {
            var file = await _fileRepo.GetByIdAsync(fileId);
            if (file == null)
                throw new ArgumentException("File not found");

            var task = await _taskRepo.Get(file.TaskItemId);
            if (task == null || task.IsDeleted)
                throw new ArgumentException("Associated task not found or deleted");

            if (task.CreatedById != userId && task.AssignedToId != userId)
                throw new UnauthorizedAccessException("You are not authorized to download this file.");

            var stream = new MemoryStream(file.Data);
            return new FileStreamResult(stream, file.ContentType)
            {
                FileDownloadName = file.FileName
            };
        }


    }
    // public class TaskFileService : ITaskFileService
    // {
    //     private readonly ITaskFileRepository _fileRepo;
    //     private readonly ITaskItemRepository _taskRepo;
    //     private readonly IFileBlobService _blobService;
    //     private readonly string _blobBaseUrl;

    //     public TaskFileService(ITaskFileRepository fileRepo, ITaskItemRepository taskRepo, IFileBlobService blobService, IConfiguration config)
    //     {
    //         _fileRepo = fileRepo;
    //         _taskRepo = taskRepo;
    //         _blobService = blobService;
    //         _blobBaseUrl = config["AzureBlob:BaseUrl"];
    //     }

    //     public async Task<TaskFileDto> UploadFileAsync(Guid taskId, IFormFile file, Guid userId)
    //     {
    //         var task = await _taskRepo.Get(taskId);
    //         if (task == null || task.IsDeleted)
    //             throw new ArgumentException("Task not found or deleted");

    //         if (task.CreatedById != userId)
    //             throw new UnauthorizedAccessException("Only the manager who created this task can upload files.");

    //         if (file.Length > 5 * 1024 * 1024)
    //             throw new ArgumentException("File size exceeds the limit (5 MB).");

    //         var allowedTypes = new[]
    //         {
    //             "image/png", "image/jpeg", "application/pdf", 
    //             "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    //         };
    //         if (!allowedTypes.Contains(file.ContentType))
    //             throw new ArgumentException("Only PNG, JPEG, PDF, and DOCX files are allowed.");

    //         var existingFiles = await _fileRepo.GetByTaskIdAsync(taskId);
    //         if (existingFiles.Any(f => f.FileName.Equals(file.FileName, StringComparison.OrdinalIgnoreCase)))
    //             throw new ArgumentException("A file with the same name already exists for this task.");

    //         // Upload to Azure Blob
    //         var blobName = await _blobService.UploadAsync(file);

    //         var taskFile = new TaskFile
    //         {
    //             Id = Guid.NewGuid(),
    //             TaskItemId = taskId,
    //             FileName = file.FileName,
    //             ContentType = file.ContentType,
    //             UploadedAt = DateTime.UtcNow,
    //             FileUrl = blobName
    //         };

    //         await _fileRepo.AddAsync(taskFile);

    //         return new TaskFileDto
    //         {
    //             Id = taskFile.Id,
    //             FileName = taskFile.FileName,
    //             ContentType = taskFile.ContentType,
    //             UploadedAt = taskFile.UploadedAt,
    //             FileUrl = $"{_blobBaseUrl}/{blobName}"
    //         };
    //     }

    //     public async Task<IEnumerable<TaskFileDto>> GetFilesAsync(Guid taskId, Guid userId)
    //     {
    //         var task = await _taskRepo.Get(taskId);
    //         if (task == null || task.IsDeleted)
    //             throw new ArgumentException("Task not found or deleted");

    //         if (task.CreatedById != userId && task.AssignedToId != userId)
    //             throw new UnauthorizedAccessException("You are not authorized to view files for this task.");

    //         var files = await _fileRepo.GetByTaskIdAsync(taskId);
    //         return files.Select(f => new TaskFileDto
    //         {
    //             Id = f.Id,
    //             FileName = f.FileName,
    //             UploadedAt = f.UploadedAt,
    //             TaskTitle = task.Title,
    //             FileUrl = f.FileUrl
    //         });
    //     }

    //     public async Task<FileStreamResult> DownloadFileAsync(Guid fileId, Guid userId)
    //     {
    //         var file = await _fileRepo.GetByIdAsync(fileId);
    //         if (file == null)
    //             throw new ArgumentException("File not found");

    //         var task = await _taskRepo.Get(file.TaskItemId);
    //         if (task == null || task.IsDeleted)
    //             throw new ArgumentException("Associated task not found or deleted");

    //         if (task.CreatedById != userId && task.AssignedToId != userId)
    //             throw new UnauthorizedAccessException("You are not authorized to download this file.");

    //         var blobName = file.FileUrl;

    //         var stream = await _blobService.DownloadAsync(blobName);

    //         return new FileStreamResult(stream, file.ContentType)
    //         {
    //             FileDownloadName = file.FileName
    //         };
    //     }

    // }

}
