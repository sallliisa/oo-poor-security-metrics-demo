namespace MediLink.Data
{
    /// <summary>
    /// Represents a system administrator.
    /// Inherits vulnerable authentication from User class.
    /// </summary>
    public class Admin : User
    {
        /// <summary>
        /// Admin-specific permission level.
        /// </summary>
        public int AccessLevel { get; set; } = 1;

        /// <summary>
        /// VULNERABLE: Role stored as plain text without proper authorization checks.
        /// </summary>
        public string Role { get; set; } = "Admin";
    }
}
