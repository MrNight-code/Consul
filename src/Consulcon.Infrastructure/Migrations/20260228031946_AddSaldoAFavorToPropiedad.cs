using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consulcon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaldoAFavorToPropiedad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "autorizacion_gasto",
                columns: table => new
                {
                    id_autorizacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Niveles de firma para gastos", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_autorizacion);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_servicio",
                columns: table => new
                {
                    id_servicio = table.Column<int>(type: "int", nullable: false, comment: "Antes: serviciopago")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Agua, Luz, Multa, Expensa", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    costo_base = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    es_recurrente = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_servicio);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "CondominiosMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConnectionString = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondominiosMaster", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "PermisosMaster",
                columns: table => new
                {
                    IdPermiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisosMaster", x => x.IdPermiso);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "persona",
                columns: table => new
                {
                    id_persona = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre_completo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, comment: "Antes: nombre", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ci = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    sexo = table.Column<string>(type: "char(1)", fixedLength: true, maxLength: 1, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado_civil = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    es_activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'"),
                    id_familiar_responsable = table.Column<int>(type: "int", nullable: true, comment: "Recursiva: Para hijos/dependientes")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_persona);
                    table.ForeignKey(
                        name: "fk_persona_familiar",
                        column: x => x.id_familiar_responsable,
                        principalTable: "persona",
                        principalColumn: "id_persona");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "plan_cuentas",
                columns: table => new
                {
                    id_cuenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo_cuenta = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "Ej: 1.1.01", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_cuenta_padre = table.Column<int>(type: "int", nullable: true, comment: "Recursiva"),
                    nivel_jerarquia = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'1'"),
                    es_imputable = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'", comment: "Si/No")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_cuenta);
                    table.ForeignKey(
                        name: "fk_pc_padre",
                        column: x => x.id_cuenta_padre,
                        principalTable: "plan_cuentas",
                        principalColumn: "id_cuenta");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "proveedor",
                columns: table => new
                {
                    id_proveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    razon_social = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nit = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contacto = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    direccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_proveedor);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "RolesMaster",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesMaster", x => x.IdRol);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "condominio",
                columns: table => new
                {
                    id_condominio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    direccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    superficie_total_m2 = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    id_admin_persona = table.Column<int>(type: "int", nullable: false),
                    config_dia_cobro = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    logo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_condominio);
                    table.ForeignKey(
                        name: "fk_condominio_admin",
                        column: x => x.id_admin_persona,
                        principalTable: "persona",
                        principalColumn: "id_persona");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "medio_contacto",
                columns: table => new
                {
                    id_contacto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_persona = table.Column<int>(type: "int", nullable: false),
                    tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Telefono, Celular, Email, Facebook", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    valor = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "El numero o correo", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    es_principal = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_contacto);
                    table.ForeignKey(
                        name: "fk_contacto_persona",
                        column: x => x.id_persona,
                        principalTable: "persona",
                        principalColumn: "id_persona",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    pk_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_persona = table.Column<int>(type: "int", nullable: false),
                    username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, comment: "Antes: contrasena", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_creacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    esta_habilitado = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.pk_usuario);
                    table.ForeignKey(
                        name: "fk_usuario_persona",
                        column: x => x.id_persona,
                        principalTable: "persona",
                        principalColumn: "id_persona");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "banco",
                columns: table => new
                {
                    id_banco = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre_entidad = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero_cuenta = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    moneda = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, defaultValueSql: "'BOB'", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "BANCO", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'"),
                    id_cuenta_contable_asociada = table.Column<int>(type: "int", nullable: true),
                    saldo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_banco);
                    table.ForeignKey(
                        name: "fk_banco_cuenta",
                        column: x => x.id_cuenta_contable_asociada,
                        principalTable: "plan_cuentas",
                        principalColumn: "id_cuenta");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "config_contable_servicio",
                columns: table => new
                {
                    id_servicio = table.Column<int>(type: "int", nullable: false),
                    id_cuenta_ingreso = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id_servicio, x.id_cuenta_ingreso })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "fk_ccs_cuenta",
                        column: x => x.id_cuenta_ingreso,
                        principalTable: "plan_cuentas",
                        principalColumn: "id_cuenta");
                    table.ForeignKey(
                        name: "fk_ccs_servicio",
                        column: x => x.id_servicio,
                        principalTable: "catalogo_servicio",
                        principalColumn: "id_servicio");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "forma_pago",
                columns: table => new
                {
                    id_forma_pago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Efectivo, Cheque, Transferencia", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_cuenta_contable_asociada = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_forma_pago);
                    table.ForeignKey(
                        name: "fk_fp_cuenta",
                        column: x => x.id_cuenta_contable_asociada,
                        principalTable: "plan_cuentas",
                        principalColumn: "id_cuenta");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "PermisoMasterRolMaster",
                columns: table => new
                {
                    PermisosIdPermiso = table.Column<int>(type: "int", nullable: false),
                    RolesIdRol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisoMasterRolMaster", x => new { x.PermisosIdPermiso, x.RolesIdRol });
                    table.ForeignKey(
                        name: "FK_PermisoMasterRolMaster_PermisosMaster_PermisosIdPermiso",
                        column: x => x.PermisosIdPermiso,
                        principalTable: "PermisosMaster",
                        principalColumn: "IdPermiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermisoMasterRolMaster_RolesMaster_RolesIdRol",
                        column: x => x.RolesIdRol,
                        principalTable: "RolesMaster",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "UsuariosMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EsSuperAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdRolPrincipal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosMaster_RolesMaster_IdRolPrincipal",
                        column: x => x.IdRolPrincipal,
                        principalTable: "RolesMaster",
                        principalColumn: "IdRol");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "charge_concept",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    condominium_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_recurrent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_charge_concept", x => x.id);
                    table.ForeignKey(
                        name: "FK_charge_concept_condominio_condominium_id",
                        column: x => x.condominium_id,
                        principalTable: "condominio",
                        principalColumn: "id_condominio",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "comunicado_blog",
                columns: table => new
                {
                    id_blog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_condominio = table.Column<int>(type: "int", nullable: false),
                    fecha_publicacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contenido_html = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url_imagen = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url_archivo_adjunto = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_blog);
                    table.ForeignKey(
                        name: "fk_blog_condominio",
                        column: x => x.id_condominio,
                        principalTable: "condominio",
                        principalColumn: "id_condominio");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "config_aviso_cobranza",
                columns: table => new
                {
                    id_config = table.Column<int>(type: "int", nullable: false, comment: "Antes: confaviso")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_condominio = table.Column<int>(type: "int", nullable: false),
                    texto_header = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    texto_footer = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    dias_vencimiento_defecto = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'10'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_config);
                    table.ForeignKey(
                        name: "fk_aviso_condominio",
                        column: x => x.id_condominio,
                        principalTable: "condominio",
                        principalColumn: "id_condominio");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "financial_config",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    condominium_id = table.Column<int>(type: "int", nullable: false),
                    monthly_interest_rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    grace_days = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_config", x => x.id);
                    table.ForeignKey(
                        name: "FK_financial_config_condominio_condominium_id",
                        column: x => x.condominium_id,
                        principalTable: "condominio",
                        principalColumn: "id_condominio",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "manzano",
                columns: table => new
                {
                    id_manzano = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_condominio = table.Column<int>(type: "int", nullable: false),
                    codigo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_manzano);
                    table.ForeignKey(
                        name: "fk_manzano_condominio",
                        column: x => x.id_condominio,
                        principalTable: "condominio",
                        principalColumn: "id_condominio");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "recurso_comun",
                columns: table => new
                {
                    id_recurso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_condominio = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Churrasquera, Salon", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    costo_reserva = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    costo_garantia = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    color_calendario = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, comment: "Antes en tabla evento", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_recurso);
                    table.ForeignKey(
                        name: "fk_recurso_condominio",
                        column: x => x.id_condominio,
                        principalTable: "condominio",
                        principalColumn: "id_condominio");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "AccountDailyBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdBanco = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    BancoIdBanco = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountDailyBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountDailyBalances_banco_BancoIdBanco",
                        column: x => x.BancoIdBanco,
                        principalTable: "banco",
                        principalColumn: "id_banco",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "egreso",
                columns: table => new
                {
                    id_egreso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_condominio = table.Column<int>(type: "int", nullable: false),
                    id_proveedor = table.Column<int>(type: "int", nullable: true, comment: "Opcional"),
                    id_persona_beneficiario = table.Column<int>(type: "int", nullable: true, comment: "Opcional"),
                    id_autorizacion = table.Column<int>(type: "int", nullable: false),
                    id_banco_origen = table.Column<int>(type: "int", nullable: false),
                    id_forma_pago = table.Column<int>(type: "int", nullable: false),
                    concepto = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    monto_total = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    fecha_egreso = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    nro_factura_proveedor = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_usuario_registro = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_egreso);
                    table.ForeignKey(
                        name: "fk_egreso_aut",
                        column: x => x.id_autorizacion,
                        principalTable: "autorizacion_gasto",
                        principalColumn: "id_autorizacion");
                    table.ForeignKey(
                        name: "fk_egreso_banco",
                        column: x => x.id_banco_origen,
                        principalTable: "banco",
                        principalColumn: "id_banco");
                    table.ForeignKey(
                        name: "fk_egreso_condominio",
                        column: x => x.id_condominio,
                        principalTable: "condominio",
                        principalColumn: "id_condominio");
                    table.ForeignKey(
                        name: "fk_egreso_fp",
                        column: x => x.id_forma_pago,
                        principalTable: "forma_pago",
                        principalColumn: "id_forma_pago");
                    table.ForeignKey(
                        name: "fk_egreso_persona",
                        column: x => x.id_persona_beneficiario,
                        principalTable: "persona",
                        principalColumn: "id_persona");
                    table.ForeignKey(
                        name: "fk_egreso_proveedor",
                        column: x => x.id_proveedor,
                        principalTable: "proveedor",
                        principalColumn: "id_proveedor");
                    table.ForeignKey(
                        name: "fk_egreso_usuario",
                        column: x => x.id_usuario_registro,
                        principalTable: "usuario",
                        principalColumn: "pk_usuario");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "UsuarioCondominio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    CondominioId = table.Column<int>(type: "int", nullable: false),
                    IdRol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCondominio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioCondominio_CondominiosMaster_CondominioId",
                        column: x => x.CondominioId,
                        principalTable: "CondominiosMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCondominio_RolesMaster_IdRol",
                        column: x => x.IdRol,
                        principalTable: "RolesMaster",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCondominio_UsuariosMaster_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "UsuariosMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "propiedad",
                columns: table => new
                {
                    id_propiedad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_manzano = table.Column<int>(type: "int", nullable: false),
                    codigo_unidad = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre_funcional = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    superficie_m2 = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    porcentaje_participacion = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true, comment: "Para prorrateo"),
                    expensa_base_defecto = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Casa, Depto, Lote", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'"),
                    saldo_deudor = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValueSql: "'0.00'"),
                    saldo_a_favor = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValueSql: "'0.00'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_propiedad);
                    table.ForeignKey(
                        name: "fk_propiedad_manzano",
                        column: x => x.id_manzano,
                        principalTable: "manzano",
                        principalColumn: "id_manzano");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "AccountTransactionHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    ExpenseId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTransactionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTransactionHistory_banco_AccountId",
                        column: x => x.AccountId,
                        principalTable: "banco",
                        principalColumn: "id_banco",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountTransactionHistory_egreso_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "egreso",
                        principalColumn: "id_egreso",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "egreso_detalle",
                columns: table => new
                {
                    id_egreso_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_egreso = table.Column<int>(type: "int", nullable: false),
                    concepto = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    precio_unitario = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_egreso_detalle);
                    table.ForeignKey(
                        name: "fk_egreso_detalle_egreso",
                        column: x => x.id_egreso,
                        principalTable: "egreso",
                        principalColumn: "id_egreso",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "ExpenseAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EgresoId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoredFileName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UploadedBy = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseAttachments_egreso_EgresoId",
                        column: x => x.EgresoId,
                        principalTable: "egreso",
                        principalColumn: "id_egreso",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "contrato",
                columns: table => new
                {
                    id_contrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_propiedad = table.Column<int>(type: "int", nullable: false),
                    fecha_firma = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_ingreso_real = table.Column<DateOnly>(type: "date", nullable: true),
                    monto_expensa_pactada = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, defaultValueSql: "'Vigente'", comment: "Vigente, Finalizado, Rescindido", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    motivo_baja = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_usuario_creador = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_contrato);
                    table.ForeignKey(
                        name: "fk_contrato_creador",
                        column: x => x.id_usuario_creador,
                        principalTable: "usuario",
                        principalColumn: "pk_usuario");
                    table.ForeignKey(
                        name: "fk_contrato_propiedad",
                        column: x => x.id_propiedad,
                        principalTable: "propiedad",
                        principalColumn: "id_propiedad");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "contrato_participante",
                columns: table => new
                {
                    id_contrato = table.Column<int>(type: "int", nullable: false),
                    id_persona = table.Column<int>(type: "int", nullable: false),
                    rol_contrato = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Titular, Inquilino, Garante", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_alta = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_baja = table.Column<DateOnly>(type: "date", nullable: true),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id_contrato, x.id_persona })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "fk_cp_contrato",
                        column: x => x.id_contrato,
                        principalTable: "contrato",
                        principalColumn: "id_contrato");
                    table.ForeignKey(
                        name: "fk_cp_persona",
                        column: x => x.id_persona,
                        principalTable: "persona",
                        principalColumn: "id_persona");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "contrato_servicio_suscrito",
                columns: table => new
                {
                    id_suscripcion = table.Column<int>(type: "int", nullable: false, comment: "Antes: servicio_contrato")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_contrato = table.Column<int>(type: "int", nullable: false),
                    id_servicio = table.Column<int>(type: "int", nullable: false),
                    costo_personalizado = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true, comment: "Si difiere del base"),
                    activo = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_suscripcion);
                    table.ForeignKey(
                        name: "fk_css_contrato",
                        column: x => x.id_contrato,
                        principalTable: "contrato",
                        principalColumn: "id_contrato");
                    table.ForeignKey(
                        name: "fk_css_servicio",
                        column: x => x.id_servicio,
                        principalTable: "catalogo_servicio",
                        principalColumn: "id_servicio");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "deuda_cabecera",
                columns: table => new
                {
                    id_deuda = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_contrato = table.Column<int>(type: "int", nullable: false),
                    anio_periodo = table.Column<int>(type: "int", nullable: false),
                    mes_periodo = table.Column<int>(type: "int", nullable: false),
                    fecha_emision = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_vencimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    total_deuda = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    total_pagado = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    estado_pago = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, defaultValueSql: "'PENDIENTE'", comment: "PENDIENTE, PARCIAL, PAGADO, ANULADO", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_usuario_generador = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_deuda);
                    table.ForeignKey(
                        name: "fk_deuda_contrato",
                        column: x => x.id_contrato,
                        principalTable: "contrato",
                        principalColumn: "id_contrato");
                    table.ForeignKey(
                        name: "fk_deuda_usuario",
                        column: x => x.id_usuario_generador,
                        principalTable: "usuario",
                        principalColumn: "pk_usuario");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "reserva",
                columns: table => new
                {
                    id_reserva = table.Column<int>(type: "int", nullable: false, comment: "Antes: evento")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_recurso = table.Column<int>(type: "int", nullable: false),
                    id_contrato = table.Column<int>(type: "int", nullable: false, comment: "Quien reserva"),
                    fecha_inicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "datetime", nullable: false),
                    cantidad_invitados = table.Column<int>(type: "int", nullable: true),
                    motivo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    amenizado_por = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    monto_total_cobrado = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, defaultValueSql: "'PENDIENTE'", comment: "PENDIENTE, CONFIRMADA, FINALIZADA", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_reserva);
                    table.ForeignKey(
                        name: "fk_reserva_contrato",
                        column: x => x.id_contrato,
                        principalTable: "contrato",
                        principalColumn: "id_contrato");
                    table.ForeignKey(
                        name: "fk_reserva_recurso",
                        column: x => x.id_recurso,
                        principalTable: "recurso_comun",
                        principalColumn: "id_recurso");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "lectura_servicio",
                columns: table => new
                {
                    id_lectura = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_suscripcion = table.Column<int>(type: "int", nullable: false),
                    anio = table.Column<int>(type: "int", nullable: false),
                    mes = table.Column<int>(type: "int", nullable: false),
                    valor_leido = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true, comment: "Para agua/luz variable"),
                    monto_calculado = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    fecha_lectura = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_lectura);
                    table.ForeignKey(
                        name: "fk_lectura_suscripcion",
                        column: x => x.id_suscripcion,
                        principalTable: "contrato_servicio_suscrito",
                        principalColumn: "id_suscripcion");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "deuda_detalle",
                columns: table => new
                {
                    id_deuda_det = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_deuda = table.Column<int>(type: "int", nullable: false),
                    id_servicio = table.Column<int>(type: "int", nullable: false, comment: "Origen del cobro"),
                    concepto = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, comment: "Ej: Expensa Mayo 2025", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    monto_unitario = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    cantidad = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true, defaultValueSql: "'1.00'"),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_deuda_det);
                    table.ForeignKey(
                        name: "fk_dd_cabecera",
                        column: x => x.id_deuda,
                        principalTable: "deuda_cabecera",
                        principalColumn: "id_deuda",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dd_servicio",
                        column: x => x.id_servicio,
                        principalTable: "catalogo_servicio",
                        principalColumn: "id_servicio");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "transaccion_pago",
                columns: table => new
                {
                    id_pago = table.Column<int>(type: "int", nullable: false, comment: "Antes: cuota")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_deuda = table.Column<int>(type: "int", nullable: false, comment: "Pago especifico de una deuda"),
                    id_persona_pagador = table.Column<int>(type: "int", nullable: false),
                    id_banco_destino = table.Column<int>(type: "int", nullable: false),
                    id_forma_pago = table.Column<int>(type: "int", nullable: false),
                    fecha_pago = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    monto_abonado = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    tipo_cambio = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: true, defaultValueSql: "'1.0000'"),
                    nro_comprobante_banco = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, defaultValueSql: "'CONFIRMADO'", comment: "CONFIRMADO, RECHAZADO", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recibo_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_recibo = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_pago);
                    table.ForeignKey(
                        name: "fk_tp_banco",
                        column: x => x.id_banco_destino,
                        principalTable: "banco",
                        principalColumn: "id_banco");
                    table.ForeignKey(
                        name: "fk_tp_deuda",
                        column: x => x.id_deuda,
                        principalTable: "deuda_cabecera",
                        principalColumn: "id_deuda");
                    table.ForeignKey(
                        name: "fk_tp_forma",
                        column: x => x.id_forma_pago,
                        principalTable: "forma_pago",
                        principalColumn: "id_forma_pago");
                    table.ForeignKey(
                        name: "fk_tp_persona",
                        column: x => x.id_persona_pagador,
                        principalTable: "persona",
                        principalColumn: "id_persona");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "asiento_contable",
                columns: table => new
                {
                    id_asiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_condominio = table.Column<int>(type: "int", nullable: false),
                    fecha_contable = table.Column<DateTime>(type: "datetime", nullable: false),
                    glosa_general = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_asiento = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, comment: "Diario, Ajuste, Cierre", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nro_documento_respaldo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_transaccion_origen_pago = table.Column<int>(type: "int", nullable: true, comment: "Link a Tesoreria"),
                    id_transaccion_origen_egreso = table.Column<int>(type: "int", nullable: true, comment: "Link a Tesoreria")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_asiento);
                    table.ForeignKey(
                        name: "fk_asiento_condominio",
                        column: x => x.id_condominio,
                        principalTable: "condominio",
                        principalColumn: "id_condominio");
                    table.ForeignKey(
                        name: "fk_asiento_egreso",
                        column: x => x.id_transaccion_origen_egreso,
                        principalTable: "egreso",
                        principalColumn: "id_egreso");
                    table.ForeignKey(
                        name: "fk_asiento_pago",
                        column: x => x.id_transaccion_origen_pago,
                        principalTable: "transaccion_pago",
                        principalColumn: "id_pago");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "asiento_detalle",
                columns: table => new
                {
                    id_asiento_det = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_asiento = table.Column<int>(type: "int", nullable: false),
                    id_cuenta = table.Column<int>(type: "int", nullable: false),
                    glosa_linea = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    debe = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "'0.00'"),
                    haber = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "'0.00'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_asiento_det);
                    table.ForeignKey(
                        name: "fk_ad_asiento",
                        column: x => x.id_asiento,
                        principalTable: "asiento_contable",
                        principalColumn: "id_asiento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ad_cuenta",
                        column: x => x.id_cuenta,
                        principalTable: "plan_cuentas",
                        principalColumn: "id_cuenta");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDailyBalances_BancoIdBanco",
                table: "AccountDailyBalances",
                column: "BancoIdBanco");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactionHistory_AccountId",
                table: "AccountTransactionHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactionHistory_ExpenseId",
                table: "AccountTransactionHistory",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "fk_asiento_condominio",
                table: "asiento_contable",
                column: "id_condominio");

            migrationBuilder.CreateIndex(
                name: "fk_asiento_egreso",
                table: "asiento_contable",
                column: "id_transaccion_origen_egreso");

            migrationBuilder.CreateIndex(
                name: "fk_asiento_pago",
                table: "asiento_contable",
                column: "id_transaccion_origen_pago");

            migrationBuilder.CreateIndex(
                name: "fk_ad_asiento",
                table: "asiento_detalle",
                column: "id_asiento");

            migrationBuilder.CreateIndex(
                name: "fk_ad_cuenta",
                table: "asiento_detalle",
                column: "id_cuenta");

            migrationBuilder.CreateIndex(
                name: "fk_banco_cuenta",
                table: "banco",
                column: "id_cuenta_contable_asociada");

            migrationBuilder.CreateIndex(
                name: "IX_charge_concept_condominium_id",
                table: "charge_concept",
                column: "condominium_id");

            migrationBuilder.CreateIndex(
                name: "fk_blog_condominio",
                table: "comunicado_blog",
                column: "id_condominio");

            migrationBuilder.CreateIndex(
                name: "fk_condominio_admin",
                table: "condominio",
                column: "id_admin_persona");

            migrationBuilder.CreateIndex(
                name: "IX_CondominiosMaster_TenantId",
                table: "CondominiosMaster",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_aviso_condominio",
                table: "config_aviso_cobranza",
                column: "id_condominio");

            migrationBuilder.CreateIndex(
                name: "fk_ccs_cuenta",
                table: "config_contable_servicio",
                column: "id_cuenta_ingreso");

            migrationBuilder.CreateIndex(
                name: "fk_contrato_creador",
                table: "contrato",
                column: "id_usuario_creador");

            migrationBuilder.CreateIndex(
                name: "fk_contrato_propiedad",
                table: "contrato",
                column: "id_propiedad");

            migrationBuilder.CreateIndex(
                name: "fk_cp_persona",
                table: "contrato_participante",
                column: "id_persona");

            migrationBuilder.CreateIndex(
                name: "fk_css_contrato",
                table: "contrato_servicio_suscrito",
                column: "id_contrato");

            migrationBuilder.CreateIndex(
                name: "fk_css_servicio",
                table: "contrato_servicio_suscrito",
                column: "id_servicio");

            migrationBuilder.CreateIndex(
                name: "fk_deuda_contrato",
                table: "deuda_cabecera",
                column: "id_contrato");

            migrationBuilder.CreateIndex(
                name: "fk_deuda_usuario",
                table: "deuda_cabecera",
                column: "id_usuario_generador");

            migrationBuilder.CreateIndex(
                name: "fk_dd_cabecera",
                table: "deuda_detalle",
                column: "id_deuda");

            migrationBuilder.CreateIndex(
                name: "fk_dd_servicio",
                table: "deuda_detalle",
                column: "id_servicio");

            migrationBuilder.CreateIndex(
                name: "fk_egreso_aut",
                table: "egreso",
                column: "id_autorizacion");

            migrationBuilder.CreateIndex(
                name: "fk_egreso_banco",
                table: "egreso",
                column: "id_banco_origen");

            migrationBuilder.CreateIndex(
                name: "fk_egreso_condominio",
                table: "egreso",
                column: "id_condominio");

            migrationBuilder.CreateIndex(
                name: "fk_egreso_fp",
                table: "egreso",
                column: "id_forma_pago");

            migrationBuilder.CreateIndex(
                name: "fk_egreso_persona",
                table: "egreso",
                column: "id_persona_beneficiario");

            migrationBuilder.CreateIndex(
                name: "fk_egreso_proveedor",
                table: "egreso",
                column: "id_proveedor");

            migrationBuilder.CreateIndex(
                name: "fk_egreso_usuario",
                table: "egreso",
                column: "id_usuario_registro");

            migrationBuilder.CreateIndex(
                name: "fk_egreso_detalle_egreso",
                table: "egreso_detalle",
                column: "id_egreso");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAttachments_EgresoId",
                table: "ExpenseAttachments",
                column: "EgresoId");

            migrationBuilder.CreateIndex(
                name: "IX_financial_config_condominium_id",
                table: "financial_config",
                column: "condominium_id");

            migrationBuilder.CreateIndex(
                name: "fk_fp_cuenta",
                table: "forma_pago",
                column: "id_cuenta_contable_asociada");

            migrationBuilder.CreateIndex(
                name: "fk_lectura_suscripcion",
                table: "lectura_servicio",
                column: "id_suscripcion");

            migrationBuilder.CreateIndex(
                name: "fk_manzano_condominio",
                table: "manzano",
                column: "id_condominio");

            migrationBuilder.CreateIndex(
                name: "fk_contacto_persona",
                table: "medio_contacto",
                column: "id_persona");

            migrationBuilder.CreateIndex(
                name: "IX_PermisoMasterRolMaster_RolesIdRol",
                table: "PermisoMasterRolMaster",
                column: "RolesIdRol");

            migrationBuilder.CreateIndex(
                name: "fk_persona_familiar",
                table: "persona",
                column: "id_familiar_responsable");

            migrationBuilder.CreateIndex(
                name: "fk_pc_padre",
                table: "plan_cuentas",
                column: "id_cuenta_padre");

            migrationBuilder.CreateIndex(
                name: "fk_propiedad_manzano",
                table: "propiedad",
                column: "id_manzano");

            migrationBuilder.CreateIndex(
                name: "fk_recurso_condominio",
                table: "recurso_comun",
                column: "id_condominio");

            migrationBuilder.CreateIndex(
                name: "fk_reserva_contrato",
                table: "reserva",
                column: "id_contrato");

            migrationBuilder.CreateIndex(
                name: "fk_reserva_recurso",
                table: "reserva",
                column: "id_recurso");

            migrationBuilder.CreateIndex(
                name: "fk_tp_banco",
                table: "transaccion_pago",
                column: "id_banco_destino");

            migrationBuilder.CreateIndex(
                name: "fk_tp_deuda",
                table: "transaccion_pago",
                column: "id_deuda");

            migrationBuilder.CreateIndex(
                name: "fk_tp_forma",
                table: "transaccion_pago",
                column: "id_forma_pago");

            migrationBuilder.CreateIndex(
                name: "fk_tp_persona",
                table: "transaccion_pago",
                column: "id_persona_pagador");

            migrationBuilder.CreateIndex(
                name: "fk_usuario_persona",
                table: "usuario",
                column: "id_persona");

            migrationBuilder.CreateIndex(
                name: "username",
                table: "usuario",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCondominio_CondominioId",
                table: "UsuarioCondominio",
                column: "CondominioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCondominio_IdRol",
                table: "UsuarioCondominio",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCondominio_UsuarioId",
                table: "UsuarioCondominio",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosMaster_IdRolPrincipal",
                table: "UsuariosMaster",
                column: "IdRolPrincipal");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosMaster_Username",
                table: "UsuariosMaster",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountDailyBalances");

            migrationBuilder.DropTable(
                name: "AccountTransactionHistory");

            migrationBuilder.DropTable(
                name: "asiento_detalle");

            migrationBuilder.DropTable(
                name: "charge_concept");

            migrationBuilder.DropTable(
                name: "comunicado_blog");

            migrationBuilder.DropTable(
                name: "config_aviso_cobranza");

            migrationBuilder.DropTable(
                name: "config_contable_servicio");

            migrationBuilder.DropTable(
                name: "contrato_participante");

            migrationBuilder.DropTable(
                name: "deuda_detalle");

            migrationBuilder.DropTable(
                name: "egreso_detalle");

            migrationBuilder.DropTable(
                name: "ExpenseAttachments");

            migrationBuilder.DropTable(
                name: "financial_config");

            migrationBuilder.DropTable(
                name: "lectura_servicio");

            migrationBuilder.DropTable(
                name: "medio_contacto");

            migrationBuilder.DropTable(
                name: "PermisoMasterRolMaster");

            migrationBuilder.DropTable(
                name: "reserva");

            migrationBuilder.DropTable(
                name: "UsuarioCondominio");

            migrationBuilder.DropTable(
                name: "asiento_contable");

            migrationBuilder.DropTable(
                name: "contrato_servicio_suscrito");

            migrationBuilder.DropTable(
                name: "PermisosMaster");

            migrationBuilder.DropTable(
                name: "recurso_comun");

            migrationBuilder.DropTable(
                name: "CondominiosMaster");

            migrationBuilder.DropTable(
                name: "UsuariosMaster");

            migrationBuilder.DropTable(
                name: "egreso");

            migrationBuilder.DropTable(
                name: "transaccion_pago");

            migrationBuilder.DropTable(
                name: "catalogo_servicio");

            migrationBuilder.DropTable(
                name: "RolesMaster");

            migrationBuilder.DropTable(
                name: "autorizacion_gasto");

            migrationBuilder.DropTable(
                name: "proveedor");

            migrationBuilder.DropTable(
                name: "banco");

            migrationBuilder.DropTable(
                name: "deuda_cabecera");

            migrationBuilder.DropTable(
                name: "forma_pago");

            migrationBuilder.DropTable(
                name: "contrato");

            migrationBuilder.DropTable(
                name: "plan_cuentas");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "propiedad");

            migrationBuilder.DropTable(
                name: "manzano");

            migrationBuilder.DropTable(
                name: "condominio");

            migrationBuilder.DropTable(
                name: "persona");
        }
    }
}
