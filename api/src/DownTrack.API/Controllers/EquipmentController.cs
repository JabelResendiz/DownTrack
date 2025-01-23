using System.Security.Claims;
using DownTrack.Application.DTO;
using DownTrack.Application.DTO.Paged;
using DownTrack.Application.IServices;
using DownTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DownTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentServices _equipmentService;

    public EquipmentController(IEquipmentServices equipmentServices)
    {
        _equipmentService = equipmentServices;
    }

    [HttpPost]
    [Route("POST")]
    public async Task<IActionResult> CreateEquipment(EquipmentDto equipment)
    {
        // Obtener el claim "role"
        // var roleClaim = User?.FindFirst(ClaimTypes.Role);  // ClaimTypes.Role es el nombre estándar para el claim de rol

        // if(roleClaim == null)
        // {
        //     Console.WriteLine("es null");
        //     throw new Exception();
        // }    
        
        // Console.WriteLine(roleClaim.Value);
        
        // if (roleClaim == null || roleClaim.Value != "Technician")
        // {
        //     return Unauthorized();  // Si el claim "role" no es igual a "Technician", se deniega el acceso
        // }
        await _equipmentService.CreateAsync(equipment);

        return Ok("Equipment added successfully");
    }

    [HttpGet]
    [Route("GET_ALL")]

    public async Task<ActionResult<IEnumerable<Equipment>>> GetAllEquipment()
    {
        var results = await _equipmentService.ListAsync();

        return Ok(results);

    }

    [HttpGet]
    [Route("GET")]

    public async Task<ActionResult<Equipment>> GetUserById(int equipmentId)
    {
        var result = await _equipmentService.GetByIdAsync(equipmentId);

        if (result == null)
            return NotFound($"Equipment with ID {equipmentId} not found");

        return Ok(result);

    }

    [HttpGet]
    [Route("GetPaged")]

    public async Task<IActionResult> GetPagedEquipment ([FromQuery]PagedRequestDto paged)
    {
        paged.BaseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

        var result = await _equipmentService.GetPagedResultAsync(paged);
        
        return Ok (result);
        
    }

    [HttpPut]
    [Route("PUT")]

    public async Task<IActionResult> UpdateEquipment(EquipmentDto equipment)
    {
        var result = await _equipmentService.UpdateAsync(equipment);
        return Ok(result);
    }

    [HttpDelete]
    [Route("{equipmentId}")]

    public async Task<IActionResult> DeleteEquipment(int equipmentId)
    {
        await _equipmentService.DeleteAsync(equipmentId);

        return Ok("Equipment deleted successfully");
    }
}