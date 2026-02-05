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

    // Navigation properties
    public virtual ICollection<UsuarioCondominio> Condominios { get; set; } = new List<UsuarioCondominio>();
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
    public virtual ICollection<UsuarioCondominio> Usuarios { get; set; } = new List<UsuarioCondominio>();
}

[Table("UsuarioCondominio")]
public class UsuarioCondominio
{
    [Key]
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int CondominioId { get; set; }

    [MaxLength(50)]
    public string RolInicial { get; set; } = "Usuario";

    // Navigation properties
    public virtual UsuarioMaster Usuario { get; set; } = null!;
    public virtual CondominioMaster Condominio { get; set; } = null!;
}
