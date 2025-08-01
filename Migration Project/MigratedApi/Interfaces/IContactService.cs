using MigratedApi.Models.Dtos;

namespace MigratedApi.Interfaces
{
    public interface IContactService
    {
        Task<ContactResponseDto> CreateContactAsync(ContactRequestDto contactRequest);
        Task<IEnumerable<ContactResponseDto>> GetAllContactsAsync();
        Task<ContactResponseDto?> GetContactByIdAsync(int id);
        Task<bool> DeleteContactAsync(int id);
        Task<ContactResponseDto?> UpdateContactAsync(int id, ContactRequestDto contactRequest);
    }
}