using HRApi.Models.DTOs.FileHandlingDtos;
using HRApi.Interfaces;
using HRApi.Models;
using HRApi.Repositories;
using Microsoft.EntityFrameworkCore;


namespace HRApi.Services
{
    public class FileService : IFileService
    {
        private readonly FileRepository _fileRepository;

        public FileService(FileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<FileModel> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.");

            var fileModel = new FileModel
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
            };

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileModel.Content = memoryStream.ToArray();
            }

            return await _fileRepository.AddAsync(fileModel);
        }

        public async Task<FileModel?> GetFileAsync(int id)
        {
            return await _fileRepository.Get(id);
        }
    }
}