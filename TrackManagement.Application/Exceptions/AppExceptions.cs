namespace TrackManagement.Application.Exceptions;

/// <summary>
/// Thrown when a requested entity does not exist. Mapped to HTTP 404 by the
/// API's global exception handling middleware.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.") { }
}

public class ValidationAppException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationAppException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public ValidationAppException(string field, string error)
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]> { [field] = new[] { error } };
    }
}


public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
