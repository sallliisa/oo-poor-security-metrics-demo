# 🏥 MediLink: Telemedicine & Prescription Management System
## Detailed Technical Specification Document

**Version:** 1.0  
**Platform:** C# / .NET  
**Architecture:** Three-Tier (Data → Logic → Presentation)

> [!CAUTION]
> This specification intentionally demonstrates **POOR** software security practices for educational analysis of AVR, CIVPF, VCC, and VA metrics.

---

## 1. System Overview

MediLink is a minimal telemedicine application designed to manage digital prescription workflows between patients, doctors, and pharmacies. This system intentionally contains security vulnerabilities for academic study of code quality metrics.

### 1.1 Metric Definitions

| Metric | Full Name | Description |
|--------|-----------|-------------|
| **AVR** | Attribute Vulnerability Ratio | Ratio of vulnerable attributes to total attributes in a class |
| **CIVPF** | Classified Information Vulnerability Propagation Factor | Measures how sensitive data propagates through class couplings |
| **VCC** | Vulnerable Class Coupling | Number of classes a vulnerable class is coupled with |
| **VA** | Vulnerability Amplification | How method access patterns amplify data exposure risk |

---

## 2. Core Classes Specification

### 2.1 PatientRecord (Data Layer)

**Purpose:** Store patient information with **intentionally high AVR**.

```csharp
namespace MediLink.Data
{
    public class PatientRecord
    {
        // ========== ATTRIBUTES ==========
        
        // SAFE ATTRIBUTES (2)
        public int PatientID { get; set; }              // Non-sensitive unique key
        public string FullName { get; set; }            // Public name
        
        // VULNERABLE ATTRIBUTES (4) - Intentionally exposed
        public DateTime DOB { get; set; }               // PII - No encryption
        public string SSN { get; set; }                 // HIGH RISK PII - Plain text!
        public string MedicalHistory { get; set; }      // PHI - No access control
        public string CreditCardToken { get; set; }     // PCI Data - Poorly named "token"
        
        // ========== METHODS ==========
        
        /// <summary>
        /// SAFE: Returns only non-sensitive data
        /// VA Risk: LOW
        /// </summary>
        public (int id, string name) GetBasicInfo()
        {
            return (PatientID, FullName);
        }
        
        /// <summary>
        /// VULNERABLE: Accesses ALL attributes including sensitive ones
        /// VA Risk: HIGH - Exposes SSN, DOB, MedicalHistory, CreditCardToken
        /// </summary>
        public string GenerateReport()
        {
            // BAD PRACTICE: Concatenating all sensitive data
            return $"Patient: {FullName}\n" +
                   $"SSN: {SSN}\n" +                    // VULNERABILITY
                   $"DOB: {DOB}\n" +                    // VULNERABILITY
                   $"History: {MedicalHistory}\n" +    // VULNERABILITY
                   $"Payment: {CreditCardToken}";      // VULNERABILITY
        }
        
        /// <summary>
        /// VULNERABLE: Returns raw SSN without masking
        /// VA Risk: HIGH
        /// </summary>
        public string GetSSN()
        {
            return SSN;  // No masking, no audit log
        }
    }
}
```

**AVR Calculation:**
| Category | Count |
|----------|-------|
| Total Attributes | 6 |
| Vulnerable Attributes | 4 |
| **AVR Score** | **4/6 = 0.667 (67%)** |

---

### 2.2 Prescription (Logic/Business Layer)

**Purpose:** Central hub linking Patient, Doctor, and Drug. **Primary target for CIVPF and VCC analysis.**

