
using BTPayPro.Domaine;


namespace BTPayPro.Services
{
    public interface IComplaintService
    {
        Task<IEnumerable<Complaint>> GetAllComplaintsAsync();
        Task<Complaint> GetComplaintByIdAsync(string id);
        Task AddComplaintAsync(Complaint complaint);
        Task UpdateComplaintAsync(Complaint complaint);
        Task DeleteComplaintAsync(string id);
    }
}