using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Contabilidad;

namespace Consulcon.Domain.Specifications;

public class EgresoWithFiltersSpec : BaseSpecification<Egreso>
{
    public EgresoWithFiltersSpec(PaginationParams p, int idCondominio) 
        : base(x => 
            x.IdCondominio == idCondominio && 
            
            (string.IsNullOrEmpty(p.SearchTerm) || 
             (x.Concepto != null && x.Concepto.Contains(p.SearchTerm)) || 
             (x.NroFacturaProveedor != null && x.NroFacturaProveedor.Contains(p.SearchTerm))) &&
             
            (!p.FromDate.HasValue || (x.FechaEgreso.HasValue && x.FechaEgreso >= p.FromDate.Value)) &&
            (!p.ToDate.HasValue || (x.FechaEgreso.HasValue && x.FechaEgreso <= p.ToDate.Value)))
    {
        AddInclude(x => x.IdProveedorNavigation!);
        AddInclude(x => x.IdBancoOrigenNavigation); 
        AddInclude(x => x.IdFormaPagoNavigation);   

        ApplyPaging((p.PageNumber - 1) * p.PageSize, p.PageSize);

        if (!string.IsNullOrEmpty(p.SortBy))
        {
            switch (p.SortBy.ToLower())
            {
                case "fecha":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.FechaEgreso!);
                    else ApplyOrderBy(x => (object)x.FechaEgreso!);
                    break;
                case "monto":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.MontoTotal);
                    else ApplyOrderBy(x => (object)x.MontoTotal);
                    break;
                default:
                    ApplyOrderByDescending(x => (object)x.FechaEgreso!);
                    break;
            }
        }
        else
        {
            ApplyOrderByDescending(x => (object)x.FechaEgreso!);
        }
    }
}