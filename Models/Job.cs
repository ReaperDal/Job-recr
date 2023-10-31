namespace Job_Recruitment.Models;

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public double Salary { get; set; }
    public DateTime PostDate { get; set; } = DateTime.Now;
    public User Employer { get; set; }
    public string EmployerId { get; set; }
    public string JobType { get; set; }
    public bool IsActive { get; set; }
    public List<Application> Applications { get; set; }
}
