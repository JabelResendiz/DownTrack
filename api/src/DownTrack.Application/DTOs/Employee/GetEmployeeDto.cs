

namespace DownTrack.Application.DTO;

//DTO de mostrado de caracteristicas de un empleado

public class GetEmployeeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string UserRole { get; set; } = null!;
    public string? Email { get; set; }
    public string? UserName { get; set; }

}