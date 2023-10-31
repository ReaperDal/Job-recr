using Microsoft.AspNetCore.Identity;

namespace Job_Recruitment.Models;

public class User:IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Image? Image { get; set; }
}
