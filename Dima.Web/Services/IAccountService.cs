using Dima.Web.Models.Identity;

namespace Dima.Web.Services;

public interface IAccountService
{
    Task<FormResult> LoginAsync(string email, string password);
    Task<FormResult> RegisterAsync(string email, string password);
    Task LogoutAsync();
    Task<bool> CheckAuthenticatedAsync();
}