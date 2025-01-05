using AutoMapper;
using DownTrack.Application.DTO;
using DownTrack.Application.IServices;
using DownTrack.Application.IUnitOfWorkPattern;
using DownTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DownTrack.Application.Services;


/// <summary>
/// Service class for handling business logic related to departments.
/// Interacts with repositories and uses DTOs for client communication.
/// </summary>
public class DepartmentServices : IDepartmentServices
{
    // Automapper instance for mapping between domain entities and DTOs.
    private readonly IMapper _mapper;

    // Unit of Work instance for managing repositories and transactions.
    private readonly IUnitOfWork _unitOfWork;


    public DepartmentServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }



    /// <summary>
    /// Creates a new department based on the provided DTO.
    /// </summary>
    /// <param name="dto">The DepartmentDto containing the department details to create.</param>
    /// <returns>The created DepartmentDto.</returns>
    public async Task<DepartmentDto> CreateAsync(DepartmentDto dto)
    {

        //Maps DTO to domain entity.

        var department = _mapper.Map<Department>(dto);
       
        //Adds the new department to the repository.
        await _unitOfWork.GetRepository<Department>().CreateAsync(department);

        //Commits the transaction.
        await _unitOfWork.CompleteAsync();

        // Maps the created entity back to DTO.
        return _mapper.Map<DepartmentDto>(department);

    }

    public async Task DeleteAsync(int departmentId, int sectionId)
    {
        var existingDepartment = await _unitOfWork.GetRepository<Department>().GetByIdAndSectionIdAsync(departmentId, sectionId);

        if (existingDepartment == null)
        {
            throw new ConflictException($"Department with ID '{departmentId}' in section '{sectionId}' does not exist.");
        }

        await _unitOfWork.GetRepository<Department>().DeleteAsync(existingDepartment);
    }



    /// <summary>
    /// Deletes a department by its ID.
    /// </summary>
    /// <param name="dto">The ID of the department to delete.</param>
    public async Task DeleteAsync(int dto)
    {   
        // Removes the department by its ID
        await _unitOfWork.GetRepository<Department>().DeleteByIdAsync(dto);

        await _unitOfWork.CompleteAsync(); // Commits the transaction.
    }



    /// <summary>
    /// Retrieves a list of all departments.
    /// </summary>
    /// <returns>A collection of DepartmentDto representing all departments.</returns>
    public async Task<IEnumerable<DepartmentDto>> ListAsync()
    {
        var department = await _unitOfWork.GetRepository<Department>().GetAllAsync().ToListAsync();

        return department.Select(_mapper.Map<DepartmentDto>);
    }



    /// <summary>
    /// Updates an existing department's information.
    /// </summary>
    /// <param name="dto">The DepartmentDto containing updated details.</param>
    /// <returns>The updated DepartmentDto.</returns>
    public async Task<DepartmentDto> UpdateAsync(DepartmentDto dto)
    {

        var existingDepartment = await _unitOfWork.GetRepository<Department>().GetByIdAndSectionIdAsync(dto.Id, dto.SectionId);

        if (existingDepartment == null)
        {
            throw new ConflictException($"Department with ID '{dto.Id}' in section '{dto.SectionId}' does not exist.");
        }


        _mapper.Map(dto, department);

        _unitOfWork.GetRepository<Department>().Update(department);

        await _unitOfWork.CompleteAsync();

        return _mapper.Map<DepartmentDto>(department);

    }



    /// <summary>
    /// Retrieves a department by its ID.
    /// </summary>
    /// <param name="departmentDto">The ID of the department to retrieve.</param>
    /// <returns>The DepartmentDto of the retrieved department.</returns>
    public async Task<DepartmentDto> GetByIdAsync(int departmentDto)
    {
        var result = await _unitOfWork.GetRepository<Department>().GetByIdAsync(departmentDto);
        
        return _mapper.Map<DepartmentDto>(result);

    }



}