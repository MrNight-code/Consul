namespace Consulcon.Application.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string? AccountNumber { get; set; }
        public bool IsActive { get; set; }
        public decimal Balance { get; set; }
    }
}
