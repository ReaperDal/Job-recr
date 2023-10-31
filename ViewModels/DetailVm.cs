using Job_Recruitment.Models;

namespace Job_Recruitment.ViewModels;

public class DetailVm
{
    public User User { get; set; }
    public Job Job { get; set; }
    public List<Application> Applications { get; set; }
    public Dictionary<string, string> CandidateNames { get; set; } 
}
