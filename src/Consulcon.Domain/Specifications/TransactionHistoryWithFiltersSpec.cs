using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Contabilidad;

namespace Consulcon.Domain.Specifications;

public class TransactionHistoryWithFiltersSpec : BaseSpecification<AccountTransactionHistory>
{
    public TransactionHistoryWithFiltersSpec(PaginationParams p, int accountId) 
        : base(x => 
            x.AccountId == accountId && 
            
            (string.IsNullOrEmpty(p.SearchTerm) || 
             (x.Description != null && x.Description.Contains(p.SearchTerm)) ||
             (x.ReferenceId != null && x.ReferenceId.Contains(p.SearchTerm))) &&
             
            (!p.FromDate.HasValue || x.Date >= p.FromDate.Value) &&
            (!p.ToDate.HasValue || x.Date <= p.ToDate.Value))
    {
        AddInclude(x => x.Account!);
        AddInclude(x => x.Expense!);

        ApplyPaging((p.PageNumber - 1) * p.PageSize, p.PageSize);

        if (!string.IsNullOrEmpty(p.SortBy))
        {
            switch (p.SortBy.ToLower())
            {
                case "fecha":
                case "date":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.Date);
                    else ApplyOrderBy(x => (object)x.Date);
                    break;
                case "monto":
                case "amount":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.Amount);
                    else ApplyOrderBy(x => (object)x.Amount);
                    break;
                default:
                    ApplyOrderByDescending(x => (object)x.Date);
                    break;
            }
        }
        else
        {
            ApplyOrderByDescending(x => (object)x.Date);
        }
    }
}