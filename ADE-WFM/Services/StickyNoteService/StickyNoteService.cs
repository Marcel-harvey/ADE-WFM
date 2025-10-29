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
        public async Task AddStickyNote(CreateStickyNoteDto dto)
        {
            var newStickyNote = new StickyNote
            {
                Content = dto.Content,
                UserId = dto.UserId
            };

            _context.StickyNotes.Add(newStickyNote);
            await _context.SaveChangesAsync();
        }


        // GET services
        // Get all sticky notes related to user
        public async Task<ServiceResult<List<GetStickyNoteResponseDto>>> GetAllStickyNotes(GetAllUserStickyNotesDto dto)
        {
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
        public async Task UpdateStickyNote(UpdateStickyNoteDto dto)
        {
            var stickyNote = await _context.StickyNotes
                .FirstOrDefaultAsync(snId => snId.Id == dto.StickyNoteId)
                ?? throw new KeyNotFoundException($"Sticky Note with Id {dto.StickyNoteId} not found.");

            stickyNote.Content = dto.NewContent;
            await _context.SaveChangesAsync();
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
