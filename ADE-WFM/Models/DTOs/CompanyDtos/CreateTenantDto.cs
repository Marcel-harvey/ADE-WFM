namespace ADE_WFM.Models.DTOs.CompanyDtos
{
    public class CreateTenantDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? ConnetctionString { get; set; }
        public string? Domain { get; set; }
    }
}
