using FreeTrack.Application.DTOs;

namespace FreeTrack.Application.Interfaces;

public interface IContactInquiryService
{
    Task<ContactInquiryDto> SubmitAsync(CreateContactInquiryDto input, CancellationToken ct = default);
}