```csharp
namespace MediLink.Business
{
    public class Prescription
    {
        // ========== ATTRIBUTES ==========
        
        // SAFE ATTRIBUTES (4)
        public Guid PrescriptionID { get; set; }        // Unique identifier
        public int PatientID { get; set; }              // Foreign key only
        public string DrugName { get; set; }            // Public drug name
        public string Dosage { get; set; }              // Dosage info
        
        // VULNERABLE ATTRIBUTES (2) - Intentionally duplicated sensitive data
        public string DoctorAuthToken { get; set; }     // HIGH PRIVILEGE - Should never be stored here!
        public string RawPatientSSN { get; set; }       // CRITICAL FLAW: Copied for "convenience"
        
        // ========== COUPLING REFERENCES ==========
        private PatientRecord _patient;                  // VCC +1
        private Doctor _doctor;                          // VCC +1
        private PharmacyAdapter _pharmacy;               // VCC +1
        
        // ========== METHODS ==========
        
        /// <summary>
        /// VULNERABLE: Creates prescription by accessing sensitive data from multiple sources
        /// CIVPF Risk: HIGH - Propagates SSN and AuthToken
        /// VCC Impact: Couples with PatientRecord and Doctor
        /// </summary>
        public void Create(PatientRecord patient, Doctor doctor)
        {
            _patient = patient;
            _doctor = doctor;
            
            // BAD PRACTICE: Copying sensitive data to this class
            RawPatientSSN = patient.SSN;           // CIVPF propagation!
            DoctorAuthToken = doctor.AuthToken;    // CIVPF propagation!
            
            PrescriptionID = Guid.NewGuid();
            PatientID = patient.PatientID;
        }
        
        /// <summary>
        /// VULNERABLE: Returns ALL attributes including sensitive ones
        /// VA Risk: CRITICAL - Exposes SSN and AuthToken to external parties
        /// </summary>
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
        /// VULNERABLE: Validates using sensitive data
        /// VA Risk: MEDIUM
        /// </summary>
        public bool ValidateWithSSN(string inputSSN)
        {
            // BAD PRACTICE: Direct string comparison of SSN
            return RawPatientSSN == inputSSN;
        }
        
        /// <summary>
        /// VULNERABLE: Logs sensitive data to console
        /// VA Risk: HIGH
        /// </summary>
        public void DebugPrint()
        {
            // BAD PRACTICE: Logging sensitive data
            Console.WriteLine($"[DEBUG] SSN: {RawPatientSSN}, Token: {DoctorAuthToken}");
        }
    }
    
    // DTO that carries vulnerability downstream
    public class PrescriptionDTO
    {
        public Guid PrescriptionID { get; set; }
        public int PatientID { get; set; }
        public string DrugName { get; set; }
        public string Dosage { get; set; }
        public string PatientSSN { get; set; }     // VULNERABLE
        public string AuthToken { get; set; }      // VULNERABLE
    }
}
```

**VCC Calculation:**
| Coupled Class | Coupling Type | Vulnerability Status |
|---------------|---------------|---------------------|
| PatientRecord | Composition/Parameter | VULNERABLE (accesses SSN) |
| Doctor | Composition/Parameter | VULNERABLE (accesses AuthToken) |
| PharmacyAdapter | Composition | VULNERABLE (sends sensitive data) |
| **VCC Score** | | **3 (All vulnerable couplings)** |

**AVR Calculation:**
| Category | Count |
|----------|-------|
| Total Attributes | 6 |
| Vulnerable Attributes | 2 |
| **AVR Score** | **2/6 = 0.333 (33%)** |

---

### 2.3 Doctor (Data Layer)

**Purpose:** Store doctor information with authentication credentials.

```csharp
namespace MediLink.Data
{
    public class Doctor
    {
        // ========== ATTRIBUTES ==========
        
        // SAFE ATTRIBUTES (3)
        public int DoctorID { get; set; }               // Unique identifier
        public string FullName { get; set; }            // Public name
        public string Specialty { get; set; }           // Public info
        
        // VULNERABLE ATTRIBUTES (3)
        public string AuthToken { get; set; }           // HIGH PRIVILEGE - Plain text
        public string PrivateKey { get; set; }          // CRITICAL - Signing key exposed
        public string Password { get; set; }            // CRITICAL - Plain text password!
        
        // ========== METHODS ==========
        
        /// <summary>
        /// VULNERABLE: Validates password with plain text comparison
        /// VA Risk: HIGH
        /// </summary>
        public bool ValidatePassword(string input)
        {
            // BAD PRACTICE: Plain text password comparison
            return Password == input;
        }
        
        /// <summary>
        /// VULNERABLE: Returns auth token without validation
        /// VA Risk: HIGH
        /// </summary>
        public string GetAuthToken()
        {
            // BAD PRACTICE: No caller validation
            return AuthToken;
        }
        
        /// <summary>
        /// VULNERABLE: Signs data with exposed private key
        /// VA Risk: CRITICAL
        /// </summary>
        public string SignPrescription(string data)
        {
            // BAD PRACTICE: Using PrivateKey directly
            return $"{data}|SIGNED_BY|{PrivateKey}";
        }
    }
}
```

