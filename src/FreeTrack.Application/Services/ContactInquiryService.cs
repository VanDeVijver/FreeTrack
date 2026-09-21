using FreeTrack.Application.DTOs;
using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Entities;

namespace FreeTrack.Application.Services;

public class ContactInquiryService : IContactInquiryService
{
    private readonly IContactInquiryRepository _repository;

    public ContactInquiryService(IContactInquiryRepository repository) => _repository = repository;

    public async Task<ContactInquiryDto> SubmitAsync(CreateContactInquiryDto input, CancellationToken ct = default)
    {
        var inquiry = new ContactInquiry
        {
            Name = input.Name,
            Email = input.Email,
            Phone = input.Phone,
            Message = input.Message,
            RoomId = input.RoomId,
            StudioServiceId = input.StudioServiceId,
            PreferredDate = input.PreferredDate
        };

        await _repository.AddAsync(inquiry, ct);
        await _repository.SaveChangesAsync(ct);

        // TODO: hook up IEmailSender here to notify staff + send a confirmation to the visitor.

        return new ContactInquiryDto(inquiry.Id, inquiry.Name, inquiry.Email, inquiry.Phone,
            inquiry.Message, inquiry.RoomId, inquiry.StudioServiceId, inquiry.PreferredDate,
            inquiry.Status.ToString(), inquiry.CreatedAt);
    }
}
