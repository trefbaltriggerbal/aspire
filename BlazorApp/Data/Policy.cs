using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp.Data;

public class Policy
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    [Required]
    public string PolicyNumber { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<InsuranceClaim> Claims { get; set; } = new();
}
