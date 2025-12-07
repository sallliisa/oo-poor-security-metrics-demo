using MediLink.Data;
using MediLink.Business;
using MediLink.Services;

namespace MediLink
{
    /// <summary>
    /// MediLink: Telemedicine & Prescription Management System
    /// 
    /// ⚠️ WARNING: This application intentionally demonstrates POOR security practices
    /// for educational analysis of security metrics (AVR, CIVPF, VCC, VA).
    /// 
    /// DO NOT use this code as a template for production systems.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   MediLink - Telemedicine System v2.0  ");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine("⚠️  WARNING: This is an INSECURE demo application!");
            Console.WriteLine("    For educational security metric analysis only.");
            Console.WriteLine();

            // ========== DEMO: Admin User (Inherited Vulnerabilities) ==========
            Console.WriteLine("--- Creating Admin User ---");
            var admin = new Admin
            {
                UserID = 1,
                FullName = "System Administrator",
                AuthToken = "ADMIN_TOKEN_12345",
                Password = "AdminPassword123!",
                Role = "SuperAdmin"
            };
            Console.WriteLine($"[VULNERABLE] Admin created with plain text password: {admin.Password}");

            // ========== DEMO: Medical Specialty (Safe Reference Data) ==========
            Console.WriteLine("\n--- Creating Medical Specialty ---");

            var specialty = new MedicalSpecialty
            {
                SpecialtyID = 1,
                Name = "Internal Medicine",
                Description = "Diagnosis and treatment of adult diseases"
            };
            Console.WriteLine($"[SAFE] Specialty: {specialty.GetDisplayName()}");

            // ========== DEMO: Patient Record (High AVR) ==========
            Console.WriteLine("\n--- Creating Patient Record ---");

            var patient = new PatientRecord
            {
                PatientID = 1001,
                FullName = "John Doe",
                DOB = new DateTime(1985, 3, 15),
                SSN = "123-45-6789",             // BAD: Plain text NIK
                MedicalHistory = "Hypertension, Diabetes Type 2",
                PhoneNumber = "+1-555-0123",      // BAD: Plain text PII
                EmailAddress = "john.doe@email.com", // BAD: Plain text PII
                EmergencyContact = "Jane Doe: +1-555-0124" // BAD: Plain text PII
            };

            Console.WriteLine("\n[VULNERABLE] Patient Report:");
            Console.WriteLine(patient.GenerateReport());

            // ========== DEMO: Doctor (User Inheritance) ==========
            Console.WriteLine("\n--- Creating Doctor Record ---");

            var doctor = new Doctor
            {
                UserID = 5001,
                FullName = "Dr. Jane Smith",
                SpecialtyID = 1,  // Reference to specialty by ID
                MedicalLicenseNumber = "MD-12345-NY", // BAD: Plain text credential
                AuthToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.secret", // BAD: Plain text token
                Password = "SecurePassword123!"  // CRITICAL: Plain text password!
            };
            Console.WriteLine($"[SAFE] Doctor assigned to specialty ID: {doctor.SpecialtyID}");

            // VULNERABLE: Password validation with plain text (inherited from User)
            Console.WriteLine($"\n[VULNERABLE] Password validation: {doctor.ValidatePassword("SecurePassword123!")}");

            // VULNERABLE: Expose auth token (inherited from User)
            Console.WriteLine($"[VULNERABLE] Auth Token: {doctor.GetAuthToken()}");

            // ========== DEMO: Prescription (Data Duplication) ==========
            Console.WriteLine("\n--- Creating Prescription ---");

            var prescription = new Prescription
            {
                DrugName = "Metformin 500mg",
                Dosage = "Take twice daily with meals",
                DrugCost = 45.99m  // BAD: Financial data
            };

            // SAFE: Data stored using references only
            prescription.Create(patient, doctor);

            // ========== DEMO: In-House Pharmacy Order ==========
            Console.WriteLine("\n--- In-House Pharmacy Order ---");

            // User decides to fulfill in-house
            var order = new PharmacyOrder(prescription, patient);
            
            // VULNERABLE: Order contains duplicated patient name
            Console.WriteLine($"[VULNERABLE] Order created for: {order.PatientName}");
            
            order.MarkFulfilled();

            // ========== DEMO: Appointment (Safe Class) ==========
            Console.WriteLine("\n--- Creating Appointment ---");

            var appointment = new Appointment();
            appointment.Schedule(
                patientId: patient.PatientID,
                doctorId: doctor.UserID,
                date: DateTime.Now.AddDays(7),
                duration: 30
            );
            Console.WriteLine($"[SAFE] Appointment {appointment.AppointmentID} scheduled");
            Console.WriteLine($"[SAFE] Patient ID: {appointment.PatientID}, Doctor ID: {appointment.DoctorID}");
            Console.WriteLine($"[SAFE] Status: {appointment.Status}");

            // ========== DEMO: Export Sensitive Data ==========
            Console.WriteLine("\n--- Sensitive Data Export ---");

            // VULNERABLE: Export all patient data
            Console.WriteLine("[VULNERABLE] Patient Data Export:");
            var patientData = patient.ExportAllData();
            foreach (var kvp in patientData)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }

            // ========== SUMMARY ==========
            Console.WriteLine("\n========================================");
            Console.WriteLine("        Security Metrics Summary        ");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine("Class                | AVR   | VCC | Status");
            Console.WriteLine("---------------------|-------|-----|--------");
            Console.WriteLine("PatientRecord        | 0.75  | 0   | 🔴 POOR");
            Console.WriteLine("Doctor (User)        | 0.50  | 0   | ⚠️  MODERATE");
            Console.WriteLine("Admin (User)         | 0.40  | 0   | ⚠️  MODERATE");
            Console.WriteLine("Prescription         | 0.20  | 2   | 🟢 GOOD");
            Console.WriteLine("PharmacyOrder        | 0.20  | 1   | 🟢 GOOD");
            Console.WriteLine("MedicalSpecialty     | 0.00  | 0   | 🟢 GOOD");
            Console.WriteLine("Appointment          | 0.00  | 0   | 🟢 GOOD");
            Console.WriteLine("---------------------|-------|-----|--------");
            Console.WriteLine("System AVR           | 0.31  |     | ⚠️  MODERATE");
            Console.WriteLine("System VCC           |       | 3   | ⚠️  MODERATE");
            Console.WriteLine("Max CIVPF Path       |       | 2   | ⚠️  MODERATE");
            Console.WriteLine();
            Console.WriteLine("Critical VA Methods (≥0.50): 4 methods");
            Console.WriteLine();
            Console.WriteLine("⚠️  This application is intentionally insecure.");
            Console.WriteLine("    Use only for educational security analysis.");
            Console.WriteLine();

            // Wait for user input before closing
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
