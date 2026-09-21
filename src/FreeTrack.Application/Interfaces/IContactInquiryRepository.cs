using FreeTrack.Domain.Entities;

namespace FreeTrack.Application.Interfaces;

public interface IContactInquiryRepository
{
    Task AddAsync(ContactInquiry inquiry, CancellationToken ct = default);
    Task<IReadOnlyList<ContactInquiry>> GetAllAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
