using GoldenBread.Application.Common.Constants;
using GoldenBread.Domain.Exceptions;

namespace GoldenBread.Application.Common.Exceptions;

public class AuthException(string message) 
    : UnauthorizedAccessException(message) { }

public class NotFoundException(string message) 
    : Exception(message) { }

public class BusinessValidationException(string propertyName, string message) 
    : DomainExceptions(propertyName, message) { }

public class DuplicateEntityException(string entityProperty) :
    BusinessValidationException(entityProperty, ValidationErrorConstants.Duplicate) { }


