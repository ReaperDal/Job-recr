using Job_Recruitment.Models;

namespace Job_Recruitment.ViewModels;

public class UserVm
{
    public User? User { get; set; }
    public string? Role { get; set; }
    public List<Job> Jobs { get; set; }
}
