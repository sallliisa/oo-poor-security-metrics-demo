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
            Console.WriteLine("   MediLink - Telemedicine System v1.0  ");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine("⚠️  WARNING: This is an INSECURE demo application!");
            Console.WriteLine("    For educational security metric analysis only.");
            Console.WriteLine();

            // ========== DEMO: Patient Record (High AVR) ==========
            Console.WriteLine("--- Creating Patient Record ---");

            var patient = new PatientRecord
            {
                PatientID = 1001,
                FullName = "John Doe",
                DOB = new DateTime(1985, 3, 15),
                SSN = "123-45-6789",             // BAD: Plain text SSN
                MedicalHistory = "Hypertension, Diabetes Type 2",
                CardToken = "4532-XXXX-XXXX-1234"  // BAD: Card data stored
            };

            // VULNERABLE: GenerateReport exposes all sensitive data
            Console.WriteLine("\n[VULNERABLE] Patient Report:");
            Console.WriteLine(patient.GenerateReport());

            // ========== DEMO: Doctor (40% AVR with credentials) ==========
            Console.WriteLine("\n--- Creating Doctor Record ---");

            var doctor = new Doctor
            {
                DoctorID = 5001,
                FullName = "Dr. Jane Smith",
                Specialty = "Internal Medicine",
                AuthToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.secret", // BAD: Plain text token
                Password = "SecurePassword123!"  // CRITICAL: Plain text password!
            };

            // VULNERABLE: Password validation with plain text
            Console.WriteLine($"\n[VULNERABLE] Password validation: {doctor.ValidatePassword("SecurePassword123!")}");

            // VULNERABLE: Expose auth token
            Console.WriteLine($"[VULNERABLE] Auth Token: {doctor.GetAuthToken()}");

            // ========== DEMO: Prescription (Data Duplication) ==========
            Console.WriteLine("\n--- Creating Prescription ---");

            var prescription = new Prescription
            {
                DrugName = "Metformin 500mg",
                Dosage = "Take twice daily with meals"
            };

            // VULNERABLE: Data duplication - sensitive data flows from Patient & Doctor
            prescription.Create(patient, doctor);

            // VULNERABLE: Debug print exposes SSN and AuthToken
            Console.WriteLine("\n[VULNERABLE] Debug Print:");
            prescription.DebugPrint();

            // ========== DEMO: Pharmacy Adapter (Fulfillment) ==========
            Console.WriteLine("\n--- Pharmacy Adapter (Fulfillment) ---");

            var pharmacy = new PharmacyAdapter
            {
                PharmacyID = 7001,
                PharmacyName = "HealthFirst Pharmacy",
                ConnectionString = "Server=prod-db;Database=Pharmacy;User=admin;Password=DbPass123!" // BAD!
            };

            // SIMPLIFIED: Pharmacist just receives and marks as fulfilled
            Console.WriteLine("\n[SIMPLIFIED] Fulfilling Prescription:");
            pharmacy.FulfillPrescription(prescription);

            // VULNERABLE: SQL Injection demonstration
            Console.WriteLine("\n[VULNERABLE] SQL Injection Risk:");
            pharmacy.VerifyPatientID("123-45-6789");

            // Demonstrate SQL injection attack vector
            Console.WriteLine("\n[ATTACK] SQL Injection attempt:");
            pharmacy.VerifyPatientID("' OR '1'='1' --"); // This would bypass authentication!

            // SAFE: Only safe transaction logging
            Console.WriteLine("\n[SAFE] Transaction Logging:");
            pharmacy.LogTransaction(prescription.PrescriptionID, DateTime.Now);

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
            Console.WriteLine("PatientRecord        | 0.67  | 0   | 🔴 POOR");
            Console.WriteLine("Doctor               | 0.40  | 0   | ⚠️  MODERATE");
            Console.WriteLine("Prescription         | 0.33  | 2   | ⚠️  MODERATE");
            Console.WriteLine("PharmacyAdapter      | 0.33  | 1   | ⚠️  MODERATE");
            Console.WriteLine("---------------------|-------|-----|--------");
            Console.WriteLine("System AVR           | 0.43  |     | ⚠️  MODERATE");
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
