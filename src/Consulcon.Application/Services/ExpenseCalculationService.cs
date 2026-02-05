using Consulcon.Application.Interfaces;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Domain.Entities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Consulcon.Application.Services
{
    public class ExpenseCalculationService : IExpenseCalculationService
    {
        public List<UnitDebtDistribution> CalculateDistribution(
            Egreso egreso,
            List<Propiedad> propiedades,
            bool validarPorcentajeTotal = true)
        {
            // Validaciones básicas
            if (egreso == null)
                throw new ArgumentNullException(nameof(egreso));

            if (propiedades == null || !propiedades.Any())
                throw new ArgumentException("La lista de propiedades no puede estar vacía.");

            if (egreso.MontoTotal <= 0)
                throw new ArgumentException("El monto total del egreso debe ser mayor a cero.");

            // 1. Filtrar unidades activas (Propiedad.Activo es bool?)
            var propiedadesActivas = propiedades.Where(p => p.Activo == true).ToList();

            if (!propiedadesActivas.Any())
                throw new InvalidOperationException("No hay propiedades activas para distribuir el gasto.");

            // 2. Validar porcentajes individuales
            foreach (var prop in propiedadesActivas)
            {
                var porcentaje = prop.PorcentajeParticipacion ?? 0m;
                if (porcentaje < 0)
                    throw new ArgumentException($"El porcentaje no puede ser negativo. Unidad: {prop.CodigoUnidad}");
                if (porcentaje > 100)
                    throw new ArgumentException($"El porcentaje no puede ser mayor a 100%. Unidad: {prop.CodigoUnidad}");
            }

            // 3. Validar suma de porcentajes
            if (validarPorcentajeTotal)
            {
                var totalPercentage = propiedadesActivas.Sum(p => p.PorcentajeParticipacion ?? 0m);
                const decimal tolerance = 0.01m;
                const decimal expected = 100m;

                if (Math.Abs(totalPercentage - expected) > tolerance)
                {
                    throw new InvalidOperationException(
                        $"La suma de porcentajes de participación ({totalPercentage:F2}%) no es 100%. " +
                        $"Diferencia: {Math.Abs(totalPercentage - expected):F2}%.");
                }
            }

            // 4. Calcular distribución
            var distributions = new List<UnitDebtDistribution>();
            decimal distributedTotal = 0m;
            var remainingAmount = egreso.MontoTotal;

            // Ordenar para consistencia determinística
            var orderedProps = propiedadesActivas.OrderBy(p => p.CodigoUnidad).ToList();
            var lastIndex = orderedProps.Count - 1;

            for (int i = 0; i < orderedProps.Count; i++)
            {
                var prop = orderedProps[i];
                decimal calculatedAmount;
                decimal porcentaje = prop.PorcentajeParticipacion ?? 0m;

                if (i == lastIndex)
                {
                    // Última unidad recibe el remanente para asegurar suma exacta
                    calculatedAmount = remainingAmount;
                }
                else
                {
                    // Calcular proporción exacta
                    decimal exactAmount = egreso.MontoTotal * (porcentaje / 100m);
                    
                    // Redondear a 2 decimales (hacia el par más cercano o estándar)
                    calculatedAmount = Math.Round(exactAmount, 2, MidpointRounding.ToEven);
                    
                    // Asegurar no negativo (por si acaso)
                    calculatedAmount = Math.Max(calculatedAmount, 0);
                }

                var dist = new UnitDebtDistribution(
                    egreso.IdEgreso,
                    prop.IdPropiedad,
                    porcentaje,
                    calculatedAmount
                );

                distributions.Add(dist);

                distributedTotal += calculatedAmount;
                remainingAmount = egreso.MontoTotal - distributedTotal;
            }

            // 5. Validación final de integridad (sanity check)
            var finalSum = distributions.Sum(d => d.MontoCalculado);
            if (Math.Abs(finalSum - egreso.MontoTotal) > 0.01m)
            {
               throw new InvalidOperationException(
                   $"Error interno en distribución. Total Gasto: {egreso.MontoTotal}, Distribuido: {finalSum}");
            }

            return distributions;
        }
    }
}
