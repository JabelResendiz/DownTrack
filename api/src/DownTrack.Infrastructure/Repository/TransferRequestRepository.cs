using DownTrack.Application.IRepository;
using DownTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace DownTrack.Infrastructure.Repository;

public class TransferRequestRepository : GenericRepository<TransferRequest>, ITransferRequestRepository
{
    public TransferRequestRepository(DownTrackContext context) : base(context) { }

    public async Task<TransferRequest> GetByIdAsync(int TransferId)
    {
        var result = await _entity.FirstOrDefaultAsync(d => d.Id == TransferId);
        if (result == null)
            throw new KeyNotFoundException($"No se encontró una Solicitud de Traslado con el ID '{TransferId}'.");
        return result;
    }

    public async Task DeleteAsync(int TransferId)
    {
        var result = await GetByIdAsync(TransferId);

        _entity.Remove(result);
    }
}