using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.ProjectDtos;
using ADE_WFM.Models.DTOs.StickyNoteDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.StickyNoteService
{
    public class StickyNoteService : IStickyNoteService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StickyNoteService> _logger;

        public StickyNoteService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<StickyNoteService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }


        // ADD services
        public async Task<ServiceResult<StickyNoteResponseDto>> AddStickyNote(CreateStickyNoteDto dto)
        {
            // General validations
            if (dto == null)
                return ServiceResult<StickyNoteResponseDto>.Failure("Input data is null.");

            if (string.IsNullOrEmpty(dto.UserId))
                return ServiceResult<StickyNoteResponseDto>.Failure("UserId is required.");

            if (string.IsNullOrEmpty(dto.Content))
                return ServiceResult<StickyNoteResponseDto>.Failure("Content is required.");

            var user = await _userManager
                .FindByIdAsync(dto.UserId);
            if (user == null)
                return ServiceResult<StickyNoteResponseDto>.Failure($"User with Id {dto.UserId} not found.");

            try
            {
                var stickyNote = new StickyNote
                {
                    Content = dto.Content,
                    UserId = dto.UserId
                };

                _context.StickyNotes.Add(stickyNote);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Sticky note created successfully for user with ID: {UserId}", dto.UserId);

                return ServiceResult<StickyNoteResponseDto>.Success(
                    new StickyNoteResponseDto
                    {
                        Id = stickyNote.Id,
                        Content = stickyNote.Content
                    },
                    "Sticky note created successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding sticky note to user '{UserName}'", user.UserName);
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "A database error occurred while adding the sticky note.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding sticky note to user '{UserName}'", user.UserName);
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "An unexpected error occurred while adding the sticky note.",
                    new[] { ex.Message });
            }
        }


        // GET services
        // Get all sticky notes related to user
        public async Task<ServiceResult<List<GetStickyNoteResponseDto>>> GetAllStickyNotes(GetAllUserStickyNotesDto dto)
        {
            // General validations
            if (dto == null)
                return ServiceResult<List<GetStickyNoteResponseDto>>.Failure("Input data is null.");

            if (string.IsNullOrEmpty(dto.UserId))
                return ServiceResult<List<GetStickyNoteResponseDto>>.Failure("UserId is required.");

            try
            {
                var stickyNotes = await _context.StickyNotes
                    .Where(sn => sn.UserId == dto.UserId)
                    .Include(u => u.User)
                    .ToListAsync();

                if (!stickyNotes.Any())
                {
                    _logger.LogWarning("No sticky notes found for user with ID: {UserId}", dto.UserId);
                    return ServiceResult<List<GetStickyNoteResponseDto>>.Failure("No sticky notes found for user");
                }
                _logger.LogInformation("Retrieved {Count} sticky notes for user with ID: {UserId}", stickyNotes.Count, dto.UserId);

                return ServiceResult<List<GetStickyNoteResponseDto>>.Success(
                    stickyNotes.Select(sn => new GetStickyNoteResponseDto
                    {
                        Content = sn.Content
                    }).ToList(),
                    "Sticky notes retrieved successfully."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sticky notes.");

                return ServiceResult<List<GetStickyNoteResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving sticky notes.",
                    new[] { ex.Message });
            }
        }


        // UPDATE services
        public async Task<ServiceResult<StickyNoteResponseDto>> UpdateStickyNote(UpdateStickyNoteDto dto)
        {
            // General validations
            if (dto == null)
                return ServiceResult<StickyNoteResponseDto>.Failure("Input data is null.");

            if (string.IsNullOrEmpty(dto.NewContent))
                return ServiceResult<StickyNoteResponseDto>.Failure("Updated content is required.");

            if (dto.StickyNoteId <= 0)
                return ServiceResult<StickyNoteResponseDto>.Failure("Valid StickyNoteId is required.");

            if (string.IsNullOrEmpty(dto.UserId))
                return ServiceResult<StickyNoteResponseDto>.Failure("UserId is required.");

            try
            {
                var stickyNote = await _context.StickyNotes
                    .Include(sn => sn.User)
                    .FirstOrDefaultAsync(sn => sn.Id == dto.StickyNoteId && sn.UserId == dto.UserId);

                if (stickyNote == null)
                {
                    _logger.LogWarning("Sticky note with ID {StickyNoteId} not found for user with ID: {UserId}", dto.StickyNoteId, dto.UserId);
                    return ServiceResult<StickyNoteResponseDto>.Failure("Sticky note not found.");
                }

                stickyNote.Content = dto.NewContent;
                await _context.SaveChangesAsync();

                return ServiceResult<StickyNoteResponseDto>.Success(
                    new StickyNoteResponseDto
                    {
                        Id = stickyNote.Id,
                        Content = stickyNote.Content
                    },
                    "Sticky note updated successfully."
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating sticky note");
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "A database error occurred while updating the sticky note.",
                    new[] { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating sticky not");
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "An unexpected error occurred while updating the sticky note.",
                    new[] { ex.Message });
            }
        }

        // DELETE services
        public async Task DeleteStickyNote(DeleteStickyNoteDto dto)
        {
            var stickyNote = await _context.StickyNotes
                .FirstOrDefaultAsync(snId => snId.Id == dto.StickyNoteId)
                ?? throw new KeyNotFoundException($"Sticky Note with Id {dto.StickyNoteId} not found.");

            _context.StickyNotes.Remove(stickyNote);
            await _context.SaveChangesAsync();
        }
    }
}
