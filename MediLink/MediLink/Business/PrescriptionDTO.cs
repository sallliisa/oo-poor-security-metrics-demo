namespace MediLink.Business
{
    /// <summary>
    /// Data Transfer Object that carries vulnerability downstream.
    /// This DTO propagates sensitive data to external systems.
    /// 
    /// VULNERABILITY: Contains SSN and AuthToken that should never be 
    /// transmitted to external pharmacy systems.
    /// </summary>
    public class PrescriptionDTO
    {
        /// <summary>
        /// Prescription unique identifier - SAFE.
        /// </summary>
        public Guid PrescriptionID { get; set; }

        /// <summary>
        /// Patient ID reference - SAFE (just an ID).
        /// </summary>
        public int PatientID { get; set; }

        /// <summary>
        /// Name of the prescribed drug - SAFE.
        /// </summary>
        public string DrugName { get; set; } = string.Empty;

        /// <summary>
        /// Dosage instructions - SAFE.
        /// </summary>
        public string Dosage { get; set; } = string.Empty;

        /// <summary>
        /// VULNERABLE: Patient's SSN is included in the DTO.
        /// This should NEVER be sent to external pharmacy systems.
        /// </summary>
        public string PatientSSN { get; set; } = string.Empty;

        /// <summary>
        /// VULNERABLE: Doctor's auth token is included in the DTO.
        /// This exposes internal authentication to external systems.
        /// </summary>
        public string AuthToken { get; set; } = string.Empty;

        /// <summary>
        /// VULNERABLE: Converts DTO to JSON string exposing all data.
        /// </summary>
        /// <returns>JSON representation with all fields</returns>
        public string ToJson()
        {
            // BAD PRACTICE: Manual JSON creation exposing sensitive data
            return $@"{{
    ""prescriptionId"": ""{PrescriptionID}"",
    ""patientId"": {PatientID},
    ""drugName"": ""{DrugName}"",
    ""dosage"": ""{Dosage}"",
    ""patientSSN"": ""{PatientSSN}"",
    ""authToken"": ""{AuthToken}""
}}";
        }
    }
}
