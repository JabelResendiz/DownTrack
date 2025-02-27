using DownTrack.Application.DTO;
using DownTrack.Application.DTO.Paged;
using DownTrack.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace DownTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class DoneMaintenanceController : ControllerBase
{
    private readonly IDoneMaintenanceQueryServices _doneMaintenanceQueryService;
    private readonly IDoneMaintenanceCommandServices _doneMaintenanceCommandService;
    public DoneMaintenanceController(IDoneMaintenanceQueryServices doneMaintenanceQueryServices,
                                     IDoneMaintenanceCommandServices doneMaintenanceCommandServices)
    {
        _doneMaintenanceQueryService = doneMaintenanceQueryServices;
        _doneMaintenanceCommandService = doneMaintenanceCommandServices;
        
    }

    #region Command
    [HttpPost]
    [Route("POST")]

    public async Task<IActionResult> CreateDoneMaintenance(DoneMaintenanceDto doneMaintenance)
    {
        await _doneMaintenanceCommandService.CreateAsync(doneMaintenance);

        return Ok("Done Maintenance added successfully");
    }

    [HttpPut]
    [Route("finish")]
    public async Task<IActionResult> FinalizeMaintenance(FinalizeMaintenanceDto requestFinalize)
    {
        await _doneMaintenanceCommandService.FinalizeMaintenanceAsync(requestFinalize);

        return Ok("Maintenance process finished successfully");
    }


    [HttpPut]
    [Route("PUT")]

    public async Task<IActionResult> UpdateDoneMaintenance(DoneMaintenanceDto doneMaintenance)
    {
        var result = await _doneMaintenanceCommandService.UpdateAsync(doneMaintenance);
        return Ok(result);
    }

    [HttpDelete]
    [Route("{doneMaintenanceId}")]

    public async Task<IActionResult> DeleteDoneMaintenance(int doneMaintenanceId)
    {
        await _doneMaintenanceCommandService.DeleteAsync(doneMaintenanceId);

        return Ok("Done Maintenance deleted successfully");
    }

    #endregion

    #region Query

    [HttpGet]
    [Route("GET")]

    public async Task<ActionResult<GetDoneMaintenanceDto>> GetDoneMaintenanceById(int doneMaintenanceId)
    {
        var result = await _doneMaintenanceQueryService.GetByIdAsync(doneMaintenanceId);

        if (result == null)
            return NotFound($"Done Maintenance with ID {doneMaintenanceId} not found");

        return Ok(result);

    }


    [HttpGet]
    [Route("GetPaged")]

    public async Task<IActionResult> GetPagedDoneMaintenance ([FromQuery]PagedRequestDto paged)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _doneMaintenanceQueryService.GetAllPagedResultAsync(paged);
        
        return Ok (result);
        
    }

    [HttpGet]
    [Route("GetAllMaintenanceByTechnicianId")]

    
    public async Task<ActionResult<GetDoneMaintenanceDto>> GetDoneMaintenanceByTechnicianId(
                                                            [FromQuery]PagedRequestDto paged,int technicianId)
    {
        var result = await _doneMaintenanceQueryService.GetByTechnicianIdAsync(paged,technicianId);

        return Ok(result);

    }

    [HttpGet]
    [Route("Get_Maintenances_By_Technician_Status")]
    public async Task<IActionResult> GetMaintenancesByTechnicianStatus ([FromQuery]PagedRequestDto paged,int technicianId, bool IsFinish)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var maintenance = await _doneMaintenanceQueryService.GetMaintenanceByTechnicianStatusAsync(paged,technicianId,IsFinish);

        return Ok(maintenance);
    }

    
    [HttpGet]
    [Route("Get_Maintenances_By_Technician_UserName")]
    public async Task<IActionResult> GetMaintenancesByTechnicianUserName([FromQuery] PagedRequestDto paged, string technicianUserName){
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var maintenance = await _doneMaintenanceQueryService.GetMaintenanceByTechnicianUserNameAsync(paged, technicianUserName);

        return Ok(maintenance);
    }

    [HttpGet]
    [Route("Get_Maintenances_By_EquipmentId")]
    public async Task<IActionResult> GetMaintenancesByEquipmentId ([FromQuery]PagedRequestDto paged,int equipmentId)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var maintenance = await _doneMaintenanceQueryService.GetMaintenanceByEquipmentIdAsync(paged,equipmentId);

        return Ok(maintenance);
    }

    
    #endregion

   
}