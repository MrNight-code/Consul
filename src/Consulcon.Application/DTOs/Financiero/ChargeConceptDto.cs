namespace Consulcon.Application.DTOs.Financiero;

public class ChargeConceptDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public bool IsRecurrent { get; set; }
    public bool IsActive { get; set; }
}

public class CreateChargeConceptDto
{
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public bool IsRecurrent { get; set; }
}

public class UpdateChargeConceptDto
{
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public bool IsRecurrent { get; set; }
    public bool IsActive { get; set; }
}
