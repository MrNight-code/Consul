using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consulcon.Domain.Entities.Master;

[Table("UsuariosMaster")]
public class UsuarioMaster
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = null!;

    [MaxLength(150)]
    public string? Email { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public bool EsSuperAdmin { get; set; }

    public int? IdRolPrincipal { get; set; }

    [ForeignKey("IdRolPrincipal")]
    public virtual RolMaster? RolPrincipal { get; set; }

    // Navigation properties
    public virtual ICollection<UsuarioCondominio> Condominios { get; set; } = [];
}

[Table("CondominiosMaster")]
public class CondominioMaster
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string TenantId { get; set; } = null!;

    [Required]
    [MaxLength(150)]
    public string Nombre { get; set; } = null!;

    [MaxLength(500)]
    public string? ConnectionString { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<UsuarioCondominio> Usuarios { get; set; } = [];
}

[Table("UsuarioCondominio")]
public class UsuarioCondominio
{
    [Key]
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int CondominioId { get; set; }

    public int IdRol { get; set; } = 3; // Default to Operador

    [ForeignKey("IdRol")]
    public virtual RolMaster Rol { get; set; } = null!;

    // Navigation properties
    public virtual UsuarioMaster Usuario { get; set; } = null!;
    public virtual CondominioMaster Condominio { get; set; } = null!;
}

[Table("RolesMaster")]
public class RolMaster
{
    [Key]
    public int IdRol { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = null!;

    public virtual ICollection<UsuarioMaster> Usuarios { get; set; } = [];

    public virtual ICollection<PermisoMaster> Permisos { get; set; } = [];
}

[Table("PermisosMaster")]
public class PermisoMaster
{
    [Key]
    public int IdPermiso { get; set; }

    [Required]
    [MaxLength(100)]
    public string Descripcion { get; set; } = null!;

    public virtual ICollection<RolMaster> Roles { get; set; } = [];
}
