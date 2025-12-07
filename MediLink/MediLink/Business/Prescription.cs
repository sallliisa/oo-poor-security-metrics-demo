using MediLink.Data;

namespace MediLink.Business
{
    /// <summary>
    /// Central hub linking Patient, Doctor, and Drug.
    /// Demonstrates data duplication vulnerabilities.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 5
    /// - Vulnerable Attributes: 1 (DrugCost)
    /// - AVR Score: 1/5 = 0.20 (20%)
    /// 
    /// VCC Calculation:
    /// - Coupled with: PatientRecord, Doctor
    /// - VCC Score: 2 (Safe couplings - no data duplication)
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

        /// <summary>
        /// VULNERABLE: Drug cost stored without encryption.
        /// VULNERABILITY: Financial information should be protected.
        /// </summary>
        public decimal DrugCost { get; set; }



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
        /// SAFE: Creates prescription using references only.
        /// No data duplication - stores IDs only.
        /// </summary>
        /// <param name="patient">Patient record containing SSN</param>
        /// <param name="doctor">Doctor containing AuthToken</param>
        public void Create(PatientRecord patient, Doctor doctor)
        {
            _patient = patient;
            _doctor = doctor;

            // GOOD PRACTICE: Only storing IDs, not copying sensitive data
            PrescriptionID = Guid.NewGuid();
            PatientID = patient.PatientID;
        }



        /// <summary>
        /// SAFE: Debug print without exposing sensitive data.
        /// </summary>
        public void DebugPrint()
        {
            // GOOD PRACTICE: Only logging non-sensitive identifiers
            Console.WriteLine($"[DEBUG] Prescription ID: {PrescriptionID}, Patient ID: {PatientID}");
        }

        /// <summary>
        /// VULNERABLE: Exposes financial information in logs.
        /// VA Risk: MEDIUM (1/1 = 1.00)
        /// </summary>
        /// <returns>Total cost of the prescription</returns>
        public decimal CalculateTotalCost()
        {
            // BAD PRACTICE: Logging financial information
            Console.WriteLine($"[COST] Prescription {PrescriptionID}: ${DrugCost}");
            return DrugCost;
        }


    }
}
