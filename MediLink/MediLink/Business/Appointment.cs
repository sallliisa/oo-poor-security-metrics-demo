using MediLink.Data;

namespace MediLink.Business
{
    /// <summary>
    /// Represents a medical appointment.
    /// SAFE CLASS: Uses ID references only, no sensitive data duplication.
    /// 
    /// AVR Calculation:
    /// - Total Attributes: 7
    /// - Vulnerable Attributes: 0
    /// - AVR Score: 0/7 = 0.00 (0%)
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// Unique identifier for the appointment.
        /// </summary>
        public Guid AppointmentID { get; set; }

        /// <summary>
        /// Reference to patient by ID only (SAFE - no PII copied).
        /// </summary>
        public int PatientID { get; set; }

        /// <summary>
        /// Reference to doctor by ID only (SAFE - no credentials copied).
        /// </summary>
        public int DoctorID { get; set; }

        /// <summary>
        /// Scheduled date and time.
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// Duration in minutes.
        /// </summary>
        public int Duration { get; set; } = 30;

        /// <summary>
        /// Appointment status (e.g., "Scheduled", "Completed", "Cancelled").
        /// </summary>
        public string Status { get; set; } = "Scheduled";

        /// <summary>
        /// Non-medical notes (e.g., "Follow-up visit").
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// SAFE: Schedules the appointment.
        /// </summary>
        public void Schedule(int patientId, int doctorId, DateTime date, int duration)
        {
            AppointmentID = Guid.NewGuid();
            PatientID = patientId;
            DoctorID = doctorId;
            AppointmentDate = date;
            Duration = duration;
            Status = "Scheduled";
        }

        /// <summary>
        /// SAFE: Cancels the appointment.
        /// </summary>
        public void Cancel()
        {
            Status = "Cancelled";
        }

        /// <summary>
        /// SAFE: Reschedules to a new date/time.
        /// </summary>
        public void Reschedule(DateTime newDate)
        {
            AppointmentDate = newDate;
            Status = "Rescheduled";
        }

        /// <summary>
        /// SAFE: Marks appointment as completed.
        /// </summary>
        public void Complete()
        {
            Status = "Completed";
        }
    }
}