**AVR Calculation:**
| Category | Count |
|----------|-------|
| Total Attributes | 6 |
| Vulnerable Attributes | 3 |
| **AVR Score** | **3/6 = 0.50 (50%)** |

---

### 2.4 PharmacyAdapter (Presentation/Service Layer)

**Purpose:** Handle external communication. **Target for VA and downstream CIVPF analysis.**

```csharp
namespace MediLink.Services
{
    public class PharmacyAdapter
    {
        // ========== ATTRIBUTES ==========
        
        // SAFE ATTRIBUTES (2)
        public int PharmacyID { get; set; }
        public string PharmacyName { get; set; }
        
        // VULNERABLE ATTRIBUTES (2)
        public string APIKey { get; set; }              // Should be in secure config
        public string ConnectionString { get; set; }    // Database credentials exposed
        
        // ========== COUPLING REFERENCES ==========
        private Prescription _prescription;              // VCC +1
        
        // ========== METHODS ==========
        
        /// <summary>
        /// VULNERABLE: Transmits prescription including all sensitive data
        /// CIVPF Risk: HIGH - Inherits vulnerability from Prescription.GetForPharmacy()
        /// VA Risk: CRITICAL - External data transmission
        /// </summary>
        public bool TransmitPrescription(Prescription prescription)
        {
            _prescription = prescription;
            
            // BAD PRACTICE: Getting full DTO with sensitive data
            var dto = prescription.GetForPharmacy();
            
            // BAD PRACTICE: Logging sensitive data before transmission
            Console.WriteLine($"Transmitting: SSN={dto.PatientSSN}, Token={dto.AuthToken}");
            
            // BAD PRACTICE: Including API key in logs
            Console.WriteLine($"Using API Key: {APIKey}");
            
            // Simulated transmission
            return SendToExternalAPI(dto);
        }
        
        /// <summary>
        /// VULNERABLE: Verifies patient using SSN
        /// VA Risk: HIGH - Direct SSN handling
        /// </summary>
        public bool VerifyPatientID(string ssn)
        {
            // BAD PRACTICE: SSN passed as plain string parameter
            // BAD PRACTICE: No input sanitization
            var query = $"SELECT * FROM Patients WHERE SSN = '{ssn}'";  // SQL INJECTION!
            
            Console.WriteLine($"Verifying SSN: {ssn}");  // Logging SSN
            
            return ExecuteQuery(query);
        }
        
        /// <summary>
        /// SAFE: Only logs non-sensitive data
        /// VA Risk: LOW
        /// </summary>
        public void LogTransaction(Guid prescriptionId, DateTime timestamp)
        {
            // Good practice: Only safe data
            Console.WriteLine($"Transaction: {prescriptionId} at {timestamp}");
        }
        
        /// <summary>
        /// VULNERABLE: Exposes connection string
        /// VA Risk: MEDIUM
        /// </summary>
        public string GetDatabaseConnection()
        {
            return ConnectionString;  // Exposed credentials
        }
        
        private bool SendToExternalAPI(PrescriptionDTO dto)
        {
            // Simulated - would send to external pharmacy system
            // BAD PRACTICE: No encryption, no secure channel verification
            return true;
        }
        
        private bool ExecuteQuery(string query)
        {
            // Simulated - BAD PRACTICE: Direct SQL execution
            return true;
        }
    }
}
```

**AVR Calculation:**
| Category | Count |
|----------|-------|
| Total Attributes | 4 |
| Vulnerable Attributes | 2 |
| **AVR Score** | **2/4 = 0.50 (50%)** |

---

### 2.5 AuditLogger (Utility Class)

**Purpose:** Logging utility that improperly handles sensitive data.

```csharp
namespace MediLink.Utilities
{
    public class AuditLogger
    {
        // VULNERABLE ATTRIBUTES (1)
        public string LogFilePath { get; set; }         // Could expose file system
        
        // ========== METHODS ==========
        
        /// <summary>
        /// VULNERABLE: Logs patient data without sanitization
        /// VA Risk: HIGH
        /// </summary>
        public void LogPatientAccess(PatientRecord patient, string action)
        {
            // BAD PRACTICE: Logging all patient data
            var logEntry = $"[{DateTime.Now}] {action}: " +
                          $"ID={patient.PatientID}, " +
                          $"SSN={patient.SSN}, " +           // VULNERABILITY
                          $"DOB={patient.DOB}";              // VULNERABILITY
            
            File.AppendAllText(LogFilePath, logEntry + "\n");
        }
        
        /// <summary>
        /// VULNERABLE: Logs prescription with sensitive data
        /// VA Risk: HIGH
        /// </summary>
        public void LogPrescription(Prescription rx)
        {
            // BAD PRACTICE: Logging auth token and SSN
            var logEntry = $"[{DateTime.Now}] Prescription: " +
                          $"RxID={rx.PrescriptionID}, " +
                          $"SSN={rx.RawPatientSSN}, " +      // VULNERABILITY
                          $"Token={rx.DoctorAuthToken}";     // VULNERABILITY
            
            File.AppendAllText(LogFilePath, logEntry + "\n");
        }
    }
}
```

