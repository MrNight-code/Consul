using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Facturacion;
using System;
using System.Linq;

namespace Consulcon.Domain.Specifications;

/// <summary>
/// Especificación para filtrar recibos generados (TransaccionPago) siguiendo el estándar de PaginationParams.
/// </summary>
public class ReceiptWithFiltersSpec : BaseSpecification<TransaccionPago>
{
    public ReceiptWithFiltersSpec(PaginationParams p, string? medio = null, int? propiedadId = null) 
        : base(x => 
            (string.IsNullOrEmpty(p.SearchTerm) || 
             (x.IdPersonaPagadorNavigation != null && x.IdPersonaPagadorNavigation.NombreCompleto.ToLower().Contains(p.SearchTerm.ToLower())) ||
             (x.IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation.CodigoUnidad != null && 
              x.IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation.CodigoUnidad.ToLower().Contains(p.SearchTerm.ToLower()))) &&
             
            (!p.FromDate.HasValue || (x.FechaPago >= p.FromDate.Value || x.FechaRecibo >= p.FromDate.Value)) &&
            (!p.ToDate.HasValue || (x.FechaPago < p.ToDate.Value.AddDays(1) || x.FechaRecibo < p.ToDate.Value.AddDays(1))) &&
            
            (string.IsNullOrEmpty(medio) || medio == "Todos" || (x.IdFormaPagoNavigation != null && x.IdFormaPagoNavigation.Descripcion == medio)) &&
            (!propiedadId.HasValue || x.IdDeudaNavigation.IdContratoNavigation.IdPropiedad == propiedadId.Value)
        )
    {
        // Navegaciones necesarias para el DTO
        AddInclude(x => x.IdPersonaPagadorNavigation!);
        AddInclude(x => x.IdFormaPagoNavigation!);
        AddInclude(x => x.IdDeudaNavigation!);
        AddInclude("IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation");

        // Paginación estándar
        ApplyPaging((p.PageNumber - 1) * p.PageSize, p.PageSize);

        // Ordenamiento dinámico
        if (!string.IsNullOrEmpty(p.SortBy))
        {
            switch (p.SortBy.ToLower())
            {
                case "fechapago":
                case "fecha":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.FechaPago!);
                    else ApplyOrderBy(x => (object)x.FechaPago!);
                    break;
                case "monto":
                case "abonado":
                    if (p.SortDescending) ApplyOrderByDescending(x => (object)x.MontoAbonado);
                    else ApplyOrderBy(x => (object)x.MontoAbonado);
                    break;
                default:
                    ApplyOrderByDescending(x => (object)x.FechaPago!);
                    break;
            }
        }
        else
        {
            // Orden por defecto: Más reciente primero
            ApplyOrderByDescending(x => (object)x.FechaPago!);
        }
    }
}
