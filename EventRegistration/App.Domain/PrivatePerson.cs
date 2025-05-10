using System.ComponentModel.DataAnnotations;

namespace App.Domain;

public class PrivatePerson : Participant
{
    [MaxLength(512)]
    public string FirstName { get; set; } = default!;
    [MaxLength(512)]
    public string LastName { get; set; } = default!;
    
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Personal Code should be 11 digits long.")]
    public string PersonalCode { get; set; } = default!;
}