namespace Job_Recruitment.Models;

public class Interview
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public Application? Application { get; set; }
    public DateTime DateScheduled { get; set; }
    public string InterviewerName { get; set; }
    public string? Result { get; set; }
}
