using MediLink.Data;

namespace MediLink.Business
{
    /// <summary>
    /// Central hub linking Patient, Doctor, and Drug.
    /// Demonstrates data duplication vulnerabilities.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 6
    /// - Vulnerable Attributes: 2 (DoctorAuthToken, RawPatientSSN)
    /// - AVR Score: 2/6 = 0.333 (33%)
    /// 
    /// VCC Calculation:
    /// - Coupled with: PatientRecord, Doctor
    /// - VCC Score: 2 (Vulnerable couplings)
    /// </summary>
    public class Prescription
    {
        // ========== SAFE ATTRIBUTES (4) ==========

        /// <summary>
        /// Unique identifier for the prescription.
        /// </summary>
        public Guid PrescriptionID { get; set; }

        /// <summary>
        /// Foreign key reference to patient (just an ID).
        /// </summary>
        public int PatientID { get; set; }

        /// <summary>
        /// Public drug name.
        /// </summary>
        public string DrugName { get; set; } = string.Empty;

        /// <summary>
        /// Dosage information.
        /// </summary>
        public string Dosage { get; set; } = string.Empty;

        // ========== VULNERABLE ATTRIBUTES (2) - Intentionally duplicated sensitive data ==========

        /// <summary>
        /// HIGH PRIVILEGE - Doctor's auth token should NEVER be stored here!
        /// VULNERABILITY: Token duplication increases attack surface.
        /// </summary>
        public string DoctorAuthToken { get; set; } = string.Empty;

        /// <summary>
        /// CRITICAL FLAW: SSN copied for "convenience".
        /// VULNERABILITY: Data duplication amplifies breach impact.
        /// </summary>
        public string RawPatientSSN { get; set; } = string.Empty;

        // ========== COUPLING REFERENCES (VCC +2) ==========

        /// <summary>
        /// Reference to patient record - VCC coupling point.
        /// </summary>
        private PatientRecord? _patient;

        /// <summary>
        /// Reference to doctor - VCC coupling point.
        /// </summary>
        private Doctor? _doctor;

        // ========== METHODS ==========

        /// <summary>
        /// VULNERABLE: Creates prescription by accessing sensitive data from multiple sources.
        /// CIVPF Risk: HIGH - Propagates SSN and AuthToken.
        /// VCC Impact: Couples with PatientRecord and Doctor.
        /// VA Score: 2/4 = 0.50
        /// </summary>
        /// <param name="patient">Patient record containing SSN</param>
        /// <param name="doctor">Doctor containing AuthToken</param>
        public void Create(PatientRecord patient, Doctor doctor)
        {
            _patient = patient;
            _doctor = doctor;

            // BAD PRACTICE: Copying sensitive data to this class
            // This creates data duplication and increases CIVPF
            RawPatientSSN = patient.SSN;           // CIVPF propagation!
            DoctorAuthToken = doctor.AuthToken;    // CIVPF propagation!

            PrescriptionID = Guid.NewGuid();
            PatientID = patient.PatientID;
        }

        /// <summary>
        /// VULNERABLE: Validates using sensitive data.
        /// VA Risk: MEDIUM (1/1 = 1.00)
        /// </summary>
        /// <param name="inputSSN">SSN to validate against</param>
        /// <returns>True if SSN matches</returns>
        public bool ValidateWithSSN(string inputSSN)
        {
            // BAD PRACTICE: Direct string comparison of SSN
            // No timing-safe comparison, vulnerable to timing attacks
            return RawPatientSSN == inputSSN;
        }

        /// <summary>
        /// VULNERABLE: Logs sensitive data to console.
        /// VA Risk: HIGH (2/2 = 1.00)
        /// </summary>
        public void DebugPrint()
        {
            // BAD PRACTICE: Logging sensitive data to console/stdout
            Console.WriteLine($"[DEBUG] SSN: {RawPatientSSN}, Token: {DoctorAuthToken}");
        }

        /// <summary>
        /// VULNERABLE: Returns sensitive data for "verification".
        /// </summary>
        /// <returns>Tuple containing SSN and AuthToken</returns>
        public (string ssn, string token) GetSensitiveData()
        {
            // BAD PRACTICE: Bundling sensitive data for easy extraction
            return (RawPatientSSN, DoctorAuthToken);
        }
    }
}
