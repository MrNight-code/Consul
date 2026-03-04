using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.General;

namespace Consulcon.Domain.Specifications;

public class ProveedorWithFiltersSpec : BaseSpecification<Proveedor>
{
    public ProveedorWithFiltersSpec(PaginationParams p) 
        : base(x => string.IsNullOrEmpty(p.SearchTerm) || 
                    (x.RazonSocial != null && x.RazonSocial.Contains(p.SearchTerm)) || 
                    (x.Nit != null && x.Nit.Contains(p.SearchTerm)))
    {
        ApplyPaging((p.PageNumber - 1) * p.PageSize, p.PageSize);

        if (!string.IsNullOrEmpty(p.SortBy))
        {
            switch (p.SortBy.ToLower())
            {
                case "razonsocial":
                    if (p.SortDescending) 
                        ApplyOrderByDescending(x => x.RazonSocial!); 
                    else 
                        ApplyOrderBy(x => x.RazonSocial!);
                    break;
                case "nit":
                    if (p.SortDescending) 
                        ApplyOrderByDescending(x => x.Nit!); 
                    else 
                        ApplyOrderBy(x => x.Nit!);
                    break;
                default:
                    ApplyOrderByDescending(x => x.IdProveedor);
                    break;
            }
        }
        else
        {
            ApplyOrderByDescending(x => x.IdProveedor);
        }
    }
}