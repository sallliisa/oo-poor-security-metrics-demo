using MediLink.Business;

namespace MediLink.Services
{
    /// <summary>
    /// Handles external communication with pharmacy systems.
    /// PRIMARY TARGET for VA (Vulnerability Amplification) and downstream CIVPF analysis.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 4
    /// - Vulnerable Attributes: 2 (APIKey, ConnectionString)
    /// - AVR Score: 2/4 = 0.50 (50%)
    /// 
    /// This class demonstrates data leaving the system boundary.
    /// </summary>
    public class PharmacyAdapter
    {
        // ========== SAFE ATTRIBUTES (2) ==========

        /// <summary>
        /// Pharmacy identifier.
        /// </summary>
        public int PharmacyID { get; set; }

        /// <summary>
        /// Public pharmacy name.
        /// </summary>
        public string PharmacyName { get; set; } = string.Empty;

        // ========== VULNERABLE ATTRIBUTES (2) ==========

        /// <summary>
        /// API Key stored in plain text - should be in secure configuration.
        /// VULNERABILITY: API keys should use secure vault storage.
        /// </summary>
        public string APIKey { get; set; } = string.Empty;

        /// <summary>
        /// Database connection string with exposed credentials.
        /// VULNERABILITY: Connection strings should be encrypted/secured.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        // ========== COUPLING REFERENCES (VCC +1) ==========

        /// <summary>
        /// Reference to prescription - VCC coupling point.
        /// </summary>
        private Prescription? _prescription;

        // ========== METHODS ==========

        /// <summary>
        /// VULNERABLE: Transmits prescription including all sensitive data.
        /// CIVPF Risk: HIGH - Inherits vulnerability from Prescription.GetForPharmacy().
        /// VA Risk: CRITICAL (3/5 = 0.60) - External data transmission.
        /// 
        /// This is the final hop in the CIVPF chain before data leaves the system.
        /// </summary>
        /// <param name="prescription">Prescription with sensitive data</param>
        /// <returns>True if transmission succeeds</returns>
        public bool TransmitPrescription(Prescription prescription)
        {
            _prescription = prescription;

            // BAD PRACTICE: Getting full DTO with sensitive data
            var dto = prescription.GetForPharmacy();

            // BAD PRACTICE: Logging sensitive data before transmission
            Console.WriteLine($"Transmitting: SSN={dto.PatientSSN}, Token={dto.AuthToken}");

            // BAD PRACTICE: Including API key in logs
            Console.WriteLine($"Using API Key: {APIKey}");

            // BAD PRACTICE: No encryption, sending to external system
            return SendToExternalAPI(dto);
        }

        /// <summary>
        /// VULNERABLE: Verifies patient using SSN with SQL Injection vulnerability!
        /// VA Risk: HIGH (1/1 = 1.00) - Direct SSN handling.
        /// </summary>
        /// <param name="ssn">Patient SSN for verification</param>
        /// <returns>True if patient is verified</returns>
        public bool VerifyPatientID(string ssn)
        {
            // BAD PRACTICE: SSN passed as plain string parameter
            // BAD PRACTICE: No input sanitization
            // CRITICAL: SQL INJECTION VULNERABILITY!
            var query = $"SELECT * FROM Patients WHERE SSN = '{ssn}'";

            // BAD PRACTICE: Logging SSN
            Console.WriteLine($"Verifying SSN: {ssn}");

            return ExecuteQuery(query);
        }

        /// <summary>
        /// SAFE: Only logs non-sensitive data.
        /// VA Risk: LOW (0/2 = 0.00)
        /// </summary>
        /// <param name="prescriptionId">Prescription GUID</param>
        /// <param name="timestamp">Transaction timestamp</param>
        public void LogTransaction(Guid prescriptionId, DateTime timestamp)
        {
            // Good practice: Only safe data logged
            Console.WriteLine($"Transaction: {prescriptionId} at {timestamp}");
        }

        /// <summary>
        /// VULNERABLE: Exposes connection string.
        /// VA Risk: MEDIUM (1/1 = 1.00)
        /// </summary>
        /// <returns>Raw connection string with credentials</returns>
        public string GetDatabaseConnection()
        {
            // BAD PRACTICE: Returning raw connection string
            return ConnectionString;
        }

        /// <summary>
        /// VULNERABLE: Exposes API key.
        /// VA Risk: MEDIUM (1/1 = 1.00)
        /// </summary>
        /// <returns>Raw API key</returns>
        public string GetAPIKey()
        {
            // BAD PRACTICE: Direct API key exposure
            return APIKey;
        }

        /// <summary>
        /// Simulated external API call - would send data to pharmacy.
        /// VULNERABILITY: No encryption, no secure channel verification.
        /// </summary>
        /// <param name="dto">Prescription DTO with sensitive data</param>
        /// <returns>True (simulated success)</returns>
        private bool SendToExternalAPI(PrescriptionDTO dto)
        {
            // BAD PRACTICE: No encryption, no secure channel
            // In reality, dto.PatientSSN and dto.AuthToken are being transmitted unsecurely

            // Simulated logging of the JSON payload (BAD PRACTICE)
            Console.WriteLine($"[EXTERNAL API] Payload: {dto.ToJson()}");

            return true;
        }

        /// <summary>
        /// Simulated SQL execution - vulnerable to SQL injection.
        /// </summary>
        /// <param name="query">Raw SQL query (unsanitized)</param>
        /// <returns>True (simulated success)</returns>
        private bool ExecuteQuery(string query)
        {
            // BAD PRACTICE: Direct SQL execution without parameterization
            Console.WriteLine($"[SQL] Executing: {query}");
            return true;
        }

        /// <summary>
        /// VULNERABLE: Bulk verification with multiple SSNs.
        /// </summary>
        /// <param name="ssnList">List of SSNs to verify</param>
        /// <returns>Count of verified patients</returns>
        public int BulkVerify(List<string> ssnList)
        {
            int verified = 0;
            foreach (var ssn in ssnList)
            {
                // BAD PRACTICE: Logging all SSNs
                Console.WriteLine($"Bulk verifying: {ssn}");
                if (VerifyPatientID(ssn))
                {
                    verified++;
                }
            }
            return verified;
        }
    }
}
