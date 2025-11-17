namespace Fcg.Auth.Domain
{
    public class User
    {
        public Guid Id { get; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }

        public User(string email, string role = "User")
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(email));
            if (!IsValidEmail(email))
                throw new ArgumentException("Formato de email é inválido", nameof(email));
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role não pode ser vazio ou nulo.", nameof(role));

            Id = Guid.NewGuid();
            Email = email;
            Role = role;
            PasswordHash = string.Empty;
        }

        public User(Guid id, string email, string passwordHash, string role)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id não pode ser vazio ou nulo.", nameof(id));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash não pode ser vazio ou nulo.", nameof(passwordHash));
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role não pode ser vazio ou nulo.", nameof(role));

            Id = id;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }

        public void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash não pode ser vazio ou nulo.", nameof(passwordHash));

            PasswordHash = passwordHash;
        }

        public void SetRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role não pode ser vazio ou nulo.", nameof(role));

            Role = role;
        }

        public void UpdateEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("Email não pode ser vazio ou nulo.", nameof(newEmail));
            if (!IsValidEmail(newEmail))
                throw new ArgumentException("Formato de email é inválido (Parameter 'newEmail')", nameof(newEmail));

            Email = newEmail;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
