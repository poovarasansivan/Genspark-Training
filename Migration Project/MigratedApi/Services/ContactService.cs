using MigratedApi.Contexts;
using MigratedApi.Interfaces;
using MigratedApi.Models;
using MigratedApi.Models.Dtos;
using MigratedApi.Repositories;

namespace MigratedApi.Services
{
    public class ContactService : IContactService
    {
        private readonly ContactRepository _contactRepository;
        private readonly ShopDbContext _context;
        public ContactService(ContactRepository contactRepository, ShopDbContext context)
        {
            _contactRepository = contactRepository;
            _context = context;
        }
        public async Task<ContactResponseDto> CreateContactAsync(ContactRequestDto contactRequest)
        {
            if (contactRequest == null)
                throw new ArgumentException("Contact request cannot be null");

            if (string.IsNullOrEmpty(contactRequest.Name) || string.IsNullOrEmpty(contactRequest.Email))
                throw new ArgumentException("Name and Email cannot be null or empty");

            var contact = new Contact
            {
                Name = contactRequest.Name,
                Email = contactRequest.Email,
                Subject = contactRequest.Subject,
                Messgae = contactRequest.Message,
                CreatedAt = contactRequest.CreatedAt
            };
            await _contactRepository.AddAsync(contact);
            return new ContactResponseDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Subject = contact.Subject,
                Message = contact.Messgae,
                CreatedAt = contact.CreatedAt
            };
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Contact ID must be greater than zero");

            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                throw new KeyNotFoundException($"Contact with ID {id} not found");

            await _contactRepository.DeleteAsync(contact.Id);
            return true;
        }

        public async Task<IEnumerable<ContactResponseDto>> GetAllContactsAsync()
        {
            var contacts = await _contactRepository.GetAllAsync();
            if (contacts == null || !contacts.Any())
                return Enumerable.Empty<ContactResponseDto>();
            return contacts.Select(c => new ContactResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Subject = c.Subject,
                Message = c.Messgae,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<ContactResponseDto?> GetContactByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Contact ID must be greater than zero");

            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                return null;

            return new ContactResponseDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Subject = contact.Subject,
                Message = contact.Messgae,
                CreatedAt = contact.CreatedAt
            };
        }

        public async Task<ContactResponseDto?> UpdateContactAsync(int id, ContactRequestDto contactRequest)
        {
            if (id <= 0)
                throw new ArgumentException("Contact ID must be greater than zero");

            if (contactRequest == null)
                throw new ArgumentException("Contact request cannot be null");

            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
                return null;

            contact.Name = contactRequest.Name;
            contact.Email = contactRequest.Email;
            contact.Subject = contactRequest.Subject;
            contact.Messgae = contactRequest.Message;
            contact.CreatedAt = DateTime.UtcNow; 

            await _contactRepository.UpdateAsync(contact);
            return new ContactResponseDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Subject = contact.Subject,
                Message = contact.Messgae,
                CreatedAt = contact.CreatedAt
            };
        }
    }
}