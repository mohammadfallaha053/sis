namespace LapisApi.Helpers.Responses;

public enum ErrorType
{
  Validation, // BadRequest (400)
  NotFound, // NotFound (404)
  Unauthorized, // Unauthorized (401)
  ServerError // InternalServerError (500)
}