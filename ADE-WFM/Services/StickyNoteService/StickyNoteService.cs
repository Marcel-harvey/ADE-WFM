using ADE_WFM.Data;
using ADE_WFM.Models;
using ADE_WFM.Models.DTOs;
using ADE_WFM.Models.DTOs.StickyNoteDto;
using ADE_WFM.Services.TenantService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ADE_WFM.Services.StickyNoteService {
    public class StickyNoteService : IStickyNoteService {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StickyNoteService> _logger;
        private readonly TenantContext _tenantContext;

        public StickyNoteService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<StickyNoteService> logger,
            TenantContext tenantContext) {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _tenantContext = tenantContext;
        }


        // ADD services
        public async Task<ServiceResult<StickyNoteResponseDto>> AddStickyNote(CreateStickyNoteDto dto) {
            // General validations
            if (dto == null)
                return ServiceResult<StickyNoteResponseDto>.Failure("No information provided.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                return ServiceResult<StickyNoteResponseDto>.Failure("Title is required.");

            if (string.IsNullOrEmpty(dto.Content))
                return ServiceResult<StickyNoteResponseDto>.Failure("Content is required.");

            try {
                var stickyNote = new StickyNote {
                    Title = dto.Title,
                    Content = dto.Content,
                    UserId = _tenantContext.UserId,
                    TenantId = _tenantContext.TenantId
                };

                _context.StickyNotes.Add(stickyNote);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Sticky note created successfully for user with ID: {UserId}", _tenantContext.UserId);

                return ServiceResult<StickyNoteResponseDto>.Success(
                    new StickyNoteResponseDto {
                        Id = stickyNote.Id,
                        Title = stickyNote.Title,
                        Content = stickyNote.Content
                    },
                    "Sticky note created successfully."
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while adding sticky note to user");
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "A database error occurred while adding the sticky note.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while adding sticky note to user");
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "An unexpected error occurred while adding the sticky note.",
                    new[] { ex.Message });
            }
        }


        // GET services
        // Get all sticky notes related to user
        public async Task<ServiceResult<List<StickyNoteResponseDto>>> GetAllStickyNotes() {
            try {
                var stickyNotes = await _context.StickyNotes
                    .Where(sn => sn.UserId == _tenantContext.UserId && sn.TenantId == _tenantContext.TenantId)
                    .Include(u => u.User)
                    .ToListAsync();
                if (!stickyNotes.Any()) {
                    _logger.LogWarning("No sticky notes found for user with ID: {UserId}", _tenantContext.UserId);
                    return ServiceResult<List<StickyNoteResponseDto>>.Failure("No sticky notes found for user");
                }

                _logger.LogInformation("Retrieved {Count} sticky notes for user with ID: {UserId}", stickyNotes.Count, _tenantContext.UserId);

                return ServiceResult<List<StickyNoteResponseDto>>.Success(
                    stickyNotes.Select(sn => new StickyNoteResponseDto {
                        Id = sn.Id,
                        Title = sn.Title,
                        Content = sn.Content
                    }).ToList(),
                    "Sticky notes retrieved successfully."
                );
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving sticky notes.");

                return ServiceResult<List<StickyNoteResponseDto>>.Failure(
                    "An unexpected error occurred while retrieving sticky notes.",
                    new[] { ex.Message });
            }
        }


        // Get sticky note by ID
        public async Task<ServiceResult<StickyNoteResponseDto>> GetStickyNoteById(GetStickyNoteInfoDto dto) {
            if (dto == null)
                return ServiceResult<StickyNoteResponseDto>.Failure("No information provided.");

            if (dto.StickyNoteId <= 0)
                return ServiceResult<StickyNoteResponseDto>.Failure("No sticky note ID provided");

            try {
                var stickyNote = await _context.StickyNotes
                    .FirstOrDefaultAsync(sn => sn.Id == dto.StickyNoteId && sn.UserId == _tenantContext.UserId && sn.TenantId == _tenantContext.TenantId);
                if (stickyNote == null)
                    return ServiceResult<StickyNoteResponseDto>.Failure("Sticky note not found.");

                _logger.LogInformation("Sticky note with ID {StickyNoteId} retrieved for user with ID: {UserId}", dto.StickyNoteId, _tenantContext.UserId);

                return ServiceResult<StickyNoteResponseDto>.Success(
                        new StickyNoteResponseDto {
                            Id = stickyNote.Id,
                            Title = stickyNote.Title,
                            Content = stickyNote.Content
                        },
                        "Sticky note retreived"
                    );
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving sticky note.");

                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "An unexpected error occurred while retrieving sticky note.",
                    new[] { ex.Message });
            }
        }


        // UPDATE services
        public async Task<ServiceResult<StickyNoteResponseDto>> UpdateStickyNote(GetStickyNoteInfoDto dto) {
            // General validations
            if (dto == null)
                return ServiceResult<StickyNoteResponseDto>.Failure("No information provided.");

            if (string.IsNullOrEmpty(dto.NewContent) && string.IsNullOrWhiteSpace(dto.NewTitle))
                return ServiceResult<StickyNoteResponseDto>.Failure("Updated content is required.");

            if (dto.StickyNoteId <= 0)
                return ServiceResult<StickyNoteResponseDto>.Failure("Valid StickyNoteId is required.");

            try {
                var stickyNote = await _context.StickyNotes
                    .Include(sn => sn.User)
                    .FirstOrDefaultAsync(sn => sn.Id == dto.StickyNoteId && sn.UserId == _tenantContext.UserId && sn.TenantId == _tenantContext.TenantId);
                if (stickyNote == null) {
                    _logger.LogWarning("Sticky note with ID {StickyNoteId} not found for user with ID: {UserId}", dto.StickyNoteId, _tenantContext.UserId);
                    return ServiceResult<StickyNoteResponseDto>.Failure("Sticky note not found.");
                }

                if (dto.NewTitle != null) {
                    stickyNote.Title = dto.NewTitle;
                    _logger.LogInformation("Sticky note title updated for ID {noteId}", dto.StickyNoteId);
                }

                if (dto.NewContent != null) {
                    stickyNote.Content = dto.NewContent;
                    _logger.LogInformation("Sticky note content was update for ID {notedId}", dto.StickyNoteId);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Sticky note with ID {StickyNoteId} updated successfully for user with ID: {UserId}", dto.StickyNoteId, _tenantContext.UserId);

                return ServiceResult<StickyNoteResponseDto>.Success(
                    new StickyNoteResponseDto {
                        Id = stickyNote.Id,
                        Title = stickyNote.Title,
                        Content = stickyNote.Content
                    },
                    "Sticky note updated successfully."
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while updating sticky note");
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "A database error occurred while updating the sticky note.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while updating sticky note");
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "An unexpected error occurred while updating the sticky note.",
                    new[] { ex.Message });
            }
        }

        // DELETE services
        public async Task<ServiceResult<StickyNoteResponseDto>> DeleteStickyNote(GetStickyNoteInfoDto dto) {
            // General validations
            if (dto == null)
                return ServiceResult<StickyNoteResponseDto>.Failure("No information provided.");

            if (dto.StickyNoteId <= 0)
                return ServiceResult<StickyNoteResponseDto>.Failure("Valid ID is required.");

            try {
                var stickyNote = await _context.StickyNotes
                    .Include(sn => sn.User)
                    .FirstOrDefaultAsync(sn => sn.Id == dto.StickyNoteId && sn.UserId == _tenantContext.UserId && sn.TenantId == _tenantContext.TenantId);
                if (stickyNote == null) {
                    _logger.LogWarning("Sticky note with ID {StickyNoteId} not found for user with ID: {UserId}", dto.StickyNoteId, _tenantContext.UserId);
                    return ServiceResult<StickyNoteResponseDto>.Failure("Sticky note not found.");
                }

                _context.StickyNotes.Remove(stickyNote);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Sticky note with ID {StickyNoteId} deleted successfully for user with ID: {UserId}", dto.StickyNoteId, _tenantContext.UserId);

                return ServiceResult<StickyNoteResponseDto>.Success(
                    new StickyNoteResponseDto {
                        Id = stickyNote.Id,
                        Content = stickyNote.Content
                    },
                    "Sticky note deleted successfully."
                );
            }
            catch (DbUpdateException ex) {
                _logger.LogError(ex, "Database error while deleting sticky note");
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "A database error occurred while deleting the sticky note.",
                    new[] { ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Unexpected error while deleting sticky not");
                return ServiceResult<StickyNoteResponseDto>.Failure(
                    "An unexpected error occurred while deleting the sticky note.",
                    new[] { ex.Message });
            }
        }
    }
}
