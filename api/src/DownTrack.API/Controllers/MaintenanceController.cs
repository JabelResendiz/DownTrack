using DownTrack.Application.DTO;
using DownTrack.Application.IServices;
using DownTrack.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DownTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceServices _maintenanceService;

    public MaintenanceController(IMaintenanceServices maintenanceServices)
    {
        _maintenanceService = maintenanceServices;
    }

    [HttpPost]
    [Route("POST")]

    public async Task<IActionResult> CreateMaintenance(MaintenanceDto maintenance)
    {
        await _maintenanceService.CreateAsync(maintenance);

        return Ok("Maintenance added successfully");
    }

    [HttpGet]
    [Route("GET_ALL")]

    public async Task<ActionResult<IEnumerable<Maintenance>>> GetAllMaintenance()
    {
        var results = await _maintenanceService.ListAsync();

        return Ok(results);

    }

    [HttpPut]
    [Route("PUT")]

    public async Task<IActionResult> UpdateMaintenance(MaintenanceDto maintenance)
    {
        var result = await _maintenanceService.UpdateAsync(maintenance);
        return Ok(result);
    }

    [HttpDelete]
    [Route("Delete")]

    public async Task<IActionResult> DeleteMaintenance(int maintenanceId)
    {
        await _maintenanceService.DeleteAsync(maintenanceId);

        return Ok("Maintenance deleted successfully");
    }
}