using LapisApi.Helpers.Responses;
namespace LapisApi.App.Comments.Errors;

public static class CommentErrors
{
  public static readonly Error NotFound = new(
    code: "Comment.NotFound",
    messageAr: "كود الخصم غير موجود",
    messageEn: "Comment not found",
    type: ErrorType.NotFound
  );

  public static readonly Error AlreadyExists = new(
    code: "Comment.AlreadyExists",
    messageAr: "كود الخصم موجود بالفعل",
    messageEn: "Comment already exists",
    type: ErrorType.NotFound
  );


}