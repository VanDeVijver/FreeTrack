using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Entities;
using FreeTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FreeTrack.Infrastructure.Repositories;

public class ContactInquiryRepository : IContactInquiryRepository
{
    private readonly AppDbContext _db;
    public ContactInquiryRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(ContactInquiry inquiry, CancellationToken ct = default) =>
        await _db.ContactInquiries.AddAsync(inquiry, ct);

    public Task<IReadOnlyList<ContactInquiry>> GetAllAsync(CancellationToken ct = default) =>
        _db.ContactInquiries.AsNoTracking().OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct).ContinueWith(t => (IReadOnlyList<ContactInquiry>)t.Result, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
