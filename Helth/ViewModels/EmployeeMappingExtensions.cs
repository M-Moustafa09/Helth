using Helth.Models;

namespace Helth.ViewModels;

public static class EmployeeMappingExtensions
{
    public static EmployeeFormViewModel ToFormViewModel(this Employee employee)
    {
        return new EmployeeFormViewModel
        {
            Id = employee.Id,
            PhotoPath = employee.PhotoPath,
            Municipality = employee.Municipality,
            SubMunicipality = employee.SubMunicipality,
            FullName = employee.FullName,
            NationalId = employee.NationalId,
            Gender = employee.Gender,
            Nationality = employee.Nationality,
            HealthCertificateNumber = employee.HealthCertificateNumber,
            Profession = employee.Profession,
            IssueDateHijri = employee.IssueDateHijri,
            IssueDateGregorian = employee.IssueDateGregorian,
            TrainingProgramType = employee.TrainingProgramType,
            TrainingProgramExpiryDate = employee.TrainingProgramExpiryDate,
            LicenseNumber = employee.LicenseNumber,
            FacilityName = employee.FacilityName,
            FacilityNumber = employee.FacilityNumber
        };
    }

    public static void ApplyTo(this EmployeeFormViewModel form, Employee employee)
    {
        employee.Municipality = form.Municipality;
        employee.SubMunicipality = form.SubMunicipality;
        employee.FullName = form.FullName;
        employee.NationalId = form.NationalId;
        employee.Gender = form.Gender;
        employee.Nationality = form.Nationality;
        employee.HealthCertificateNumber = form.HealthCertificateNumber;
        employee.Profession = form.Profession;
        employee.IssueDateHijri = form.IssueDateHijri;
        employee.IssueDateGregorian = form.IssueDateGregorian;
        employee.TrainingProgramType = form.TrainingProgramType;
        employee.TrainingProgramExpiryDate = form.TrainingProgramExpiryDate;
        employee.LicenseNumber = form.LicenseNumber;
        employee.FacilityName = form.FacilityName;
        employee.FacilityNumber = form.FacilityNumber;
    }
}
