using AIDMS.Entities;

namespace AIDMS.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<Payment> GetByPaymentByNameAsync(string paymentName);
        Task<List<Payment>> GetAllPaymetnsAsync();
        Task AddPaymentAsync(Payment payment);
        Task UpdatePaymentAsync(int paymentId, Payment payment);
        Task DeletePaymentAsync(int paymentId);
    }
}
