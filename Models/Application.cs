namespace Job_Recruitment.Models;

public class Application
{
    public int Id { get; set; }
    public Job? Job { get; set; }
    public string CandidateId { get; set; }
    public DateTime DateApplied { get; set; } = DateTime.Now;
    public string Status { get; set; }
    public int JobId { get; set; }
    public bool IsActive { get; set; } = true;
}
