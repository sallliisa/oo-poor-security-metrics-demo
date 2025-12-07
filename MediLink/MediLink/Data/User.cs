namespace MediLink.Data
{
    /// <summary>
    /// Base class for all system users.
    /// Stores common attributes and authentication credentials.
    /// Demonstrates POOR security practices for educational analysis.
    /// </summary>
    public abstract class User
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Public name - considered non-sensitive.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        // ========== VULNERABLE ATTRIBUTES (2) ==========

        /// <summary>
        /// HIGH PRIVILEGE - Authentication token stored in plain text.
        /// VULNERABILITY: Should be securely stored, rotated, and never exposed.
        /// </summary>
        public string AuthToken { get; set; } = string.Empty;

        /// <summary>
        /// CRITICAL - Plain text password storage.
        /// VULNERABILITY: Passwords must be hashed, never stored in plain text.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        // ========== METHODS ==========

        /// <summary>
        /// VULNERABLE: Validates password with plain text comparison.
        /// VA Risk: HIGH
        /// </summary>
        public bool ValidatePassword(string input)
        {
            // BAD PRACTICE: Plain text password comparison
            return Password == input;
        }

        /// <summary>
        /// VULNERABLE: Returns auth token without validation.
        /// VA Risk: HIGH
        /// </summary>
        public string GetAuthToken()
        {
            return AuthToken;
        }

        /// <summary>
        /// VULNERABLE: Generates new token without proper randomness.
        /// </summary>
        public string GenerateNewToken()
        {
            AuthToken = $"TOKEN_{UserID}_{DateTime.Now.Ticks}";
            return AuthToken;
        }

        /// <summary>
        /// VULNERABLE: Resets password with new plain text value.
        /// </summary>
        public void ResetPassword(string newPassword)
        {
            Password = newPassword;
        }
    }
}
