namespace MediLink.Data
{
    /// <summary>
    /// Reference data for medical specialties.
    /// SAFE CLASS: Contains only public reference data, no PII/PHI.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 3
    /// - Vulnerable Attributes: 0
    /// - AVR Score: 0/3 = 0.00 (0%)
    /// </summary>
    public class MedicalSpecialty
    {
        /// <summary>
        /// Unique identifier for the specialty.
        /// </summary>
        public int SpecialtyID { get; set; }

        /// <summary>
        /// Name of the specialty (e.g., "Cardiology", "Neurology").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the specialty.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// SAFE: Returns formatted specialty name.
        /// </summary>
        public string GetDisplayName()
        {
            return $"{Name} - {Description}";
        }
    }
}
