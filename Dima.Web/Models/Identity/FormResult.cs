namespace Dima.Web.Models.Identity;

public class FormResult
{
    public bool Succeed { get; set; }
    public string[] ErrorList { get; set; } = [];
}