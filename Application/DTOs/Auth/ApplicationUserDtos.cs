using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    /// <summary>
    /// Represents the payload required to authenticate a user.
    /// </summary>
    /// <param name="Email">The user's email address.</param>
    /// <param name="Password">The user's password.</param>
    public record LoginDataDto(
        string Email,
        string Password
        );

    /// <summary>
    /// Represents the payload required to register a new user account.
    /// </summary>
    /// <param name="UserName">The chosen username for the account.</param>
    /// <param name="Email">The email address for the account.</param>
    /// <param name="PhoneNumber">The contact phone number.</param>
    /// <param name="Password">The password for the account.</param>
    public record RegisterDataDto(
        string UserName,
        string Email,
        string PhoneNumber,
        string Password
        );
}
