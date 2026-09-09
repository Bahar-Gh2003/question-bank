namespace Application.Common;

/// <summary>The requested entity was not found -> 404</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>The request violates a business rule -> 400</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>The user may not access this resource -> 403</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}