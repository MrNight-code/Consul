using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Facturacion;

namespace Consulcon.Domain.Specifications;

public class CobranzaWithFiltersSpec : BaseSpecification<TransaccionPago>
{
    public CobranzaWithFiltersSpec(PaginationParams p, int idCondominio) 
        : base(x => 
            x.IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation.IdManzanoNavigation.IdCondominio == idCondominio && 
            
            (string.IsNullOrEmpty(p.SearchTerm) || 
             (x.NroComprobanteBanco != null && x.NroComprobanteBanco.Contains(p.SearchTerm)) ||
             (x.Observaciones != null && x.Observaciones.Contains(p.SearchTerm))) &&
             
            (!p.FromDate.HasValue || (x.FechaPago.HasValue && x.FechaPago >= p.FromDate.Value)) &&
            (!p.ToDate.HasValue || (x.FechaPago.HasValue && x.FechaPago <= p.ToDate.Value)))
    {
        AddInclude(x => x.IdPersonaPagadorNavigation!);
        AddInclude(x => x.IdDeudaNavigation!);
        AddInclude(x => x.IdBancoDestinoNavigation!);
        AddInclude(x => x.IdFormaPagoNavigation!);
        
        AddInclude("IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation");

        ApplyPaging((p.PageNumber - 1) * p.PageSize, p.PageSize);

        if (!string.IsNullOrEmpty(p.SortBy))
        {
            switch (p.SortBy.ToLower())
            {
                case "fecha":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.FechaPago!);
                    else ApplyOrderBy(x => (object)x.FechaPago!);
                    break;
                case "monto":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.MontoAbonado);
                    else ApplyOrderBy(x => (object)x.MontoAbonado);
                    break;
                default:
                    ApplyOrderByDescending(x => (object)x.IdPago);
                    break;
            }
        }
        else
        {
            ApplyOrderByDescending(x => (object)x.FechaPago!);
        }
    }
}