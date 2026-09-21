namespace Enterprise.Models;

public partial class Learnlog
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public string? Description { get; set; }

    public int? uid { get; set; }
}
