
namespace DownTrack.Application.DTO;

public class TransferDto
{
    public int Id {get;set;}
    public int RequestId { get; set; }
    public int ShippingSupervisorId { get; set; }
    public int EquipmentReceptorId { get; set; }
    public DateTime Date { get; set; }

}

