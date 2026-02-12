using System;
using System.Collections.Generic;
using System.Reflection;
using Consulcon.Domain.Entities;
using Consulcon.Domain.Entities.Comunicacion;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Domain.Entities.Contratos;
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Domain.Entities.Financiero;
using Consulcon.Domain.Entities.General;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Entities.Master;
using Consulcon.Domain.Entities.Reservas;
using Consulcon.Domain.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;

namespace Consulcon.Infrastructure.Persistence;

public partial class ConsulconDbContext : DbContext
{
    public ConsulconDbContext()
    {
        AccountDailyBalances = Set<AccountDailyBalance>();
    }

    public ConsulconDbContext(DbContextOptions<ConsulconDbContext> options)
        : base(options)
    {
        AccountDailyBalances = Set<AccountDailyBalance>();
    }

    // Master Entities
    public virtual DbSet<UsuarioMaster> UsuariosMaster { get; set; } = null!;
    public virtual DbSet<CondominioMaster> CondominiosMaster { get; set; } = null!;
    public virtual DbSet<UsuarioCondominio> UsuarioCondominios { get; set; } = null!;

    // Financiero Entities
    public virtual DbSet<ChargeConcept> ChargeConcepts { get; set; } = null!;
    public virtual DbSet<FinancialConfig> FinancialConfigs { get; set; } = null!;

    // Consulcon Entities
    public virtual DbSet<AsientoContable> AsientoContables { get; set; } = null!;
    public virtual DbSet<AsientoDetalle> AsientoDetalles { get; set; } = null!;
    public virtual DbSet<AutorizacionGasto> AutorizacionGastos { get; set; } = null!;
    public virtual DbSet<Banco> Bancos { get; set; } = null!;
    public virtual DbSet<CatalogoServicio> CatalogoServicios { get; set; } = null!;
    public virtual DbSet<ComunicadoBlog> ComunicadoBlogs { get; set; } = null!;
    public virtual DbSet<Condominio> Condominios { get; set; } = null!;
    public virtual DbSet<ConfigAvisoCobranza> ConfigAvisoCobranzas { get; set; } = null!;
    public virtual DbSet<Contrato> Contratos { get; set; } = null!;
    public virtual DbSet<ContratoParticipante> ContratoParticipantes { get; set; } = null!;
    public virtual DbSet<ContratoServicioSuscrito> ContratoServicioSuscritos { get; set; } = null!;
    public virtual DbSet<DeudaCabecera> DeudaCabeceras { get; set; } = null!;
    public virtual DbSet<DeudaDetalle> DeudaDetalles { get; set; } = null!;
    public virtual DbSet<Egreso> Egresos { get; set; } = null!;
    public virtual DbSet<EgresoDetalle> EgresoDetalles { get; set; } = null!;
    public virtual DbSet<AccountTransactionHistory> AccountTransactionHistories { get; set; } = null!;
    public virtual DbSet<ExpenseAttachment> ExpenseAttachments { get; set; } = null!;
    public virtual DbSet<FormaPago> FormaPagos { get; set; } = null!;
    public virtual DbSet<LecturaServicio> LecturaServicios { get; set; } = null!;
    public virtual DbSet<Manzano> Manzanos { get; set; } = null!;
    public virtual DbSet<MedioContacto> MedioContactos { get; set; } = null!;
    public virtual DbSet<Permiso> Permisos { get; set; } = null!;
    public virtual DbSet<Persona> Personas { get; set; } = null!;
    public virtual DbSet<PlanCuenta> PlanCuentas { get; set; } = null!;
    public virtual DbSet<Propiedad> Propiedads { get; set; } = null!;
    public virtual DbSet<Proveedor> Proveedors { get; set; } = null!;
    public virtual DbSet<RecursoComun> RecursoComuns { get; set; } = null!;
    public virtual DbSet<Reserva> Reservas { get; set; } = null!;
    public virtual DbSet<Rol> Rols { get; set; } = null!;
    public virtual DbSet<TransaccionPago> TransaccionPagos { get; set; } = null!;
    public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<AccountDailyBalance> AccountDailyBalances { get; set; }
    
    // Facturacion
    // public virtual DbSet<Recibo> Recibos { get; set; } = null!; // REFACTORED: Integrated into TransaccionPago

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
