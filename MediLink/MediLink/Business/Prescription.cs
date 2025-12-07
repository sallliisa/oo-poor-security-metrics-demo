using MediLink.Data;
using MediLink.Services;

namespace MediLink.Business
{
    /// <summary>
    /// Central hub linking Patient, Doctor, and Drug.
    /// PRIMARY TARGET for CIVPF and VCC analysis.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 6
    /// - Vulnerable Attributes: 2 (DoctorAuthToken, RawPatientSSN)
    /// - AVR Score: 2/6 = 0.333 (33%)
    /// 
    /// VCC Calculation:
    /// - Coupled with: PatientRecord, Doctor, PharmacyAdapter
    /// - VCC Score: 3 (All vulnerable couplings)
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

        // ========== COUPLING REFERENCES (VCC +3) ==========

        /// <summary>
        /// Reference to patient record - VCC coupling point.
        /// </summary>
        private PatientRecord? _patient;

        /// <summary>
        /// Reference to doctor - VCC coupling point.
        /// </summary>
        private Doctor? _doctor;

        /// <summary>
        /// Reference to pharmacy adapter - VCC coupling point.
        /// </summary>
        private PharmacyAdapter? _pharmacy;

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
        /// VULNERABLE: Returns ALL attributes including sensitive ones.
        /// VA Risk: CRITICAL (2/6 = 0.33) - Exposes SSN and AuthToken to external parties.
        /// Used by PharmacyAdapter, extending the CIVPF chain.
        /// </summary>
        /// <returns>DTO containing sensitive patient and doctor data</returns>
        public PrescriptionDTO GetForPharmacy()
        {
            // BAD PRACTICE: Sending sensitive data to external system
            return new PrescriptionDTO
            {
                PrescriptionID = PrescriptionID,
                PatientID = PatientID,
                DrugName = DrugName,
                Dosage = Dosage,
                PatientSSN = RawPatientSSN,        // VULNERABILITY: SSN to pharmacy
                AuthToken = DoctorAuthToken        // VULNERABILITY: Token exposure
            };
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
        /// Sets the pharmacy adapter for transmission.
        /// Increases VCC coupling.
        /// </summary>
        /// <param name="pharmacy">Pharmacy adapter instance</param>
        public void SetPharmacy(PharmacyAdapter pharmacy)
        {
            _pharmacy = pharmacy;
        }

        /// <summary>
        /// VULNERABLE: Transmits prescription through pharmacy adapter.
        /// This method chains the CIVPF propagation to the external system.
        /// </summary>
        /// <returns>True if transmission succeeds</returns>
        public bool Transmit()
        {
            if (_pharmacy == null)
            {
                throw new InvalidOperationException("Pharmacy not set");
            }

            // BAD PRACTICE: Sending prescription with all sensitive data
            return _pharmacy.TransmitPrescription(this);
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
