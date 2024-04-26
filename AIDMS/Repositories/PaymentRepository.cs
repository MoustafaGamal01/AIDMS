using AIDMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIDMS.Repositories;
public class PaymentRepository : IPaymentRepository {
    private readonly AIDMSContextClass _context;

    public PaymentRepository(AIDMSContextClass context) {
        _context = context;
    }
    
    public async Task<Payment> GetPaymentByIdAsync(int paymentId) {
        return await _context.Payments.FirstOrDefaultAsync(i => i.Id == paymentId);
    }

    public async Task<List<Payment>> GetAllPaymetnsAsync() {
        return await _context.Payments.ToListAsync();
    }

    public async Task AddPaymentAsync(Payment payment) {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePaymentAsync(int paymentId, Payment payment) {
        var existingPayment= await GetPaymentByIdAsync(paymentId);
        if (existingPayment == null) {
            throw new InvalidOperationException($"Payment with ID {paymentId} not found.");
        }
        _context.Entry(existingPayment).CurrentValues.SetValues(payment);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePaymentAsync(int paymentId) {
        var paymentToDelete= await GetPaymentByIdAsync(paymentId);
        if (paymentToDelete == null) {
            throw new InvalidOperationException($"Payment with ID {paymentId} not found.");
        }

        _context.Payments.Remove(paymentToDelete);
        await _context.SaveChangesAsync();
    }
}