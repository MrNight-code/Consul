using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Inmuebles; 

namespace Consulcon.Domain.Specifications;

public class PropiedadWithFiltersSpec : BaseSpecification<Propiedad>
{
    public PropiedadWithFiltersSpec(PaginationParams p, int idCondominio) 
        : base(x => 
            x.IdManzanoNavigation.IdCondominio == idCondominio && 
            
            (string.IsNullOrEmpty(p.SearchTerm) || 
             (x.CodigoUnidad != null && x.CodigoUnidad.Contains(p.SearchTerm)) ||
             (x.NombreFuncional != null && x.NombreFuncional.Contains(p.SearchTerm))))
    {
        AddInclude(x => x.IdManzanoNavigation!);

        ApplyPaging((p.PageNumber - 1) * p.PageSize, p.PageSize);

        if (!string.IsNullOrEmpty(p.SortBy))
        {
            switch (p.SortBy.ToLower())
            {
                case "codigo":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.CodigoUnidad);
                    else ApplyOrderBy(x => (object)x.CodigoUnidad);
                    break;
                case "saldo":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.SaldoDeudor);
                    else ApplyOrderBy(x => (object)x.SaldoDeudor);
                    break;
                default:
                    ApplyOrderBy(x => (object)x.CodigoUnidad); 
                    break;
            }
        }
        else
        {
            ApplyOrderBy(x => (object)x.CodigoUnidad);
        }
    }
}