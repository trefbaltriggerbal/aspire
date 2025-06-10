using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp.Data;

public class InsuranceClaim
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Policy")]
    public int PolicyId { get; set; }
    public Policy? Policy { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime IncidentDate { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty; // e.g., Open, Closed
}
