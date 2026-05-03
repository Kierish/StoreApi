using Domain.Results;

namespace Domain.ErrorMessages
{
    public static class UserErrors
    {
        public static Error InvalidCredentials() => Error.Unauthorized(
            "User.InvalidCredentials", "Invalid email or password.");
        public static Error UserNotFound(Guid id) => Error.NotFound(
            "User.NotFound", $"The user with ID {id} was not found.");
        public static Error UserAlreadyExists(string email) => Error.Unauthorized(
            "User.UserAlreadyExists", $"The user with Email {email} already exists.");
    }
}
