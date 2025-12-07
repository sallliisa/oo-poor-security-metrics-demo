using MediLink.Data;
using MediLink.Business;

namespace MediLink.Utilities
{
    /// <summary>
    /// Logging utility that IMPROPERLY handles sensitive data.
    /// Demonstrates how logging can amplify vulnerability exposure.
    /// 
    /// VCC: Coupled with PatientRecord and Prescription
    /// </summary>
    public class AuditLogger
    {
        // ========== VULNERABLE ATTRIBUTES (1) ==========

        /// <summary>
        /// Log file path - could expose file system structure.
        /// VULNERABILITY: Path should not be publicly accessible.
        /// </summary>
        public string LogFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Flag to enable verbose logging - increases exposure.
        /// </summary>
        public bool VerboseMode { get; set; } = true;

        // ========== METHODS ==========

        /// <summary>
        /// VULNERABLE: Logs patient data without sanitization.
        /// VA Risk: HIGH - Writes SSN and DOB to log file.
        /// </summary>
        /// <param name="patient">Patient record with sensitive data</param>
        /// <param name="action">Action being performed</param>
        public void LogPatientAccess(PatientRecord patient, string action)
        {
            // BAD PRACTICE: Logging all patient data including sensitive fields
            var logEntry = $"[{DateTime.Now}] {action}: " +
                          $"ID={patient.PatientID}, " +
                          $"Name={patient.FullName}, " +
                          $"SSN={patient.SSN}, " +           // VULNERABILITY: Logging SSN
                          $"DOB={patient.DOB}";              // VULNERABILITY: Logging DOB

            WriteLog(logEntry);
        }

        /// <summary>
        /// VULNERABLE: Logs prescription with sensitive data.
        /// VA Risk: HIGH - Logs auth token and SSN.
        /// </summary>
        /// <param name="rx">Prescription with sensitive data</param>
        public void LogPrescription(Prescription rx)
        {
            // BAD PRACTICE: Logging auth token and SSN
            var logEntry = $"[{DateTime.Now}] Prescription: " +
                          $"RxID={rx.PrescriptionID}, " +
                          $"PatientID={rx.PatientID}, " +
                          $"Drug={rx.DrugName}, " +
                          $"SSN={rx.RawPatientSSN}, " +      // VULNERABILITY: Logging SSN
                          $"Token={rx.DoctorAuthToken}";     // VULNERABILITY: Logging AuthToken

            WriteLog(logEntry);
        }

        /// <summary>
        /// VULNERABLE: Logs doctor credentials.
        /// VA Risk: CRITICAL - Logs password and private key.
        /// </summary>
        /// <param name="doctor">Doctor with credentials</param>
        /// <param name="action">Action being performed</param>
        public void LogDoctorActivity(Doctor doctor, string action)
        {
            // BAD PRACTICE: Logging credentials
            var logEntry = $"[{DateTime.Now}] Doctor Activity: " +
                          $"DoctorID={doctor.DoctorID}, " +
                          $"Name={doctor.FullName}, " +
                          $"Action={action}";

            // EXTREMELY BAD PRACTICE: Verbose mode logs credentials
            if (VerboseMode)
            {
                logEntry += $", Token={doctor.AuthToken}";       // VULNERABILITY
                logEntry += $", PrivateKey={doctor.PrivateKey}"; // VULNERABILITY
            }

            WriteLog(logEntry);
        }

        /// <summary>
        /// VULNERABLE: Logs authentication attempts with passwords.
        /// VA Risk: CRITICAL
        /// </summary>
        /// <param name="doctorId">Doctor attempting login</param>
        /// <param name="password">Password used (should NEVER be logged)</param>
        /// <param name="success">Whether login succeeded</param>
        public void LogAuthAttempt(int doctorId, string password, bool success)
        {
            // EXTREMELY BAD PRACTICE: Logging passwords
            var logEntry = $"[{DateTime.Now}] Auth Attempt: " +
                          $"DoctorID={doctorId}, " +
                          $"Password={password}, " +         // CRITICAL VULNERABILITY
                          $"Success={success}";

            WriteLog(logEntry);
        }

        /// <summary>
        /// SAFE: Logs only non-sensitive transaction data.
        /// VA Risk: LOW
        /// </summary>
        /// <param name="transactionId">Transaction identifier</param>
        /// <param name="action">Action performed</param>
        public void LogTransaction(Guid transactionId, string action)
        {
            var logEntry = $"[{DateTime.Now}] Transaction: ID={transactionId}, Action={action}";
            WriteLog(logEntry);
        }

        /// <summary>
        /// VULNERABLE: Exports all logs (may contain sensitive data).
        /// </summary>
        /// <returns>All log contents</returns>
        public string ExportLogs()
        {
            // BAD PRACTICE: Returning all logs which may contain sensitive data
            if (File.Exists(LogFilePath))
            {
                return File.ReadAllText(LogFilePath);
            }
            return string.Empty;
        }

        /// <summary>
        /// Writes log entry to file.
        /// </summary>
        /// <param name="logEntry">Log entry text</param>
        private void WriteLog(string logEntry)
        {
            // Also output to console for demonstration
            Console.WriteLine($"[LOG] {logEntry}");

            // Write to file if path is set
            if (!string.IsNullOrEmpty(LogFilePath))
            {
                try
                {
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    // BAD PRACTICE: Swallowing exception
                    Console.WriteLine($"Log write failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// VULNERABLE: Returns the log file path.
        /// </summary>
        /// <returns>Path to log file</returns>
        public string GetLogPath()
        {
            return LogFilePath;
        }
    }
}
