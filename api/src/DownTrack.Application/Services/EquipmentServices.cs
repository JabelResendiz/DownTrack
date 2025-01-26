using AutoMapper;
using DownTrack.Application.DTO;
using DownTrack.Application.DTO.Paged;
using DownTrack.Application.IServices;
using DownTrack.Application.IUnitOfWorkPattern;
using DownTrack.Domain.Entities;
using DownTrack.Domain.Status;
using Microsoft.EntityFrameworkCore;
using DownTrack.Application.IRepository;
using System.Linq.Expressions;

namespace DownTrack.Application.Services;

public class EquipmentServices : IEquipmentServices
{
    private readonly IMapper _mapper;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IGenericRepository<Section> _sectionRepository;

    private readonly IGenericRepository<Department> _departmentRepository;

    private readonly IGenericRepository<Equipment> _equipmentRepository;

    public EquipmentServices(IUnitOfWork unitOfWork, IMapper mapper, IGenericRepository<Section> sectionRepository,
    IGenericRepository<Department> departmentRepository, IGenericRepository<Equipment> equipmentRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _sectionRepository = sectionRepository;
        _departmentRepository = departmentRepository;
        _equipmentRepository = equipmentRepository;

    }

    public async Task<EquipmentDto> CreateAsync(EquipmentDto dto)
    {
        var equipment = _mapper.Map<Equipment>(dto);

        var department = await _unitOfWork.DepartmentRepository
                                  .GetByIdAsync(equipment.DepartmentId);

        if (department.SectionId != dto.SectionId)
            throw new Exception($"Department with Id: {department.SectionId} not exist in Section with Id : {dto.SectionId}");


        if (!EquipmentStatusHelper.IsValidStatus(equipment.Status))
            throw new Exception("Invalid status");

        await _unitOfWork.GetRepository<Equipment>().CreateAsync(equipment);

        await _unitOfWork.CompleteAsync();

        return _mapper.Map<EquipmentDto>(equipment);
    }

    public async Task DeleteAsync(int dto)
    {
        await _unitOfWork.GetRepository<Equipment>().DeleteByIdAsync(dto);

        await _unitOfWork.CompleteAsync();

    }

    public async Task<IEnumerable<EquipmentDto>> ListAsync()
    {
        var equipment = await _unitOfWork.GetRepository<Equipment>().GetAll().ToListAsync();

        return equipment.Select(_mapper.Map<EquipmentDto>);
    }

    public async Task<EquipmentDto> UpdateAsync(EquipmentDto dto)
    {
        var equipment = await _unitOfWork.GetRepository<Equipment>().GetByIdAsync(dto.Id);

        _mapper.Map(dto, equipment);

        _unitOfWork.GetRepository<Equipment>().Update(equipment);

        await _unitOfWork.CompleteAsync();

        return _mapper.Map<EquipmentDto>(equipment);
    }

    /// <summary>
    /// Retrieves an equipment by their ID
    /// </summary>
    /// <param name="equipmentDto">The equipment's ID to retrieve</param>
    /// <returns>A Task representing the asynchronous operation that fetches the equipment</returns>
    public async Task<EquipmentDto> GetByIdAsync(int equipmentDto)
    {
        var result = await _unitOfWork.GetRepository<Equipment>()
                                      .GetByIdAsync(equipmentDto, default, e => e.Department);

        /// and returns the updated equipment as an EquipmentDto.
        return _mapper.Map<EquipmentDto>(result);

    }



