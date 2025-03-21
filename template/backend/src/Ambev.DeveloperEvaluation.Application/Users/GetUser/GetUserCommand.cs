using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUser;

/// <summary>
/// Command for retrieving a user by their ID
/// </summary>
public record GetUserCommand : IRequest<GetUserResult>
{
    /// <summary>
    /// The unique identifier of the user to retrieve
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Initializes a new instance of GetUserCommand
    /// </summary>
    public GetUserCommand(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of GetUserCommand with default values
    /// </summary>
    public GetUserCommand()
    {
        Id = Guid.Empty;
    }
}
