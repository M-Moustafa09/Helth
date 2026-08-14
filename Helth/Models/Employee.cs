using System.ComponentModel.DataAnnotations;
using Helth.Models.Enums;

namespace Helth.Models;

public class Employee
{
    public int Id { get; set; }

    public string? PhotoPath { get; set; }

    [Required, MaxLength(200)]
    public string Municipality { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string SubMunicipality { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string NationalId { get; set; } = string.Empty;

    [Required]
    public Gender Gender { get; set; }

    [Required, MaxLength(100)]
    public string Nationality { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string HealthCertificateNumber { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Profession { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string IssueDateHijri { get; set; } = string.Empty;

    [Required]
    public DateTime IssueDateGregorian { get; set; }

    [Required, MaxLength(200)]
    public string TrainingProgramType { get; set; } = string.Empty;

    [Required]
    public DateTime TrainingProgramExpiryDate { get; set; }

    [Required, MaxLength(100)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string FacilityName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FacilityNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
