
using DownTrack.Application.DTO;
using DownTrack.Application.DTO.Paged;
using DownTrack.Application.IServices;
using DownTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DownTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TechnicianNewController : ControllerBase
{
    private readonly ITechnicianQueryServices _technicianQueryService;
    private readonly ITechnicianCommandServices _technicianCommandService;

    public TechnicianNewController(ITechnicianQueryServices technicianQueryServices,
                                   ITechnicianCommandServices technicianCommandServices)
    {
        _technicianQueryService = technicianQueryServices;
        _technicianCommandService = technicianCommandServices;
    }


    [HttpGet]
    [Route("GET")]

    public async Task<ActionResult<TechnicianDto>> GetTechnicianById(int technicianId)
    {
        var result = await _technicianQueryService.GetByIdAsync(technicianId);

        if (result == null)
            return NotFound($"Technician with ID {technicianId} not found");

        return Ok(result);

    }

    [HttpGet]
    [Route("GetPaged")]

    public async Task<IActionResult> GetPagedTechnician ([FromQuery]PagedRequestDto paged)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _technicianQueryService.GetPagedResultAsync(paged);
        
        return Ok (result);
        
    }




}

