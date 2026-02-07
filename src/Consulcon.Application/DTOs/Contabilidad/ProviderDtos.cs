namespace Consulcon.Application.DTOs.Contabilidad;

/// <summary>
/// DTO para respuesta de proveedor
/// </summary>
public record ProviderDto
{
    public int Id { get; init; }
    public string TaxId { get; init; } = null!;        // Mapea a: nit
    public string LegalName { get; init; } = null!;    // Mapea a: razon_social
    public string? PhoneNumber { get; init; }          // Mapea a: contacto
    public string? Email { get; init; }                // Campo nuevo (si existe en BD legacy)
    public string? Address { get; init; }              // Mapea a: direccion
    public bool IsActive { get; init; }                // Mapea a: activo
}

/// <summary>
/// DTO para crear un nuevo proveedor
/// </summary>
public record CreateProviderDto
{
    public string TaxId { get; init; } = null!;        // NIT - solo números
    public string LegalName { get; init; } = null!;    // Razón Social
    public string? PhoneNumber { get; init; }          // Teléfono de contacto
    public string? Email { get; init; }                // Email (opcional)
    public string? Address { get; init; }              // Dirección
}

/// <summary>
/// DTO para actualizar un proveedor existente
/// </summary>
public record UpdateProviderDto
{
    public string LegalName { get; init; } = null!;    // Razón Social
    public string? PhoneNumber { get; init; }          // Teléfono de contacto
    public string? Email { get; init; }                // Email (opcional)
    public string? Address { get; init; }              // Dirección
}
