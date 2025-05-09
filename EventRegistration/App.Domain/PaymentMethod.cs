using System.ComponentModel.DataAnnotations;
using Domain.Base;

namespace App.Domain;

public class PaymentMethod : DomainEntityId
{
    [MaxLength(512)]
    public string Name { get; set; } = default!;
}