using Microsoft.AspNetCore.Mvc;
using HRApi.Repository;
using HRApi.Models;
using HRApi.Models.DTOs.FileDto;
using HRApi.Interfaces;

namespace HRApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileRepository _fileRepo;

        public FilesController(FileRepository fileRepo)
        {
            _fileRepo = fileRepo;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File is empty or null");

            var savedFile = await _fileRepo.SaveFileAsync(dto.File);
            return Ok(new { savedFile.Id, savedFile.FileName });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            var file = await _fileRepo.GetFileAsync(id);
            if (file == null)
                return NotFound();

            return File(file.FileData, file.ContentType, file.FileName);
        }
    }

}