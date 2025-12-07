using MediLink.Data;
using MediLink.Business;
using MediLink.Services;
using MediLink.Utilities;

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

            // Initialize audit logger (VULNERABLE: logs sensitive data)
            var logger = new AuditLogger
            {
                LogFilePath = "medilink_audit.log",
                VerboseMode = true  // BAD PRACTICE: Enables credential logging
            };

            // ========== DEMO: Patient Record (High AVR) ==========
            Console.WriteLine("--- Creating Patient Record ---");

            var patient = new PatientRecord
            {
                PatientID = 1001,
                FullName = "John Doe",
                DOB = new DateTime(1985, 3, 15),
                SSN = "123-45-6789",             // BAD: Plain text SSN
                MedicalHistory = "Hypertension, Diabetes Type 2",
                CreditCardToken = "4532-XXXX-XXXX-1234"  // BAD: Card data stored
            };

            // VULNERABLE: Logging patient with sensitive data
            logger.LogPatientAccess(patient, "CREATED");

            // VULNERABLE: GenerateReport exposes all sensitive data
            Console.WriteLine("\n[VULNERABLE] Patient Report:");
            Console.WriteLine(patient.GenerateReport());

            // ========== DEMO: Doctor (50% AVR with credentials) ==========
            Console.WriteLine("\n--- Creating Doctor Record ---");

            var doctor = new Doctor
            {
                DoctorID = 5001,
                FullName = "Dr. Jane Smith",
                Specialty = "Internal Medicine",
                AuthToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.secret", // BAD: Plain text token
                PrivateKey = "-----BEGIN RSA PRIVATE KEY-----MIIEpQIB...",   // BAD: Exposed key
                Password = "SecurePassword123!"  // CRITICAL: Plain text password!
            };

            // VULNERABLE: Log doctor activity with credentials
            logger.LogDoctorActivity(doctor, "CREATED");

            // VULNERABLE: Password validation with plain text
            Console.WriteLine($"\n[VULNERABLE] Password validation: {doctor.ValidatePassword("SecurePassword123!")}");

            // VULNERABLE: Expose auth token
            Console.WriteLine($"[VULNERABLE] Auth Token: {doctor.GetAuthToken()}");

            // ========== DEMO: Prescription (CIVPF and VCC Hub) ==========
            Console.WriteLine("\n--- Creating Prescription ---");

            var prescription = new Prescription
            {
                DrugName = "Metformin 500mg",
                Dosage = "Take twice daily with meals"
            };

            // VULNERABLE: CIVPF propagation - sensitive data flows from Patient & Doctor
            prescription.Create(patient, doctor);

            // VULNERABLE: Debug print exposes SSN and AuthToken
            Console.WriteLine("\n[VULNERABLE] Debug Print:");
            prescription.DebugPrint();

            // Log prescription (VULNERABLE: logs sensitive data)
            logger.LogPrescription(prescription);

            // ========== DEMO: Pharmacy Adapter (External Transmission) ==========
            Console.WriteLine("\n--- Pharmacy Adapter (External System) ---");

            var pharmacy = new PharmacyAdapter
            {
                PharmacyID = 7001,
                PharmacyName = "HealthFirst Pharmacy",
                APIKey = "pk_live_abc123xyz789_SENSITIVE",  // BAD: Plain text API key
                ConnectionString = "Server=prod-db;Database=Pharmacy;User=admin;Password=DbPass123!" // BAD!
            };

            prescription.SetPharmacy(pharmacy);

            // VULNERABLE: Transmit prescription (final CIVPF hop - data leaves system)
            Console.WriteLine("\n[VULNERABLE] Transmitting to External Pharmacy:");
            pharmacy.TransmitPrescription(prescription);

            // VULNERABLE: SQL Injection demonstration
            Console.WriteLine("\n[VULNERABLE] SQL Injection Risk:");
            pharmacy.VerifyPatientID("123-45-6789"); // Logged in plain text

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

            // VULNERABLE: Export doctor credentials
            Console.WriteLine("\n[VULNERABLE] Doctor Credentials Export:");
            var credentials = doctor.ExportCredentials();
            foreach (var kvp in credentials)
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
            Console.WriteLine("Doctor               | 0.50  | 0   | 🔴 POOR");
            Console.WriteLine("Prescription         | 0.33  | 3   | 🔴 POOR");
            Console.WriteLine("PharmacyAdapter      | 0.50  | 1   | 🔴 POOR");
            Console.WriteLine("AuditLogger          | 0.50  | 2   | 🔴 POOR");
            Console.WriteLine("---------------------|-------|-----|--------");
            Console.WriteLine("System AVR           | 0.50  |     | 🔴 POOR");
            Console.WriteLine("System VCC           |       | 6   | 🔴 POOR");
            Console.WriteLine("Max CIVPF Path       |       | 3   | 🔴 POOR");
            Console.WriteLine();
            Console.WriteLine("Critical VA Methods (≥0.50): 6 methods");
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
