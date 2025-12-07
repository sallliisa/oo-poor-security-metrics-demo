using MediLink.Business;

namespace MediLink.Services
{
    /// <summary>
    /// Handles prescription fulfillment at the pharmacy.
    /// Simplified to demonstrate that pharmacist just receives and marks prescriptions as fulfilled.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 3
    /// - Vulnerable Attributes: 1 (ConnectionString)
    /// - AVR Score: 1/3 = 0.33 (33%)
    /// 
    /// This class demonstrates simplified pharmacy operations.
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

        // ========== VULNERABLE ATTRIBUTES (1) ==========

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
        /// SIMPLIFIED: Receives prescription and marks it as fulfilled.
        /// No external transmission - pharmacist just processes it internally.
        /// </summary>
        /// <param name="prescription">Prescription to fulfill</param>
        /// <returns>True if fulfillment succeeds</returns>
        public bool FulfillPrescription(Prescription prescription)
        {
            _prescription = prescription;

            // Simple fulfillment - just mark as received
            Console.WriteLine($"Prescription {prescription.PrescriptionID} received and marked as FULFILLED");
            Console.WriteLine($"Drug: {prescription.DrugName}, Dosage: {prescription.Dosage}");

            return true;
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

            Console.WriteLine($"Verifying patient with SSN");

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
                Console.WriteLine($"Bulk verifying patient");
                if (VerifyPatientID(ssn))
                {
                    verified++;
                }
            }
            return verified;
        }
    }
}
