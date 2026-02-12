using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Consulcon.API.Swagger;

/// <summary>
/// Adds X-Condominio-Id header parameter to all Swagger operations.
/// </summary>
public class TenantHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Condominio-Id",
            In = ParameterLocation.Header,
            Required = true,
            Description = "Condominio ID (integer, e.g., 1)",
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Default = new Microsoft.OpenApi.Any.OpenApiInteger(1)
            }
        });
    }
}
