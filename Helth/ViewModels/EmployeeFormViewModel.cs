using System.ComponentModel.DataAnnotations;
using Helth.Models.Enums;

namespace Helth.ViewModels;

public class EmployeeFormViewModel
{
    public int Id { get; set; }

    public string? PhotoPath { get; set; }

    [Display(Name = "صورة الموظف")]
    public IFormFile? PhotoFile { get; set; }

    [Required(ErrorMessage = "الأمانة مطلوبة")]
    [Display(Name = "الأمانة")]
    public string Municipality { get; set; } = string.Empty;

    [Required(ErrorMessage = "البلدية مطلوبة")]
    [Display(Name = "البلدية")]
    public string SubMunicipality { get; set; } = string.Empty;

    [Required(ErrorMessage = "الاسم مطلوب")]
    [Display(Name = "الاسم")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الهوية مطلوب")]
    [Display(Name = "رقم الهوية")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "الجنس مطلوب")]
    [Display(Name = "الجنس")]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = "الجنسية مطلوبة")]
    [Display(Name = "الجنسية")]
    public string Nationality { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم الشهادة الصحية مطلوب")]
    [Display(Name = "رقم الشهادة الصحية")]
    public string HealthCertificateNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "المهنة مطلوبة")]
    [Display(Name = "المهنة")]
    public string Profession { get; set; } = string.Empty;

    [Required(ErrorMessage = "تاريخ إصدار الشهادة الصحية (هجري) مطلوب")]
    [Display(Name = "تاريخ إصدار الشهادة الصحية (هجري)")]
    public string IssueDateHijri { get; set; } = string.Empty;

    [Required(ErrorMessage = "تاريخ إصدار الشهادة الصحية (ميلادي) مطلوب")]
    [Display(Name = "تاريخ إصدار الشهادة الصحية (ميلادي)")]
    [DataType(DataType.Date)]
    public DateTime IssueDateGregorian { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "نوع البرنامج التثقيفي مطلوب")]
    [Display(Name = "نوع البرنامج التثقيفي")]
    public string TrainingProgramType { get; set; } = string.Empty;

    [Required(ErrorMessage = "تاريخ انتهاء البرنامج التثقيفي مطلوب")]
    [Display(Name = "تاريخ انتهاء البرنامج التثقيفي")]
    [DataType(DataType.Date)]
    public DateTime TrainingProgramExpiryDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "رقم الرخصة مطلوب")]
    [Display(Name = "رقم الرخصة")]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "اسم المنشأة مطلوب")]
    [Display(Name = "اسم المنشأة")]
    public string FacilityName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رقم المنشأة مطلوب")]
    [Display(Name = "رقم المنشأة")]
    public string FacilityNumber { get; set; } = string.Empty;
}
