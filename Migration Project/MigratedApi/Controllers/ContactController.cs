using MigratedApi.Models.Dtos;
using MigratedApi.Interfaces;
using MigratedApi.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MigratedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateContact([FromBody] ContactRequestDto contactRequest)
        {
            try
            {
                var createdContact = await _contactService.CreateContactAsync(contactRequest);
                return Ok(SuccessResponseHandler.Success(createdContact, "Contact created successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            try
            {
                var contacts = await _contactService.GetAllContactsAsync();
                return Ok(SuccessResponseHandler.Success(contacts, "Contacts retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            try
            {
                var contact = await _contactService.GetContactByIdAsync(id);
                if (contact == null)
                {
                    return NotFound($"Contact with id {id} not found.");
                }
                return Ok(SuccessResponseHandler.Success(contact, "Contact retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                var result = await _contactService.DeleteContactAsync(id);
                return Ok(SuccessResponseHandler.Success(result, "Contact deleted successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactRequestDto contactRequest)
        {
            try
            {
                var updatedContact = await _contactService.UpdateContactAsync(id, contactRequest);
                if (updatedContact == null)
                {
                    return NotFound($"Contact with id {id} not found.");
                }
                return Ok(SuccessResponseHandler.Success(updatedContact, "Contact updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}