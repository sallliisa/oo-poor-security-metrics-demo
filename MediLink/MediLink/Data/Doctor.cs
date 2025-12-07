namespace MediLink.Data
{
    /// <summary>
    /// Stores doctor information with authentication credentials.
    /// Demonstrates POOR security practices for educational analysis.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 6
    /// - Vulnerable Attributes: 3 (AuthToken, Password, MedicalLicenseNumber)
    /// - AVR Score: 3/6 = 0.50 (50%)
    /// </summary>
    /// <summary>
    /// Stores doctor information.
    /// Inherits vulnerable authentication from User class.
    /// </summary>
    public class Doctor : User
    {
        // ========== SAFE ATTRIBUTES (1) ==========

        /// <summary>
        /// Reference to medical specialty by ID (SAFE - normalized data).
        /// </summary>
        public int SpecialtyID { get; set; }

        /// <summary>
        /// VULNERABLE: Medical license number stored in plain text.
        /// VULNERABILITY: Professional credentials should be encrypted.
        /// </summary>
        public string MedicalLicenseNumber { get; set; } = string.Empty;
    }
}