    public async Task<PagedResultDto<EquipmentDto>> GetPagedResultAsync(PagedRequestDto paged)
    {
        //The queryable collection of entities to paginate
        IQueryable<Equipment> queryEquipment = _unitOfWork.GetRepository<Equipment>()
                                                          .GetAll()
                                                          .Include(e => e.Department);

        var totalCount = await queryEquipment.CountAsync();

        var items = await queryEquipment // Apply pagination to the query.
                        .Skip((paged.PageNumber - 1) * paged.PageSize) // Skip the appropriate number of items based on the current page
                        .Take(paged.PageSize) // Take only the number of items specified by the page size.
                        .ToListAsync(); // Convert the result to a list asynchronously.


        return new PagedResultDto<EquipmentDto>
        {
            Items = items?.Select(_mapper.Map<EquipmentDto>) ?? Enumerable.Empty<EquipmentDto>(),
            TotalCount = totalCount,
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            NextPageUrl = paged.PageNumber * paged.PageSize < totalCount
                        ? $"{paged.BaseUrl}?pageNumber={paged.PageNumber + 1}&pageSize={paged.PageSize}"
                        : null,
            PreviousPageUrl = paged.PageNumber > 1
                        ? $"{paged.BaseUrl}?pageNumber={paged.PageNumber - 1}&pageSize={paged.PageSize}"
                        : null

        };
    }











    //query for the section manager 
    public async Task<PagedResultDto<Equipment>> GetPagedEquipmentsBySectionManagerIdAsync(
        int sectionManagerId,
        PagedRequestDto pagedRequest)
    {
        // Crear el filtro para las secciones del jefe de sección
        var sectionParameter = Expression.Parameter(typeof(Section), "section");
        var sectionBody = Expression.Equal(
            Expression.Property(sectionParameter, "SectionManagerId"),
            Expression.Constant(sectionManagerId)
        );
        var sectionFilter = Expression.Lambda<Func<Section, bool>>(sectionBody, sectionParameter);

        // Obtener las secciones gestionadas por el jefe de sección
        var sectionIds = _sectionRepository
            .GetAllByItems(new[] { sectionFilter })
            .Select(s => s.Id);

        // Crear el filtro para los departamentos pertenecientes a esas secciones
        var departmentParameter = Expression.Parameter(typeof(Department), "department");
        var departmentBody = Expression.Call(
            Expression.Constant(sectionIds),
            typeof(Enumerable).GetMethod("Contains", new[] { typeof(int) })!,
            Expression.Property(departmentParameter, "SectionId")
        );
        var departmentFilter = Expression.Lambda<Func<Department, bool>>(departmentBody, departmentParameter);

        // Obtener los departamentos asociados a esas secciones
        var departmentIds = _departmentRepository
            .GetAllByItems(new[] { departmentFilter })
            .Select(d => d.Id);

        // Crear el filtro para los equipos pertenecientes a esos departamentos
        var equipmentParameter = Expression.Parameter(typeof(Equipment), "equipment");
        var equipmentBody = Expression.Call(
            Expression.Constant(departmentIds),
            typeof(Enumerable).GetMethod("Contains", new[] { typeof(int) })!,
            Expression.Property(equipmentParameter, "DepartmentId")
        );
        var equipmentFilter = Expression.Lambda<Func<Equipment, bool>>(equipmentBody, equipmentParameter);

        // Aplicar el filtro al repositorio
        var query = _equipmentRepository.GetAllByItems(new[] { equipmentFilter });

        // Obtener el número total de registros
        var totalRecords = query.Count();

        // Aplicar paginación
        var pagedItems = await query
            .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
            .Take(pagedRequest.PageSize)
            .ToListAsync();

        // Construir URLs para las páginas siguiente y anterior
        var nextPageUrl = totalRecords > pagedRequest.PageNumber * pagedRequest.PageSize
            ? $"{pagedRequest.BaseUrl}?pageNumber={pagedRequest.PageNumber + 1}&pageSize={pagedRequest.PageSize}"
            : null;

        var previousPageUrl = pagedRequest.PageNumber > 1
            ? $"{pagedRequest.BaseUrl}?pageNumber={pagedRequest.PageNumber - 1}&pageSize={pagedRequest.PageSize}"
            : null;

        // Crear el resultado paginado
        var result = new PagedResultDto<Equipment>
        {
            Items = pagedItems,
            TotalCount = totalRecords,
            PageNumber = pagedRequest.PageNumber,
            PageSize = pagedRequest.PageSize,
            NextPageUrl = nextPageUrl,
            PreviousPageUrl = previousPageUrl
        };

        return result;
    }

}