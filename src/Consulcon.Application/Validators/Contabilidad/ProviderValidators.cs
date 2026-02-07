using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;
using FluentValidation;

namespace Consulcon.Application.Validators.Contabilidad;

/// Validador para CreateProviderDto
public class CreateProviderValidator : AbstractValidator<CreateProviderDto>
{
    private readonly IProviderRepository _repository;

    public CreateProviderValidator(IProviderRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("El NIT es requerido")
            .MaximumLength(20).WithMessage("El NIT no puede exceder 20 caracteres")
            .Matches(@"^\d+$").WithMessage("El NIT debe contener solo números");

        RuleFor(x => x.LegalName)
            .NotEmpty().WithMessage("La razón social es requerida")
            .MaximumLength(150).WithMessage("La razón social no puede exceder 150 caracteres");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El email no tiene un formato válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Address));

        // Validación asíncrona: NIT único
        RuleFor(x => x.TaxId)
            .MustAsync(BeUniqueTaxId)
            .WithMessage(x => $"El proveedor con NIT {x.TaxId} ya existe en este condominio");
    }

    private async Task<bool> BeUniqueTaxId(string taxId, CancellationToken cancellationToken)
    {
        return !await _repository.ExistsByTaxIdAsync(taxId, cancellationToken);
    }
}

/// Validador para UpdateProviderDto
public class UpdateProviderValidator : AbstractValidator<UpdateProviderDto>
{
    public UpdateProviderValidator()
    {
        RuleFor(x => x.LegalName)
            .NotEmpty().WithMessage("La razón social es requerida")
            .MaximumLength(150).WithMessage("La razón social no puede exceder 150 caracteres");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El email no tiene un formato válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Address)
            .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Address));
    }
}
