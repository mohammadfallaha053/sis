using LapisApi.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisApi.App.MediaFiles.Interfaces;
using SisApi.Data;
namespace SisApi.App.MediaFiles.Controller;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
  private readonly IWebHostEnvironment _env;
  private readonly ApplicationDbContext _context;
  private readonly IFileService _fileService;

  public FileUploadController(IWebHostEnvironment env, ApplicationDbContext context, IFileService fileService)
  {
    _env = env;
    _context = context;
    _fileService = fileService;
  }
  [ServiceFilter(typeof(ActiveUserAuthorizationFilter))]
  [HttpPost("upload")]
  public async Task<IActionResult> Upload(IFormFile file)
  {
    var result = await _fileService.UploadTempFileAsync(file);
    return result.ToActionResult(this);
  }

  //[HttpDelete("clear-temp")]
  //public async Task<IActionResult> ClearTempFiles()
  //{
  //  var result = await _fileService.ClearTempFilesAsync();
  //  return result.ToActionResult(this);
  //}
  // [Authorize(Roles = nameof(RoleEnum.Admin))]
  // [HttpPost("attach")]
  // public async Task<IActionResult> AttachFilesToEntity([FromBody] AttachFilesRequest dto)
  // {
  //   var result = await _fileService.AttachFilesAsync(dto.FileIds, dto.EntityType, dto.EntityId);
  //
  //   return result.ToActionResult(this);
  // }
  //
  // [Authorize(Roles = nameof(RoleEnum.Admin))]
  // [HttpPost("delete")]
  // public async Task<IActionResult> RemoveFilesFromEntity([FromBody] List<int> fileIds)
  // {
  //   var result = await _fileService.DeleteFilesAsync(fileIds);
  //
  //   return result.ToActionResult(this);
  // }
}

