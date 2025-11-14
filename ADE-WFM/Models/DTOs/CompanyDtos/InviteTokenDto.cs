namespace ADE_WFM.Models.DTOs.CompanyDtos {
    public class InviteTokenDto {
        public int TenantId { get; set; }
        public Guid InviteToken { get; set; }
    }
}
