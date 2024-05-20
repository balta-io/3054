namespace Dima.Web.Models.Identity;

public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public Dictionary<string, string> Claims { get; set; } = [];
}