using MigratedApi.Models.Dtos;
using MigratedApi.Interfaces;
using MigratedApi.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MigratedApi.Models;
using ClosedXML.Excel;
using System.Text;

namespace MigratedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNews([FromForm] NewsRequestDto newsDto)
        {
            try
            {
                var createdNews = await _newsService.CreateNewsAsync(newsDto);
                return Ok(SuccessResponseHandler.Success(createdNews, "News created successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                var result = await _newsService.DeleteNewsAsync(id);
                return Ok(SuccessResponseHandler.Success(result, "News deleted successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetNewsById(int id)
        {
            try
            {
                var news = await _newsService.GetNewsByIdAsync(id);
                return Ok(SuccessResponseHandler.Success(news, "News retrieved successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllNews()
        {
            try
            {
                var newsList = await _newsService.GetAllNewsAsync();
                return Ok(SuccessResponseHandler.Success(newsList, "All news retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateNews([FromForm] NewsRequestDto newsDto)
        {
            try
            {
                var updatedNews = await _newsService.UpdateNewsAsync(newsDto);
                return Ok(SuccessResponseHandler.Success(updatedNews, "News updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("export-news")]
        [Authorize]
        public async Task<IActionResult> ExportNews()
        {
            try
            {
                var newsListDto = await _newsService.GetAllNewsAsync();
                if (newsListDto == null || !newsListDto.Any())
                    return Ok(SuccessResponseHandler.Success<IEnumerable<NewsRequestDto>>(new List<NewsRequestDto>(), "No news available for export."));

                var newsList = newsListDto.Select(dto => new News
                {
                    NewsId = dto.NewsId,
                    UserId = dto.UserId,
                    Title = dto.Title,
                    ShortDescription = dto.ShortDescription,
                    Content = dto.Content,
                    CreatedDate = dto.CreatedDate,
                    Status = dto.Status
                });

                var fileContent = await ExportToExcel(newsList);
                var fileName = $"NewsExport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("export-news-csv")]
        [Authorize]
        public async Task<IActionResult> ExportNewsAsCsv()
        {
            try
            {
                var newsListDto = await _newsService.GetAllNewsAsync();
                if (newsListDto == null || !newsListDto.Any())
                    return Ok(SuccessResponseHandler.Success<IEnumerable<NewsRequestDto>>(new List<NewsRequestDto>(), "No news available for export."));

                var newsList = newsListDto.Select(dto => new News
                {
                    NewsId = dto.NewsId,
                    UserId = dto.UserId,
                    Title = dto.Title,
                    ShortDescription = dto.ShortDescription,
                    Content = dto.Content,
                    CreatedDate = dto.CreatedDate,
                    Status = dto.Status
                });

                var csvContent = await ExportToCsv(newsList);
                var fileName = $"NewsExport_{DateTime.Now:yyyyMMddHHmmss}.csv";

                return File(new UTF8Encoding(true).GetBytes(csvContent), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<string> ExportToCsv(IEnumerable<News> newsList)
        {
            var sb = new StringBuilder();

            
            sb.AppendLine("NewsId,UserId,Title,ShortDescription,Content,CreatedDate,Status");

            foreach (var news in newsList)
            {
                var line = string.Join(",", new[]
                {
                    news.NewsId.ToString(),
                    news.UserId.ToString(),
                    EscapeCsv(news.Title),
                    EscapeCsv(news.ShortDescription),
                    EscapeCsv(news.Content),
                    news.CreatedDate.ToString("yyyy-MM-dd HH:mm"),
                    EscapeCsv(news.Status)
                });

                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        private string EscapeCsv(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            if (input.Contains(",") || input.Contains("\"") || input.Contains("\n"))
                return $"\"{input.Replace("\"", "\"\"")}\"";

            return input;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<byte[]> ExportToExcel(IEnumerable<News> newsList)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("News");

            
                worksheet.Cell(1, 1).Value = "News ID";
                worksheet.Cell(1, 2).Value = "User ID";
                worksheet.Cell(1, 3).Value = "Title";
                worksheet.Cell(1, 4).Value = "Short Description";
                worksheet.Cell(1, 5).Value = "Content";
                worksheet.Cell(1, 6).Value = "Created Date";
                worksheet.Cell(1, 7).Value = "Status";

                int row = 2;
                foreach (var news in newsList)
                {
                    worksheet.Cell(row, 1).Value = news.NewsId;
                    worksheet.Cell(row, 2).Value = news.UserId;
                    worksheet.Cell(row, 3).Value = news.Title;
                    worksheet.Cell(row, 4).Value = news.ShortDescription;
                    worksheet.Cell(row, 5).Value = news.Content;
                    worksheet.Cell(row, 6).Value = news.CreatedDate.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cell(row, 7).Value = news.Status;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}