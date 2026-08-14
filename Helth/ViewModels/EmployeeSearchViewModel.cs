using Helth.Models;

namespace Helth.ViewModels;

public class EmployeeSearchViewModel
{
    public string? SearchTerm { get; set; }
    public List<Employee> Employees { get; set; } = new();
}
