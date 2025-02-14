using DownTrack.Application.DTO;

namespace DownTrack.Application.IServices;

public interface IDoneMaintenanceCommandServices : IGenericCommandService<DoneMaintenanceDto>
{
    Task FinalizeMaintenanceAsync(FinalizeMaintenanceDto requestFinalize);
}