using System.ComponentModel.DataAnnotations;

namespace Helth.Models.Enums;

public enum Gender
{
    [Display(Name = "ذكر")]
    Male,

    [Display(Name = "أنثى")]
    Female
}
