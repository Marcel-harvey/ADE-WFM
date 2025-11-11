namespace ADE_WFM.Models.DTOs.CompanyDtos
{
    public class TenantResponseDto
    {
        public int TenantId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
