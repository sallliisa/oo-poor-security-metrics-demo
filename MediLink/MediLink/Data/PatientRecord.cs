namespace MediLink.Data
{
    /// <summary>
    /// Stores patient information with INTENTIONALLY HIGH AVR (Attribute Vulnerability Ratio).
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 6
    /// - Vulnerable Attributes: 4 (DOB, SSN, MedicalHistory, CreditCardToken)
    /// - AVR Score: 4/6 = 0.667 (67%)
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
        /// PCI Data - Credit card token poorly named and not properly secured.
        /// VULNERABILITY: Payment information should be tokenized and secured.
        /// </summary>
        public string CreditCardToken { get; set; } = string.Empty;

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
        /// VA Risk: HIGH - Exposes SSN, DOB, MedicalHistory, CreditCardToken (4/6 = 0.67)
        /// </summary>
        /// <returns>Full patient report with all sensitive data exposed</returns>
        public string GenerateReport()
        {
            // BAD PRACTICE: Concatenating all sensitive data into a single string
            return $"Patient: {FullName}\n" +
                   $"SSN: {SSN}\n" +                    // VULNERABILITY
                   $"DOB: {DOB}\n" +                    // VULNERABILITY
                   $"History: {MedicalHistory}\n" +    // VULNERABILITY
                   $"Payment: {CreditCardToken}";      // VULNERABILITY
        }

        /// <summary>
        /// VULNERABLE: Returns raw SSN without masking.
        /// VA Risk: HIGH (1/1 = 1.00)
        /// </summary>
        /// <returns>Unmasked, unprotected SSN</returns>
        public string GetSSN()
        {
            // BAD PRACTICE: No masking, no audit log, direct exposure
            return SSN;
        }

        /// <summary>
        /// VULNERABLE: Updates SSN without validation.
        /// </summary>
        /// <param name="newSSN">New SSN value - no validation performed</param>
        public void UpdateSSN(string newSSN)
        {
            // BAD PRACTICE: No format validation, no audit logging
            SSN = newSSN;
        }

        /// <summary>
        /// VULNERABLE: Provides full data dump for export.
        /// VA Risk: HIGH
        /// </summary>
        /// <returns>Dictionary containing all patient data</returns>
        public Dictionary<string, object> ExportAllData()
        {
            // BAD PRACTICE: Exporting all sensitive data without redaction
            return new Dictionary<string, object>
            {
                { "PatientID", PatientID },
                { "FullName", FullName },
                { "DOB", DOB },                         // VULNERABILITY
                { "SSN", SSN },                         // VULNERABILITY
                { "MedicalHistory", MedicalHistory },   // VULNERABILITY
                { "CreditCardToken", CreditCardToken }  // VULNERABILITY
            };
        }
    }
}
