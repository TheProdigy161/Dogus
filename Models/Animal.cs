namespace Dogus.Models;

public class Animal
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public DateOnly DateOfDeath { get; set; }
    public int Age => DateOfDeath != default ? DateOfDeath.Year - DateOfBirth.Year : DateTime.Now.Year - DateOfBirth.Year;
    public bool IsDead => DateOfDeath != default;
}