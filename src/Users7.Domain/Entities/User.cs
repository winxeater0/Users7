namespace Users7.Domain.Entities;

public sealed class User
{
    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
    }

    public User(string name, string email, DateOnly birthDate, DateOnly createdAt)
    {
        SetName(name);
        SetEmail(email);
        SetBirthDate(birthDate, createdAt);
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public int Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public DateOnly CreatedAt { get; private set; }
    public DateOnly UpdatedAt { get; private set; }

    public void Update(string name, string email, DateOnly birthDate, DateOnly updatedAt)
    {
        SetName(name);
        SetEmail(email);
        SetBirthDate(birthDate, updatedAt);
        UpdatedAt = updatedAt;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Nome é obrigatório.", nameof(name));
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > 50)
        {
            throw new ArgumentException("Nome deve ter no máximo 50 caracteres.", nameof(name));
        }

        Name = trimmedName;
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("E-mail é obrigatório.", nameof(email));
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (normalizedEmail.Length > 254)
        {
            throw new ArgumentException("E-mail deve ter no máximo 254 caracteres.", nameof(email));
        }

        Email = normalizedEmail;
    }

    private void SetBirthDate(DateOnly birthDate, DateOnly referenceDate)
    {
        if (birthDate > referenceDate)
        {
            throw new ArgumentException("Data de nascimento não pode ser futura.", nameof(birthDate));
        }

        BirthDate = birthDate;
    }
}
