using ElearnApi.Models;
using ElearnApi.Repositories;
using ElearnApi.Context;
using ElearnApi.Interface;
using ElearnApi.Model.DTOs;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElearnApi.Controllers
{
    [ApiController]
    [Route("api/videos")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo([FromForm] UploadVideoDto uploadVideoDto, IFormFile videoFile, IFormFile thumbnailFile)
        {
            if (videoFile == null || videoFile.Length == 0)
            {
                return BadRequest("No video file provided.");
            }

            var video = await _videoService.UploadVideoAsync(uploadVideoDto, videoFile, thumbnailFile);
            if (video == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading video.");
            }
            return Ok(video);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            if (videos == null || !videos.Any())
            {
                return NotFound("No videos found.");
            }
            return Ok(videos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideoById(int id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);
            if (video == null)
            {
                return NotFound($"Video not found for this id : ${id}.");
            }
            return Ok(video);
        }
    }
}