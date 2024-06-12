using AIDMS.Entities;

namespace AIDMS.DTOs;
public class BaseEmployeeDto {
    public int Id { get; set; }
    public string Name { get; set; }
    public string? roleName { get; set; }
}