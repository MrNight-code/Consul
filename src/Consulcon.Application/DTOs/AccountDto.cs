namespace Consulcon.Application.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "BANCO"; // 'BANCO', 'EFECTIVO'
        public string? AccountNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
