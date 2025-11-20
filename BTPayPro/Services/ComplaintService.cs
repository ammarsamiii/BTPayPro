
using BTPayPro.Domaine;
using BTPayPro.Interfaces;


namespace BTPayPro.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly IRepositories<Complaint> _complaintRepository;

        public ComplaintService(IRepositories<Complaint> complaintRepository)
        {
            _complaintRepository = complaintRepository;
        }

        public async Task<IEnumerable<Complaint>> GetAllComplaintsAsync()
        {
            return await _complaintRepository.GetAllAsync();
        }

        public async Task<Complaint> GetComplaintByIdAsync(string id)
        {
            return await _complaintRepository.GetByIdAsync(id);
        }

        public async Task AddComplaintAsync(Complaint complaint)
        {
            await _complaintRepository.AddAsync(complaint);
        }

        public async Task UpdateComplaintAsync(Complaint complaint)
        {
            _complaintRepository.Update(complaint);
        }

        public async Task DeleteComplaintAsync(string id)
        {
            var complaint = await _complaintRepository.GetByIdAsync(id);
            if (complaint != null)
            {
                _complaintRepository.Remove(complaint);
            }
        }
    }
}