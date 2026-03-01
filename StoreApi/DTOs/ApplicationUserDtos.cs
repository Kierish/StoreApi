using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public record LoginDataDto(
        string Email,
        string Password
        );

    public record RegisterDataDto(
        string UserName,
        string Email,
        string PhoneNumber,
        string Password
        );
}
