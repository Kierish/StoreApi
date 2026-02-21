using StoreApi.DTOs;

namespace StoreApi.Interfaces
{
    public interface IAccountSevice
    {
        Task<string> LoginUser(LoginDataDto dto);
        Task<string> RegisterUser(RegisterDataDto dto);
    }
}
