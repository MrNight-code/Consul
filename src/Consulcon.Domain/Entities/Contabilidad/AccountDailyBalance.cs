using Consulcon.Domain.Entities.General;

namespace Consulcon.Domain.Entities.Contabilidad;

public class AccountDailyBalance
{
    public int Id { get; set; }
    
    public int IdBanco { get; set; }
    
    public decimal Balance { get; set; }
    
    public DateTime Date { get; set; }

    public virtual Banco Banco { get; set; } = null!;
}