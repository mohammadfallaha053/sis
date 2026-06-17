// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using LapisApi.App.Auth.Interfaces;
// using LapisApi.App.Comments.Dto;
// using LapisApi.App.Comments.Dto.Request.Queries;
// using LapisApi.App.Comments.Interfaces;
// using LapisApi.Interfaces.Auth;
// namespace LapisApi.App.Comments.Controller;
//
// [Authorize(Roles = "Admin")]
// [ApiController]
// [Route("api/[controller]")]
// public class CommentController : ControllerBase
// {
//   private readonly ICommentService _commentService;
//   private IClaimService _claimService;
//
//   public CommentController(ICommentService CommentService, IClaimService claimsService)
//   {
//     _commentService = CommentService;
//     _claimService = claimsService;
//   }
//   
//   [HttpGet("get-all")]
//   public async Task<IActionResult> GetAll([FromQuery] CommentGetAllQuery query)
//   {
//     var result = await _commentService.GetAllAsync(query);
//     return result.ToActionResult(this);
//   }
//
//   [HttpGet("get-by-id/{id}")]
//   public async Task<IActionResult> GetById(int id)
//   {
//     var result = await _commentService.GetByIdAsync(id);
//     return result.ToActionResult(this);
//   }
//
//   [HttpPut("edit/{id}")]
//   public async Task<IActionResult> Update(int id, [FromBody] CommentUpdateCommand command)
//   {
//     var result = await _commentService.UpdateAsync(id, command);
//
//     return result.ToActionResult(this);
//   }
//
//   [HttpDelete("delete/{id}")]
//   public async Task<IActionResult> Delete(int id)
//   {
//     var result = await _commentService.DeleteAsync(id);
//
//     if (result.IsSuccess)
//     {
//       return NoContent();
//     }
//
//     return result.ToActionResult(this);
//   }
// }