using System.ComponentModel.DataAnnotations;

namespace App.Domain;

public class LegalPerson : Participant
{
    [MaxLength(512)]
    public string CompanyName { get; set; } = default!;
    public string RegistryCode { get; set; } = default!;
    public int NumberOfAttendees { get; set; }
}