---

## 3. Class Relationships & CIVPF Analysis

### 3.1 Vulnerability Propagation Diagram

```
┌─────────────────┐          ┌─────────────────┐
│  PatientRecord  │          │     Doctor      │
│  ┌───────────┐  │          │  ┌───────────┐  │
│  │ SSN (V)   │──┼──────┐   │  │AuthToken(V)│──┼──────┐
│  │ DOB (V)   │  │      │   │  │PrivKey(V) │  │      │
│  │ PHI (V)   │  │      │   │  │Password(V)│  │      │
│  └───────────┘  │      │   │  └───────────┘  │      │
└─────────────────┘      │   └─────────────────┘      │
                         │                            │
                         ▼                            ▼
                    ┌─────────────────────────────────────┐
                    │           Prescription              │
                    │  ┌─────────────┐ ┌───────────────┐  │
                    │  │RawPatientSSN│ │DoctorAuthToken│  │
                    │  │    (V)      │ │     (V)       │  │
                    │  └──────┬──────┘ └───────┬───────┘  │
                    └─────────┼────────────────┼──────────┘
                              │                │
                              ▼                ▼
                    ┌─────────────────────────────────────┐
                    │         PharmacyAdapter             │
                    │  TransmitPrescription() receives:   │
                    │  - PatientSSN (propagated)          │
                    │  - AuthToken (propagated)           │
                    └─────────────────────────────────────┘
                              │
                              ▼
                    ┌─────────────────────────────────────┐
                    │       External Pharmacy API         │
                    │     (Data leaves the system)        │
                    └─────────────────────────────────────┘

Legend: (V) = Vulnerable Attribute
```

### 3.2 CIVPF Calculation

| Source Class | Source Attribute | Propagates To | Final Exposure Count |
|--------------|------------------|---------------|---------------------|
| PatientRecord | SSN | Prescription → PharmacyAdapter → External | 3 hops |
| PatientRecord | DOB | AuditLogger (logged) | 1 hop |
| Doctor | AuthToken | Prescription → PharmacyAdapter → External | 3 hops |
| Doctor | PrivateKey | SignPrescription output | 1 hop |

**CIVPF Score Calculation:**
- PatientRecord.SSN CIVPF: **3.0** (propagates through 3 classes)
- Doctor.AuthToken CIVPF: **3.0** (propagates through 3 classes)
- **System CIVPF Average: 3.0** (HIGH RISK)

---

## 4. Vulnerability Amplification (VA) Analysis

### 4.1 Method-Level VA Scores

| Class | Method | Vulnerable Attrs Accessed | Total Attrs Accessed | VA Score |
|-------|--------|--------------------------|---------------------|----------|
| PatientRecord | GetBasicInfo() | 0 | 2 | 0.00 |
| PatientRecord | GenerateReport() | 4 | 6 | **0.67** |
| PatientRecord | GetSSN() | 1 | 1 | **1.00** |
| Prescription | Create() | 2 | 4 | **0.50** |
| Prescription | GetForPharmacy() | 2 | 6 | **0.33** |
| Prescription | DebugPrint() | 2 | 2 | **1.00** |
| Doctor | GetAuthToken() | 1 | 1 | **1.00** |
| Doctor | SignPrescription() | 1 | 2 | **0.50** |
| PharmacyAdapter | TransmitPrescription() | 3 | 5 | **0.60** |
| PharmacyAdapter | VerifyPatientID() | 1 | 1 | **1.00** |
| PharmacyAdapter | LogTransaction() | 0 | 2 | 0.00 |

### 4.2 Critical VA Hotspots

Methods with VA ≥ 0.50 are considered high-risk:

