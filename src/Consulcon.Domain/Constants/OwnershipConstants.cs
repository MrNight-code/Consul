namespace Consulcon.Domain.Constants;

/// <summary>
/// Constants for ownership-related operations to avoid hardcoding strings.
/// </summary>
public static class OwnershipConstants
{
    /// <summary>
    /// Role for property owner/titleholder in a contract.
    /// </summary>
    public const string RolTitular = "PROPIETARIO";

    /// <summary>
    /// Role for tenant in a contract.
    /// </summary>
    public const string RolInquilino = "INQUILINO";

    /// <summary>
    /// Role for guarantor in a contract.
    /// </summary>
    public const string RolGarante = "GARANTE";

    /// <summary>
    /// Contract status indicating an active/current contract.
    /// </summary>
    public const string EstadoVigente = "Vigente";

    /// <summary>
    /// Contract status indicating a finalized contract.
    /// </summary>
    public const string EstadoFinalizado = "Finalizado";

    /// <summary>
    /// Contract status indicating a rescinded contract.
    /// </summary>
    public const string EstadoRescindido = "Rescindido";
}
