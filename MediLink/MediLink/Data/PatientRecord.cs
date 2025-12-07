namespace MediLink.Data
{
    /// <summary>
    /// Stores patient information with INTENTIONALLY HIGH AVR (Attribute Vulnerability Ratio).
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 8
    /// - Vulnerable Attributes: 6 (DOB, SSN, MedicalHistory, PhoneNumber, EmailAddress, EmergencyContact)
    /// - AVR Score: 6/8 = 0.75 (75%)
    /// 
    /// This class demonstrates POOR security practices for educational analysis.
    /// </summary>
    public class PatientRecord
    {
        // ========== SAFE ATTRIBUTES (2) ==========

        /// <summary>
        /// Non-sensitive unique key identifier.
        /// </summary>
        public int PatientID { get; set; }

        /// <summary>
        /// Public name - considered non-sensitive.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        // ========== VULNERABLE ATTRIBUTES (4) - Intentionally exposed ==========

        /// <summary>
        /// PII - Date of Birth stored without encryption.
        /// VULNERABILITY: Plain text storage of personal information.
        /// </summary>
        public DateTime DOB { get; set; }

        /// <summary>
        /// HIGH RISK PII - Social Security Number stored in plain text!
        /// VULNERABILITY: Should be encrypted, masked, or not stored at all.
        /// </summary>
        public string SSN { get; set; } = string.Empty;

        /// <summary>
        /// PHI - Protected Health Information with no access control.
        /// VULNERABILITY: Medical data should have strict access controls.
        /// </summary>
        public string MedicalHistory { get; set; } = string.Empty;

        /// <summary>
        /// PII - Phone number stored without encryption.
        /// VULNERABILITY: Contact information should be protected.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// PII - Email address stored in plain text.
        /// VULNERABILITY: Can be used for phishing attacks.
        /// </summary>
        public string EmailAddress { get; set; } = string.Empty;

        /// <summary>
        /// PII - Emergency contact information.
        /// VULNERABILITY: Sensitive personal information exposed.
        /// </summary>
        public string EmergencyContact { get; set; } = string.Empty;



        // ========== METHODS ==========

        /// <summary>
        /// SAFE: Returns only non-sensitive data.
        /// VA Risk: LOW (0/2 = 0.00)
        /// </summary>
        /// <returns>Tuple containing only PatientID and FullName</returns>
        public (int id, string name) GetBasicInfo()
        {
            return (PatientID, FullName);
        }

        /// <summary>
        /// VULNERABLE: Accesses ALL attributes including sensitive ones.
        /// VA Risk: HIGH - Exposes SSN, DOB, MedicalHistory (3/5 = 0.60)
        /// </summary>
        /// <returns>Full patient report with all sensitive data exposed</returns>
        public string GenerateReport()
        {
            // BAD PRACTICE: Concatenating all sensitive data into a single string
            return $"Patient: {FullName}\n" +
                   $"SSN: {SSN}\n" +                    // VULNERABILITY
                   $"DOB: {DOB}\n" +                    // VULNERABILITY
                   $"History: {MedicalHistory}";    // VULNERABILITY
        }
    }
}
