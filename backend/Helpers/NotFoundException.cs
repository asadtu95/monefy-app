namespace Monefy.Api.Helpers;

public class NotFoundException(string message) : Exception(message);

public class ValidationFailedException(string message) : Exception(message);

public class UnauthorizedException(string message) : Exception(message);
