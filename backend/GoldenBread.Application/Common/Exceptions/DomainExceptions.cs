using GoldenBread.Application.Common.Constants;
using GoldenBread.Domain.Exceptions;

namespace GoldenBread.Application.Common.Exceptions;

public enum AuthErrorType
{
    InvalidCredentials,
    ExpiredToken,
}

public class AuthException(string message, AuthErrorType type) 
    : UnauthorizedAccessException(message) 
{
    public AuthErrorType Type { get; } = type;
}

public class NotFoundException(string message) 
    : Exception(message) { }

public class BusinessValidationException(string propertyName, string message) 
    : DomainExceptions(propertyName, message) { }

public class DuplicateEntityException(string entityProperty) :
    BusinessValidationException(entityProperty, ValidationErrorConstants.Duplicate) { }


