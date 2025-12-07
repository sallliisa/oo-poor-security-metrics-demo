namespace MediLink.Data
{
    /// <summary>
    /// Stores doctor information with authentication credentials.
    /// Demonstrates POOR security practices for educational analysis.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 6
    /// - Vulnerable Attributes: 3 (AuthToken, PrivateKey, Password)
    /// - AVR Score: 3/6 = 0.50 (50%)
    /// </summary>
    public class Doctor
    {
        // ========== SAFE ATTRIBUTES (3) ==========

        /// <summary>
        /// Unique identifier for the doctor.
        /// </summary>
        public int DoctorID { get; set; }

        /// <summary>
        /// Public name - considered non-sensitive.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Medical specialty - public information.
        /// </summary>
        public string Specialty { get; set; } = string.Empty;

        // ========== VULNERABLE ATTRIBUTES (3) ==========

        /// <summary>
        /// HIGH PRIVILEGE - Authentication token stored in plain text.
        /// VULNERABILITY: Should be securely stored, rotated, and never exposed.
        /// </summary>
        public string AuthToken { get; set; } = string.Empty;

        /// <summary>
        /// CRITICAL - Private signing key exposed as public property.
        /// VULNERABILITY: Private keys should NEVER be accessible as properties.
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// CRITICAL - Plain text password storage.
        /// VULNERABILITY: Passwords must be hashed, never stored in plain text.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        // ========== METHODS ==========

        /// <summary>
        /// VULNERABLE: Validates password with plain text comparison.
        /// VA Risk: HIGH (1/1 = 1.00)
        /// </summary>
        /// <param name="input">User-provided password</param>
        /// <returns>True if password matches</returns>
        public bool ValidatePassword(string input)
        {
            // BAD PRACTICE: Plain text password comparison
            // Should use secure hash comparison (e.g., BCrypt, Argon2)
            return Password == input;
        }

        /// <summary>
        /// VULNERABLE: Returns auth token without validation.
        /// VA Risk: HIGH (1/1 = 1.00)
        /// </summary>
        /// <returns>Raw authentication token</returns>
        public string GetAuthToken()
        {
            // BAD PRACTICE: No caller validation, no scope checking
            return AuthToken;
        }

        /// <summary>
        /// VULNERABLE: Signs data with exposed private key.
        /// VA Risk: CRITICAL (1/2 = 0.50)
        /// </summary>
        /// <param name="data">Data to sign</param>
        /// <returns>Signed data string</returns>
        public string SignPrescription(string data)
        {
            // BAD PRACTICE: Using PrivateKey directly in concatenation
            // Should use proper cryptographic signing
            return $"{data}|SIGNED_BY|{PrivateKey}";
        }

        /// <summary>
        /// VULNERABLE: Generates new token without proper randomness.
        /// </summary>
        /// <returns>Weakly generated token</returns>
        public string GenerateNewToken()
        {
            // BAD PRACTICE: Predictable token generation
            AuthToken = $"TOKEN_{DoctorID}_{DateTime.Now.Ticks}";
            return AuthToken;
        }

        /// <summary>
        /// VULNERABLE: Resets password with new plain text value.
        /// </summary>
        /// <param name="newPassword">New password in plain text</param>
        public void ResetPassword(string newPassword)
        {
            // BAD PRACTICE: No password complexity validation
            // No hashing, storing plain text
            Password = newPassword;
        }

        /// <summary>
        /// VULNERABLE: Exports all credentials.
        /// VA Risk: CRITICAL
        /// </summary>
        /// <returns>Dictionary with all sensitive credentials</returns>
        public Dictionary<string, string> ExportCredentials()
        {
            // BAD PRACTICE: Exporting all sensitive credentials at once
            return new Dictionary<string, string>
            {
                { "AuthToken", AuthToken },     // VULNERABILITY
                { "PrivateKey", PrivateKey },   // VULNERABILITY
                { "Password", Password }        // VULNERABILITY
            };
        }
    }
}
