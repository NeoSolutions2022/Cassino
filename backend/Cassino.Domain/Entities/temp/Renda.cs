using Cassino.Domain.Contracts;

namespace Cassino.Domain.Entities.temp;

public class Renda : Entity, IAggregateRoot
{
    public Decimal Valor { get; set; }
}