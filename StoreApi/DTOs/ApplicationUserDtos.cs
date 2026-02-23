using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public record LoginDataDto(
        [Required] string Email,
        [Required] string Password
        );

    public record RegisterDataDto(
        [Required] string UserName,
        [Required] string Email,
        [Required] string PhoneNumber,
        [Required] string Password
        );
}
