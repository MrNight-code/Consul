using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Consulcon.Application.Common.Settings;
using Consulcon.Application.Interfaces.Seguridad;
using Consulcon.Application.Interfaces;
using Consulcon.Application.Services.Seguridad;
using Consulcon.Application.Interfaces.Personas;
using Consulcon.Application.Services.Personas;
using Consulcon.Application.Interfaces.Inmuebles;
using Consulcon.Application.Services.Inmuebles;
using Consulcon.Application.Interfaces.Contratos;
using Consulcon.Application.Services.Contratos;
using Consulcon.Application.Interfaces.Facturacion;
using Consulcon.Application.Services.Facturacion;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Application.Services.Contabilidad;
using Consulcon.Application.Interfaces.Reservas;
using Consulcon.Application.Services.Reservas;
using Consulcon.Application.Interfaces.Comunicacion;
using Consulcon.Application.Services.Comunicacion;
using Consulcon.Application.Interfaces.Dashboard;
using Consulcon.Application.Services.Dashboard;
using FluentValidation;

namespace Consulcon.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Settings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Seguridad
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioService, UsuarioService>();

        // Personas
        services.AddScoped<IPersonaService, PersonaService>();

        // Inmuebles
        services.AddScoped<ICondominioService, CondominioService>();
        services.AddScoped<IPropiedadService, PropiedadService>();
        services.AddScoped<IManzanoService, ManzanoService>();

        // Contratos
        services.AddScoped<IContratoService, ContratoService>();
        services.AddScoped<ICatalogoServicioService, CatalogoServicioService>();

        // Facturacion
        services.AddScoped<IDeudaService, DeudaService>();
        services.AddScoped<IPagoService, PagoService>();

        // Contabilidad
        services.AddScoped<ITesoreriaService, TesoreriaService>();
        services.AddScoped<IContabilidadService, ContabilidadService>();
        services.AddScoped<IExpenseCalculationService, ExpenseCalculationService>();
        services.AddScoped<IProveedorService, ProveedorService>();

        // Reservas
        services.AddScoped<IReservaService, ReservaService>();

        // Comunicacion
        services.AddScoped<IComunicacionService, ComunicacionService>();

        // Dashboard
        services.AddScoped<IDashboardService, DashboardService>();

        // FluentValidation - Auto-register all validators from this assembly
        services.AddValidatorsFromAssemblyContaining<Validators.Contabilidad.CreateProviderValidator>();

        return services;
    }
}