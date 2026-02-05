using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Domain.Entities.Inmuebles;
using System.Collections.Generic;

namespace Consulcon.Application.Interfaces
{
    public interface IExpenseCalculationService
    {
        List<UnitDebtDistribution> CalculateDistribution(
            Egreso egreso,
            List<Propiedad> propiedades,
            bool validarPorcentajeTotal = true);
    }
}