1. **PatientRecord.GetSSN()** - VA: 1.00 ⚠️
2. **Prescription.DebugPrint()** - VA: 1.00 ⚠️
3. **Doctor.GetAuthToken()** - VA: 1.00 ⚠️
4. **PharmacyAdapter.VerifyPatientID()** - VA: 1.00 ⚠️
5. **PatientRecord.GenerateReport()** - VA: 0.67 ⚠️
6. **PharmacyAdapter.TransmitPrescription()** - VA: 0.60 ⚠️

---

## 5. System-Wide Metrics Summary

### 5.1 AVR Summary by Class

| Class | Safe Attrs | Vuln Attrs | Total | AVR |
|-------|------------|------------|-------|-----|
| PatientRecord | 2 | 4 | 6 | **0.67** |
| Prescription | 4 | 2 | 6 | **0.33** |
| Doctor | 3 | 3 | 6 | **0.50** |
| PharmacyAdapter | 2 | 2 | 4 | **0.50** |
| **System Total** | 11 | 11 | 22 | **0.50** |

### 5.2 VCC Summary

| Class | Coupled With | Vulnerable Couplings |
|-------|--------------|---------------------|
| Prescription | PatientRecord, Doctor, PharmacyAdapter | 3 |
| PharmacyAdapter | Prescription | 1 |
| AuditLogger | PatientRecord, Prescription | 2 |
| **Total VCC** | | **6** |

### 5.3 Overall Security Metrics

| Metric | Value | Status |
|--------|-------|--------|
| System AVR | 0.50 (50%) | 🔴 POOR |
| System VCC | 6 vulnerable couplings | 🔴 POOR |
| Max CIVPF Path Length | 3 hops | 🔴 POOR |
| Average Method VA | 0.55 | 🔴 POOR |
| Critical VA Methods (≥0.50) | 6 | 🔴 POOR |

---

## 6. Intentional Security Flaws Summary

### 6.1 Data Protection Failures

| Flaw | Location | Impact |
|------|----------|--------|
| Plain text SSN storage | PatientRecord.SSN | PII Exposure |
| Plain text password | Doctor.Password | Credential theft |
| Exposed private key | Doctor.PrivateKey | Signature forgery |
| Credit card in plain text | PatientRecord.CreditCardToken | PCI violation |
| SSN duplication | Prescription.RawPatientSSN | Data sprawl |

### 6.2 Code Quality Failures

| Flaw | Location | Impact |
|------|----------|--------|
| SQL Injection | PharmacyAdapter.VerifyPatientID() | Data breach |
| Sensitive data logging | Multiple methods | Log exposure |
| No input validation | All methods | Injection attacks |
| No encryption | All data transmission | Data interception |
| Hardcoded credentials | PharmacyAdapter.ConnectionString | Config exposure |

### 6.3 Architecture Failures

| Flaw | Description | Metric Impact |
|------|-------------|---------------|
| Data coupling | Sensitive data copied between layers | High CIVPF |
| God methods | Methods access too many attributes | High VA |
| No separation | Business logic mixed with data | High VCC |
| No encapsulation | All properties public | High AVR |

---

## 7. File Structure

```
MediLink/
├── MediLink.sln
├── MediLink/
│   ├── MediLink.csproj
│   ├── Program.cs
│   ├── Data/
│   │   ├── PatientRecord.cs
│   │   └── Doctor.cs
│   ├── Business/
│   │   ├── Prescription.cs
│   │   └── PrescriptionDTO.cs
│   ├── Services/
│   │   └── PharmacyAdapter.cs
│   └── Utilities/
│       └── AuditLogger.cs
└── README.md
```

---

## 8. Implementation Notes

> [!IMPORTANT]
> This specification is designed for **educational purposes** to demonstrate poor security practices and their impact on code quality metrics.

### For Metric Analysis:
1. **AVR**: Count vulnerable vs total attributes per class
2. **CIVPF**: Trace how sensitive data flows through class couplings
3. **VCC**: Count how many other classes each vulnerable class touches
4. **VA**: For each method, calculate (vulnerable attrs accessed / total attrs accessed)

### Expected Poor Metric Results:
- **AVR > 0.30** indicates poor data classification
- **VCC > 3** indicates excessive coupling
- **CIVPF > 2** indicates dangerous data propagation
- **VA > 0.40** indicates methods expose too much sensitive data

---

*Document Version: 1.0 | Created for Security Metrics Analysis*
