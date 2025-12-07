# 🔒 MediLink Security Metrics Analysis

**Document Version:** 1.0  
**Analysis Date:** December 7, 2025  
**Project:** MediLink Telemedicine & Prescription Management System

> ⚠️ **Educational Purpose Only**
> 
> This analysis examines intentionally vulnerable code designed to demonstrate poor security practices and their measurable impact on software quality metrics.

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Metric Definitions](#metric-definitions)
3. [AVR Analysis (Attribute Vulnerability Ratio)](#avr-analysis)
4. [CIVPF Analysis (Classified Information Vulnerability Propagation Factor)](#civpf-analysis)
5. [VCC Analysis (Vulnerable Class Coupling)](#vcc-analysis)
6. [VA Analysis (Vulnerability Amplification)](#va-analysis)
7. [Critical Findings](#critical-findings)
8. [Recommendations](#recommendations)

---

## Executive Summary

### Overall Security Posture: 🔴 **CRITICAL**

The MediLink system demonstrates severe security vulnerabilities across all measured dimensions:

| Metric | Score | Threshold | Status | Severity |
|--------|-------|-----------|--------|----------|
| **System AVR** | 0.50 (50%) | ≤ 0.30 | 🔴 FAIL | CRITICAL |
| **System VCC** | 6 couplings | ≤ 3 | 🔴 FAIL | HIGH |
| **Max CIVPF** | 3 hops | ≤ 2 | 🔴 FAIL | CRITICAL |
| **Avg Method VA** | 0.55 (55%) | ≤ 0.40 | 🔴 FAIL | HIGH |

**Key Findings:**
- 50% of all class attributes contain sensitive data without proper protection
- Sensitive data propagates through 3 layers before leaving the system
- 6 critical methods expose 50%+ of vulnerable attributes
- SQL Injection vulnerability present in external communication layer

---

## Metric Definitions

### AVR (Attribute Vulnerability Ratio)

**Formula:** `AVR = Vulnerable Attributes / Total Attributes`

**Definition:** Measures the proportion of class attributes that contain sensitive or vulnerable data without adequate protection.

**Vulnerable Attributes Include:**
- PII (Personally Identifiable Information): SSN, DOB
- PHI (Protected Health Information): Medical records
- PCI (Payment Card Industry data): Credit card information
- Credentials: Passwords, tokens, private keys
- Configuration secrets: API keys, connection strings

**Thresholds:**
- ✅ **Good:** AVR ≤ 0.30 (30%)
- ⚠️ **Moderate:** 0.30 < AVR ≤ 0.50
- 🔴 **Poor:** AVR > 0.50

---

### CIVPF (Classified Information Vulnerability Propagation Factor)

**Formula:** `CIVPF = Number of class hops sensitive data traverses`

**Definition:** Measures how far sensitive data propagates through the system via class coupling relationships before being consumed or externalized.

**Propagation Chain:**
1. **Source Class:** Where sensitive data originates (e.g., `PatientRecord.SSN`)
2. **Intermediate Classes:** Classes that copy/store/pass the data
3. **Terminal Point:** Where data is consumed (logged, transmitted, stored)

**Thresholds:**
- ✅ **Good:** CIVPF ≤ 1 (data stays in source class)
- ⚠️ **Moderate:** CIVPF = 2 (one intermediate hop)
- 🔴 **Poor:** CIVPF ≥ 3 (multiple hops, high exposure)

---

### VCC (Vulnerable Class Coupling)

**Formula:** `VCC = Count of classes coupled with vulnerable data exchange`

**Definition:** Counts how many other classes a vulnerable class is coupled with through sensitive data sharing.

**Coupling Types:**
- **Composition:** Class contains instance of another class
- **Parameter Passing:** Method receives/returns vulnerable class
- **Data Extraction:** Method accesses vulnerable attributes from coupled class

**Thresholds:**
- ✅ **Good:** VCC ≤ 2
- ⚠️ **Moderate:** VCC = 3-4
- 🔴 **Poor:** VCC ≥ 5

---

### VA (Vulnerability Amplification)

**Formula:** `VA = Vulnerable Attributes Accessed / Total Attributes Accessed`

**Definition:** Method-level metric measuring what proportion of accessed attributes are vulnerable.

**Risk Levels:**
- **VA = 1.00:** Method accesses ONLY vulnerable attributes (maximum risk)
- **VA = 0.50:** Half of accessed attributes are vulnerable
- **VA = 0.00:** Method accesses only safe attributes

**Thresholds:**
- ✅ **Good:** VA ≤ 0.40
- ⚠️ **Moderate:** 0.40 < VA ≤ 0.60
- 🔴 **Poor:** VA > 0.60

---

## AVR Analysis

### Class-Level AVR Breakdown

#### 1. PatientRecord (Data Layer)

**AVR Score: 0.67 (67%)** 🔴 **CRITICAL**

| Attribute | Type | Sensitive? | Vulnerability |
|-----------|------|------------|---------------|
| `PatientID` | int | ❌ No | Safe - Non-sensitive identifier |
| `FullName` | string | ❌ No | Safe - Public information |
| `DOB` | DateTime | ✅ **Yes** | **PII - No encryption** |
| `SSN` | string | ✅ **Yes** | **PII - Plain text storage** |
| `MedicalHistory` | string | ✅ **Yes** | **PHI - No access control** |
| `CreditCardToken` | string | ✅ **Yes** | **PCI - Improperly secured** |

**Calculation:**
```
Vulnerable Attributes: 4 (DOB, SSN, MedicalHistory, CreditCardToken)
Total Attributes: 6
AVR = 4/6 = 0.667 (67%)
```

**Critical Issues:**
- SSN stored in plain text violates PII protection standards
- Medical history accessible without role-based access control
- Credit card data stored as "token" but not properly tokenized
- No encryption at rest for any sensitive field

---

#### 2. Doctor (Data Layer)

**AVR Score: 0.50 (50%)** 🔴 **POOR**

| Attribute | Type | Sensitive? | Vulnerability |
|-----------|------|------------|---------------|
| `DoctorID` | int | ❌ No | Safe - Non-sensitive identifier |
| `FullName` | string | ❌ No | Safe - Public information |
| `Specialty` | string | ❌ No | Safe - Public information |
| `AuthToken` | string | ✅ **Yes** | **Credential - Plain text** |
| `PrivateKey` | string | ✅ **Yes** | **Cryptographic key exposed** |
| `Password` | string | ✅ **Yes** | **Credential - Not hashed** |

**Calculation:**
```
Vulnerable Attributes: 3 (AuthToken, PrivateKey, Password)
Total Attributes: 6
AVR = 3/6 = 0.50 (50%)
```

**Critical Issues:**
- Password stored in plain text (should use bcrypt/Argon2)
- Private signing key exposed as public property
- Authentication token not secured or rotated
- No credential vault integration

---

#### 3. Prescription (Business Layer)

**AVR Score: 0.33 (33%)** ⚠️ **MODERATE**

| Attribute | Type | Sensitive? | Vulnerability |
|-----------|------|------------|---------------|
| `PrescriptionID` | Guid | ❌ No | Safe - Non-sensitive identifier |
| `PatientID` | int | ❌ No | Safe - Foreign key only |
| `DrugName` | string | ❌ No | Safe - Public drug name |
| `Dosage` | string | ❌ No | Safe - Dosage information |
| `DoctorAuthToken` | string | ✅ **Yes** | **Credential duplication** |
| `RawPatientSSN` | string | ✅ **Yes** | **PII duplication** |

**Calculation:**
```
Vulnerable Attributes: 2 (DoctorAuthToken, RawPatientSSN)
Total Attributes: 6
AVR = 2/6 = 0.333 (33%)
```

**Critical Issues:**
- SSN copied from `PatientRecord` for "convenience"
- AuthToken duplicated from `Doctor` class
- Data duplication increases attack surface
- Violates principle of single source of truth

---

#### 4. PharmacyAdapter (Service Layer)

**AVR Score: 0.50 (50%)** 🔴 **POOR**

| Attribute | Type | Sensitive? | Vulnerability |
|-----------|------|------------|---------------|
| `PharmacyID` | int | ❌ No | Safe - Non-sensitive identifier |
| `PharmacyName` | string | ❌ No | Safe - Public information |
| `APIKey` | string | ✅ **Yes** | **Configuration secret exposed** |
| `ConnectionString` | string | ✅ **Yes** | **Database credentials exposed** |

**Calculation:**
```
Vulnerable Attributes: 2 (APIKey, ConnectionString)
Total Attributes: 4
AVR = 2/4 = 0.50 (50%)
```

**Critical Issues:**
- API key stored as class property (should use secure vault)
- Connection string contains database credentials
- No configuration encryption
- Secrets accessible to any code with class reference

---

#### 5. AuditLogger (Utility Layer)

**AVR Score: 0.50 (50%)** ⚠️ **MODERATE**

| Attribute | Type | Sensitive? | Vulnerability |
|-----------|------|------------|---------------|
| `LogFilePath` | string | ✅ **Yes** | **File system exposure** |
| `VerboseMode` | bool | ❌ No | Safe - Configuration flag |

**Calculation:**
```
Vulnerable Attributes: 1 (LogFilePath)
Total Attributes: 2
AVR = 1/2 = 0.50 (50%)
```

**Note:** While AVR is moderate, this class has HIGH VA due to logging methods that expose sensitive data.

---

### System-Wide AVR Summary

| Class | Safe | Vulnerable | Total | AVR | Status |
|-------|------|------------|-------|-----|--------|
| PatientRecord | 2 | 4 | 6 | 0.67 | 🔴 |
| Doctor | 3 | 3 | 6 | 0.50 | 🔴 |
| Prescription | 4 | 2 | 6 | 0.33 | ⚠️ |
| PharmacyAdapter | 2 | 2 | 4 | 0.50 | 🔴 |
| AuditLogger | 1 | 1 | 2 | 0.50 | ⚠️ |
| **SYSTEM TOTAL** | **12** | **12** | **24** | **0.50** | **🔴** |

**System AVR: 0.50 (50%)**

**Interpretation:** Half of all attributes in the system contain sensitive data without adequate protection. This significantly exceeds the 0.30 threshold for acceptable security posture.

---

## CIVPF Analysis

### Propagation Chain Mapping

CIVPF tracks how sensitive data flows through class relationships from origin to consumption.

---

### Chain 1: Patient SSN Propagation

**CIVPF Score: 3 hops** 🔴 **CRITICAL**

```
┌─────────────────────────────────────────────────────────────┐
│ Hop 1: PatientRecord.SSN (Origin)                           │
│ - Attribute: SSN = "123-45-6789"                            │
│ - Storage: Plain text string                                │
│ - Protection: None                                           │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ Prescription.Create(patient, doctor)
                     │ Copies: RawPatientSSN = patient.SSN
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ Hop 2: Prescription.RawPatientSSN (Intermediate)            │
│ - Attribute: RawPatientSSN = "123-45-6789"                  │
│ - Duplication: Data copied for "convenience"                │
│ - Protection: None                                           │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ Prescription.GetForPharmacy()
                     │ Returns: PrescriptionDTO with PatientSSN
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ Hop 3: PharmacyAdapter.TransmitPrescription() (Terminal)    │
│ - Method: TransmitPrescription(prescription)                │
│ - Action: Sends SSN to external pharmacy API                │
│ - Protection: No encryption, logged to console              │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ SendToExternalAPI(dto)
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ EXTERNAL SYSTEM: Pharmacy API                               │
│ - Data leaves system boundary                                │
│ - SSN transmitted over network                               │
│ - No control over external storage/handling                  │
└─────────────────────────────────────────────────────────────┘
```

**Code Flow:**

```csharp
// Hop 1: Origin
var patient = new PatientRecord { SSN = "123-45-6789" };

// Hop 2: Intermediate duplication
prescription.Create(patient, doctor);
// Inside Create(): RawPatientSSN = patient.SSN;

// Hop 3: External transmission
pharmacy.TransmitPrescription(prescription);
// Inside TransmitPrescription():
//   var dto = prescription.GetForPharmacy();
//   Console.WriteLine($"SSN={dto.PatientSSN}"); // Logged!
//   SendToExternalAPI(dto); // Transmitted!
```

**Vulnerability Impact:**
- SSN exposed at 3 different points in the system
- Each hop increases breach surface area
- Data logged to console at final hop
- Transmitted to external system without encryption

---

### Chain 2: Doctor AuthToken Propagation

**CIVPF Score: 3 hops** 🔴 **CRITICAL**

```
┌─────────────────────────────────────────────────────────────┐
│ Hop 1: Doctor.AuthToken (Origin)                            │
│ - Attribute: AuthToken = "eyJhbGci..."                      │
│ - Storage: Plain text string                                │
│ - Protection: None                                           │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ Prescription.Create(patient, doctor)
                     │ Copies: DoctorAuthToken = doctor.AuthToken
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ Hop 2: Prescription.DoctorAuthToken (Intermediate)          │
│ - Attribute: DoctorAuthToken = "eyJhbGci..."                │
│ - Duplication: Token copied to prescription                 │
│ - Protection: None                                           │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ Prescription.GetForPharmacy()
                     │ Returns: PrescriptionDTO with AuthToken
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ Hop 3: PharmacyAdapter.TransmitPrescription() (Terminal)    │
│ - Method: TransmitPrescription(prescription)                │
│ - Action: Sends AuthToken to external pharmacy API          │
│ - Protection: No encryption, logged to console              │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ SendToExternalAPI(dto)
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ EXTERNAL SYSTEM: Pharmacy API                               │
│ - Internal authentication token exposed externally          │
│ - Could be used to impersonate doctor                        │
└─────────────────────────────────────────────────────────────┘
```

**Vulnerability Impact:**
- Internal authentication token exposed to external system
- Token could be intercepted and used for impersonation
- No token scoping or expiration
- Logged in plain text at transmission point

---

### Chain 3: Patient DOB to Audit Log

**CIVPF Score: 1 hop** ⚠️ **MODERATE**

```
┌─────────────────────────────────────────────────────────────┐
│ Hop 1: PatientRecord.DOB (Origin)                           │
│ - Attribute: DOB = 1985-03-15                               │
│ - Storage: DateTime                                          │
│ - Protection: None                                           │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ AuditLogger.LogPatientAccess(patient, action)
                     │ Logs: DOB={patient.DOB}
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ Terminal: AuditLogger.WriteLog() (File + Console)           │
│ - Action: Writes DOB to log file and console                │
│ - File: medilink_audit.log                                  │
│ - Protection: None, plain text file                         │
└─────────────────────────────────────────────────────────────┘
```

**Vulnerability Impact:**
- PII written to unencrypted log files
- Log files may be accessible to unauthorized users
- No log rotation or secure deletion
- Console output may be captured by monitoring tools

---

### Chain 4: Doctor PrivateKey in Signature

**CIVPF Score: 1 hop** ⚠️ **MODERATE**

```
┌─────────────────────────────────────────────────────────────┐
│ Hop 1: Doctor.PrivateKey (Origin)                           │
│ - Attribute: PrivateKey = "-----BEGIN RSA..."              │
│ - Storage: Plain text string                                │
│ - Protection: None                                           │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ Doctor.SignPrescription(data)
                     │ Returns: "{data}|SIGNED_BY|{PrivateKey}"
                     ▼
┌─────────────────────────────────────────────────────────────┐
│ Terminal: Signature String (Return Value)                   │
│ - Action: Private key embedded in signature                 │
│ - Exposure: Anyone receiving signature sees private key     │
│ - Impact: Complete cryptographic compromise                 │
└─────────────────────────────────────────────────────────────┘
```

**Vulnerability Impact:**
- Private key exposed in signature output
- Violates fundamental cryptographic principles
- Anyone with signature can forge future signatures
- Complete compromise of signing infrastructure

---

### CIVPF Summary Table

| Source Class | Attribute | Hops | Path | Terminal Point | Status |
|--------------|-----------|------|------|----------------|--------|
| PatientRecord | SSN | 3 | PatientRecord → Prescription → PharmacyAdapter → External | External API | 🔴 |
| Doctor | AuthToken | 3 | Doctor → Prescription → PharmacyAdapter → External | External API | 🔴 |
| PatientRecord | DOB | 1 | PatientRecord → AuditLogger | Log File | ⚠️ |
| Doctor | PrivateKey | 1 | Doctor → SignPrescription | Signature Output | ⚠️ |
| PatientRecord | MedicalHistory | 0 | PatientRecord only | In-class usage | ✅ |
| PatientRecord | CreditCardToken | 0 | PatientRecord only | In-class usage | ✅ |

**Maximum CIVPF: 3 hops** (SSN and AuthToken chains)

**System CIVPF Score: 3** 🔴 **CRITICAL**

**Interpretation:** Sensitive data propagates through 3 class boundaries before leaving the system, creating multiple exposure points and significantly increasing breach risk.

---

## VCC Analysis

### Vulnerable Class Coupling Matrix

VCC measures how many classes each vulnerable class is coupled with through sensitive data exchange.

---

### Class 1: Prescription (Business Layer)

**VCC Score: 3** 🔴 **POOR**

**Coupled Classes:**

| Coupled Class | Coupling Type | Vulnerable Data Exchanged | Code Location |
|---------------|---------------|---------------------------|---------------|
| **PatientRecord** | Parameter + Composition | SSN | `Prescription.Create(PatientRecord patient, ...)` |
| **Doctor** | Parameter + Composition | AuthToken | `Prescription.Create(..., Doctor doctor)` |
| **PharmacyAdapter** | Composition + Method Call | SSN + AuthToken | `PharmacyAdapter.TransmitPrescription(Prescription)` |

**Coupling Details:**

```csharp
public class Prescription
{
    // VCC Coupling 1: PatientRecord
    private PatientRecord? _patient;
    
    // VCC Coupling 2: Doctor
    private Doctor? _doctor;
    
    // VCC Coupling 3: PharmacyAdapter
    private PharmacyAdapter? _pharmacy;
    
    // Method creating couplings 1 & 2
    public void Create(PatientRecord patient, Doctor doctor)
    {
        _patient = patient;
        _doctor = doctor;
        
        // Vulnerable data extraction
        RawPatientSSN = patient.SSN;           // From PatientRecord
        DoctorAuthToken = doctor.AuthToken;    // From Doctor
    }
    
    // Method used by coupling 3
    public PrescriptionDTO GetForPharmacy()
    {
        return new PrescriptionDTO
        {
            PatientSSN = RawPatientSSN,        // To PharmacyAdapter
            AuthToken = DoctorAuthToken        // To PharmacyAdapter
        };
    }
}
```

**Impact:**
- Central hub for sensitive data aggregation
- Couples data layer (PatientRecord, Doctor) with service layer (PharmacyAdapter)
- Creates vulnerability propagation pathway
- Single point of failure for data protection

---

### Class 2: PharmacyAdapter (Service Layer)

**VCC Score: 1** ✅ **ACCEPTABLE**

**Coupled Classes:**

| Coupled Class | Coupling Type | Vulnerable Data Exchanged | Code Location |
|---------------|---------------|---------------------------|---------------|
| **Prescription** | Parameter | SSN + AuthToken | `TransmitPrescription(Prescription prescription)` |

**Coupling Details:**

```csharp
public class PharmacyAdapter
{
    // VCC Coupling 1: Prescription
    private Prescription? _prescription;
    
    public bool TransmitPrescription(Prescription prescription)
    {
        _prescription = prescription;
        
        // Vulnerable data extraction
        var dto = prescription.GetForPharmacy();
        // dto contains: PatientSSN, AuthToken
        
        return SendToExternalAPI(dto);
    }
}
```

**Impact:**
- Receives aggregated sensitive data from Prescription
- Final hop before data leaves system
- External transmission point (highest risk)

---

### Class 3: AuditLogger (Utility Layer)

**VCC Score: 2** ✅ **ACCEPTABLE**

**Coupled Classes:**

| Coupled Class | Coupling Type | Vulnerable Data Exchanged | Code Location |
|---------------|---------------|---------------------------|---------------|
| **PatientRecord** | Parameter | SSN, DOB | `LogPatientAccess(PatientRecord patient, ...)` |
| **Prescription** | Parameter | SSN, AuthToken | `LogPrescription(Prescription rx)` |

**Coupling Details:**

```csharp
public class AuditLogger
{
    // VCC Coupling 1: PatientRecord
    public void LogPatientAccess(PatientRecord patient, string action)
    {
        // Accesses: patient.SSN, patient.DOB
        var logEntry = $"SSN={patient.SSN}, DOB={patient.DOB}";
        WriteLog(logEntry);
    }
    
    // VCC Coupling 2: Prescription
    public void LogPrescription(Prescription rx)
    {
        // Accesses: rx.RawPatientSSN, rx.DoctorAuthToken
        var logEntry = $"SSN={rx.RawPatientSSN}, Token={rx.DoctorAuthToken}";
        WriteLog(logEntry);
    }
}
```

**Impact:**
- Logs sensitive data from multiple sources
- Creates persistent record of vulnerable data
- Log files become high-value targets

---

### Class 4: PatientRecord (Data Layer)

**VCC Score: 0** ✅ **GOOD**

**Coupled Classes:** None (data is extracted by other classes, but PatientRecord doesn't initiate coupling)

**Note:** While PatientRecord has high AVR (0.67), it has low VCC because it doesn't actively couple with other classes. Other classes couple with it to extract data.

---

### Class 5: Doctor (Data Layer)

**VCC Score: 0** ✅ **GOOD**

**Coupled Classes:** None (similar to PatientRecord)

---

### VCC Summary Table

| Class | Coupled With | VCC Score | Status | Risk Level |
|-------|--------------|-----------|--------|------------|
| **Prescription** | PatientRecord, Doctor, PharmacyAdapter | 3 | 🔴 | CRITICAL |
| **PharmacyAdapter** | Prescription | 1 | ✅ | MODERATE |
| **AuditLogger** | PatientRecord, Prescription | 2 | ✅ | MODERATE |
| PatientRecord | None | 0 | ✅ | LOW |
| Doctor | None | 0 | ✅ | LOW |

**System VCC Total: 6 vulnerable couplings** 🔴 **POOR**

**Calculation:**
```
Total VCC = 3 (Prescription) + 1 (PharmacyAdapter) + 2 (AuditLogger)
          = 6 vulnerable class couplings
```

**Interpretation:** The system has 6 coupling relationships that exchange vulnerable data. The `Prescription` class acts as a central hub with VCC of 3, creating a single point of failure for data protection.

---

### VCC Visualization

```
                    ┌─────────────────┐
                    │  PatientRecord  │
                    │   (AVR: 0.67)   │
                    │   (VCC: 0)      │
                    └────────┬────────┘
                             │
                             │ Extracts: SSN
                             │
                             ▼
    ┌─────────────┐    ┌─────────────────┐    ┌──────────────────┐
    │   Doctor    │───▶│  Prescription   │───▶│ PharmacyAdapter  │
    │(AVR: 0.50)  │    │  (AVR: 0.33)    │    │  (AVR: 0.50)     │
    │(VCC: 0)     │    │  (VCC: 3) 🔴    │    │  (VCC: 1)        │
    └─────────────┘    └────────┬────────┘    └──────────────────┘
         │                      │                       │
         │ Extracts:            │                       │
         │ AuthToken            │                       │
         │                      │                       │
         │                      ▼                       │
         │             ┌─────────────────┐              │
         │             │  AuditLogger    │              │
         └────────────▶│  (AVR: 0.50)    │◀─────────────┘
                       │  (VCC: 2)       │
                       └─────────────────┘

Legend:
───▶ : Vulnerable data coupling
🔴   : Critical VCC score (≥3)
```

---

## VA Analysis

### Method-Level Vulnerability Amplification

VA measures what proportion of attributes accessed by a method are vulnerable.

---

### Critical VA Methods (VA ≥ 0.50)

#### 1. PatientRecord.GetSSN()

**VA Score: 1.00** 🔴 **CRITICAL**

```csharp
public string GetSSN()
{
    return SSN;  // Accesses 1 vulnerable attribute
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 1 (SSN)
Total Attributes Accessed: 1
VA = 1/1 = 1.00 (100%)
```

**Risk:** Maximum - method exists solely to expose SSN

---

#### 2. Prescription.DebugPrint()

**VA Score: 1.00** 🔴 **CRITICAL**

```csharp
public void DebugPrint()
{
    Console.WriteLine($"[DEBUG] SSN: {RawPatientSSN}, Token: {DoctorAuthToken}");
    // Accesses: RawPatientSSN (V), DoctorAuthToken (V)
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 2 (RawPatientSSN, DoctorAuthToken)
Total Attributes Accessed: 2
VA = 2/2 = 1.00 (100%)
```

**Risk:** Maximum - debug method logs only sensitive data

---

#### 3. Doctor.GetAuthToken()

**VA Score: 1.00** 🔴 **CRITICAL**

```csharp
public string GetAuthToken()
{
    return AuthToken;  // Accesses 1 vulnerable attribute
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 1 (AuthToken)
Total Attributes Accessed: 1
VA = 1/1 = 1.00 (100%)
```

**Risk:** Maximum - exposes authentication token without validation

---

#### 4. PharmacyAdapter.VerifyPatientID()

**VA Score: 1.00** 🔴 **CRITICAL**

```csharp
public bool VerifyPatientID(string ssn)
{
    var query = $"SELECT * FROM Patients WHERE SSN = '{ssn}'";  // SQL Injection!
    Console.WriteLine($"Verifying SSN: {ssn}");
    return ExecuteQuery(query);
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 1 (ssn parameter - SSN)
Total Attributes Accessed: 1
VA = 1/1 = 1.00 (100%)
```

**Risk:** Maximum - SQL injection + SSN logging

---

#### 5. PatientRecord.GenerateReport()

**VA Score: 0.67** 🔴 **CRITICAL**

```csharp
public string GenerateReport()
{
    return $"Patient: {FullName}\n" +           // Safe
           $"SSN: {SSN}\n" +                    // Vulnerable
           $"DOB: {DOB}\n" +                    // Vulnerable
           $"History: {MedicalHistory}\n" +    // Vulnerable
           $"Payment: {CreditCardToken}";      // Vulnerable
    // Accesses: FullName (S), SSN (V), DOB (V), MedicalHistory (V), CreditCardToken (V)
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 4 (SSN, DOB, MedicalHistory, CreditCardToken)
Total Attributes Accessed: 6 (includes PatientID, FullName)
VA = 4/6 = 0.67 (67%)
```

**Risk:** High - exposes all sensitive patient data in single string

---

#### 6. PharmacyAdapter.TransmitPrescription()

**VA Score: 0.60** 🔴 **HIGH**

```csharp
public bool TransmitPrescription(Prescription prescription)
{
    _prescription = prescription;
    var dto = prescription.GetForPharmacy();
    
    // Logs: dto.PatientSSN (V), dto.AuthToken (V), APIKey (V)
    Console.WriteLine($"Transmitting: SSN={dto.PatientSSN}, Token={dto.AuthToken}");
    Console.WriteLine($"Using API Key: {APIKey}");
    
    return SendToExternalAPI(dto);
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 3 (PatientSSN, AuthToken, APIKey)
Total Attributes Accessed: 5 (includes PrescriptionID, PatientID)
VA = 3/5 = 0.60 (60%)
```

**Risk:** High - transmits sensitive data to external system

---

#### 7. Doctor.SignPrescription()

**VA Score: 0.50** ⚠️ **MODERATE**

```csharp
public string SignPrescription(string data)
{
    return $"{data}|SIGNED_BY|{PrivateKey}";
    // Accesses: data (parameter, safe), PrivateKey (V)
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 1 (PrivateKey)
Total Attributes Accessed: 2 (data parameter + PrivateKey)
VA = 1/2 = 0.50 (50%)
```

**Risk:** Moderate - exposes private key in signature

---

#### 8. Prescription.Create()

**VA Score: 0.50** ⚠️ **MODERATE**

```csharp
public void Create(PatientRecord patient, Doctor doctor)
{
    _patient = patient;
    _doctor = doctor;
    
    RawPatientSSN = patient.SSN;           // Vulnerable
    DoctorAuthToken = doctor.AuthToken;    // Vulnerable
    
    PrescriptionID = Guid.NewGuid();       // Safe
    PatientID = patient.PatientID;         // Safe
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 2 (patient.SSN, doctor.AuthToken)
Total Attributes Accessed: 4 (includes PatientID, PrescriptionID)
VA = 2/4 = 0.50 (50%)
```

**Risk:** Moderate - creates CIVPF propagation chain

---

### Safe VA Methods (VA < 0.40)

#### 9. Prescription.GetForPharmacy()

**VA Score: 0.33** ⚠️ **LOW-MODERATE**

```csharp
public PrescriptionDTO GetForPharmacy()
{
    return new PrescriptionDTO
    {
        PrescriptionID = PrescriptionID,    // Safe
        PatientID = PatientID,              // Safe
        DrugName = DrugName,                // Safe
        Dosage = Dosage,                    // Safe
        PatientSSN = RawPatientSSN,         // Vulnerable
        AuthToken = DoctorAuthToken         // Vulnerable
    };
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 2 (RawPatientSSN, DoctorAuthToken)
Total Attributes Accessed: 6
VA = 2/6 = 0.33 (33%)
```

**Note:** While VA is below threshold, this method is still critical as it enables CIVPF propagation.

---

#### 10. PatientRecord.GetBasicInfo()

**VA Score: 0.00** ✅ **SAFE**

```csharp
public (int id, string name) GetBasicInfo()
{
    return (PatientID, FullName);
    // Accesses: PatientID (S), FullName (S)
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 0
Total Attributes Accessed: 2
VA = 0/2 = 0.00 (0%)
```

**Risk:** None - only accesses safe attributes

---

#### 11. PharmacyAdapter.LogTransaction()

**VA Score: 0.00** ✅ **SAFE**

```csharp
public void LogTransaction(Guid prescriptionId, DateTime timestamp)
{
    Console.WriteLine($"Transaction: {prescriptionId} at {timestamp}");
    // Accesses: prescriptionId (S), timestamp (S)
}
```

**Calculation:**
```
Vulnerable Attributes Accessed: 0
Total Attributes Accessed: 2
VA = 0/2 = 0.00 (0%)
```

**Risk:** None - safe logging practice

---

### VA Summary Table

| Class | Method | Vuln Accessed | Total Accessed | VA Score | Status |
|-------|--------|---------------|----------------|----------|--------|
| PatientRecord | GetSSN() | 1 | 1 | **1.00** | 🔴 |
| Prescription | DebugPrint() | 2 | 2 | **1.00** | 🔴 |
| Doctor | GetAuthToken() | 1 | 1 | **1.00** | 🔴 |
| PharmacyAdapter | VerifyPatientID() | 1 | 1 | **1.00** | 🔴 |
| PatientRecord | GenerateReport() | 4 | 6 | **0.67** | 🔴 |
| PharmacyAdapter | TransmitPrescription() | 3 | 5 | **0.60** | 🔴 |
| Doctor | SignPrescription() | 1 | 2 | **0.50** | ⚠️ |
| Prescription | Create() | 2 | 4 | **0.50** | ⚠️ |
| Prescription | GetForPharmacy() | 2 | 6 | **0.33** | ⚠️ |
| PatientRecord | GetBasicInfo() | 0 | 2 | **0.00** | ✅ |
| PharmacyAdapter | LogTransaction() | 0 | 2 | **0.00** | ✅ |

**Critical VA Methods (VA ≥ 0.50): 8 methods**

**Average System VA: 0.55 (55%)** 🔴 **POOR**

---

### VA Distribution Analysis

```
VA Score Distribution:

1.00 (100%) ████████████████████████ 4 methods (36%)
0.67 (67%)  ██████                   1 method  (9%)
0.60 (60%)  █████                    1 method  (9%)
0.50 (50%)  ████                     2 methods (18%)
0.33 (33%)  ██                       1 method  (9%)
0.00 (0%)   ██                       2 methods (18%)

Critical (VA ≥ 0.50): 8 methods (73%)
Safe (VA < 0.40):     3 methods (27%)
```

**Interpretation:** 73% of analyzed methods have VA ≥ 0.50, indicating that most methods in the system expose significant amounts of sensitive data.

---

## Critical Findings

### 🔴 Critical Security Issues

#### 1. Plain Text Credential Storage

**Affected Classes:** `Doctor`, `PharmacyAdapter`

**Details:**
- Passwords stored without hashing
- Private keys exposed as public properties
- API keys stored in class attributes
- Database connection strings with embedded credentials

**Impact:** Complete credential compromise if any part of the system is breached

**CVSS Score:** 9.8 (Critical)

---

#### 2. SQL Injection Vulnerability

**Location:** `PharmacyAdapter.VerifyPatientID()`

**Code:**
```csharp
var query = $"SELECT * FROM Patients WHERE SSN = '{ssn}'";
```

**Attack Vector:**
```csharp
// Attacker input:
pharmacy.VerifyPatientID("' OR '1'='1' --");

// Resulting query:
// SELECT * FROM Patients WHERE SSN = '' OR '1'='1' --'
// Returns all patients!
```

**Impact:** Complete database compromise, data exfiltration

**CVSS Score:** 9.1 (Critical)

---

#### 3. Sensitive Data Logging

**Affected Classes:** `AuditLogger`, `PharmacyAdapter`, `Prescription`

**Logged Data:**
- Social Security Numbers
- Passwords (in `LogAuthAttempt`)
- Authentication tokens
- Private keys (in verbose mode)

**Impact:** Log files become high-value targets, persistent exposure

**CVSS Score:** 8.2 (High)

---

#### 4. Data Duplication (CIVPF Amplification)

**Location:** `Prescription.Create()`

**Details:**
```csharp
RawPatientSSN = patient.SSN;           // Duplication 1
DoctorAuthToken = doctor.AuthToken;    // Duplication 2
```

**Impact:**
- Increases attack surface
- Creates multiple breach points
- Violates single source of truth
- Amplifies CIVPF propagation

**CVSS Score:** 7.5 (High)

---

#### 5. External Data Transmission Without Encryption

**Location:** `PharmacyAdapter.TransmitPrescription()`

**Details:**
- SSN transmitted to external API
- AuthToken transmitted to external API
- No TLS verification
- No encryption at application layer

**Impact:** Man-in-the-middle attacks, data interception

**CVSS Score:** 8.8 (High)

---

### ⚠️ High-Risk Architectural Issues

#### 1. No Separation of Concerns

**Issue:** Business logic (`Prescription`) directly couples with data layer (`PatientRecord`, `Doctor`) and service layer (`PharmacyAdapter`)

**Impact:** High VCC (3), difficult to secure individual layers

---

#### 2. No Encapsulation

**Issue:** All sensitive attributes are public properties

**Impact:** High AVR (0.50), no access control

---

#### 3. No Input Validation

**Issue:** No validation on SSN format, password complexity, or any user input

**Impact:** Injection attacks, data corruption

---

#### 4. No Audit Trail

**Issue:** Sensitive data access not tracked (except vulnerable logging)

**Impact:** Cannot detect or investigate breaches

---

## Recommendations

### Immediate Actions (Priority 1)

#### 1. Fix SQL Injection

**Before:**
```csharp
var query = $"SELECT * FROM Patients WHERE SSN = '{ssn}'";
```

**After:**
```csharp
using (var cmd = new SqlCommand("SELECT * FROM Patients WHERE SSN = @ssn", connection))
{
    cmd.Parameters.AddWithValue("@ssn", ssn);
    return cmd.ExecuteScalar() != null;
}
```

---

#### 2. Hash Passwords

**Before:**
```csharp
public string Password { get; set; }

public bool ValidatePassword(string input)
{
    return Password == input;
}
```

**After:**
```csharp
private string _passwordHash;  // Never expose directly

public void SetPassword(string password)
{
    _passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
}

public bool ValidatePassword(string input)
{
    return BCrypt.Net.BCrypt.Verify(input, _passwordHash);
}
```

---

#### 3. Encrypt Sensitive Data at Rest

**Before:**
```csharp
public string SSN { get; set; }
```

**After:**
```csharp
private string _encryptedSSN;

public string SSN
{
    get => EncryptionService.Decrypt(_encryptedSSN);
    set => _encryptedSSN = EncryptionService.Encrypt(value);
}
```

---

#### 4. Remove Sensitive Data from Logs

**Before:**
```csharp
Console.WriteLine($"SSN={patient.SSN}");
```

**After:**
```csharp
Console.WriteLine($"PatientID={patient.PatientID}");  // Log ID only
```

---

### Short-Term Improvements (Priority 2)

#### 1. Reduce CIVPF

**Strategy:** Keep sensitive data in source classes, pass only IDs

**Before:**
```csharp
public void Create(PatientRecord patient, Doctor doctor)
{
    RawPatientSSN = patient.SSN;  // Duplication!
}
```

**After:**
```csharp
public void Create(int patientId, int doctorId)
{
    PatientID = patientId;  // Store ID only
    DoctorID = doctorId;
    // Retrieve sensitive data only when absolutely necessary
}
```

**Impact:** Reduces CIVPF from 3 to 1

---

#### 2. Reduce VCC

**Strategy:** Use dependency injection and interfaces to decouple classes

**Before:**
```csharp
public class Prescription
{
    private PatientRecord _patient;
    private Doctor _doctor;
    private PharmacyAdapter _pharmacy;
}
```

**After:**
```csharp
public class Prescription
{
    private readonly IPatientRepository _patientRepo;
    private readonly IDoctorRepository _doctorRepo;
    // Retrieve data through repositories, not direct coupling
}
```

**Impact:** Reduces VCC from 3 to 0 (coupling through interfaces)

---

#### 3. Implement Access Control

**Strategy:** Use private setters and validation methods

**Before:**
```csharp
public string SSN { get; set; }
```

**After:**
```csharp
private string _ssn;

public string GetSSN(IAuthContext context)
{
    if (!context.HasPermission("READ_PII"))
        throw new UnauthorizedAccessException();
    
    AuditLog.LogAccess(context.UserId, "SSN", PatientID);
    return _ssn;
}
```

---

#### 4. Add Input Validation

**Strategy:** Validate all inputs at entry points

**Example:**
```csharp
public void SetSSN(string ssn)
{
    if (!Regex.IsMatch(ssn, @"^\d{3}-\d{2}-\d{4}$"))
        throw new ArgumentException("Invalid SSN format");
    
    _ssn = ssn;
}
```

---

### Long-Term Improvements (Priority 3)

#### 1. Implement Data Masking

**Strategy:** Return masked versions of sensitive data for display

```csharp
public string GetMaskedSSN()
{
    return $"***-**-{SSN.Substring(7)}";  // Returns: ***-**-6789
}
```

---

#### 2. Use Secure Vault for Secrets

**Strategy:** Store API keys, connection strings in Azure Key Vault / AWS Secrets Manager

```csharp
public class PharmacyAdapter
{
    private readonly ISecretManager _secretManager;
    
    private string GetAPIKey()
    {
        return _secretManager.GetSecret("PharmacyAPIKey");
    }
}
```

---

#### 3. Implement TLS/Encryption for External Communication

**Strategy:** Use HTTPS with certificate pinning

```csharp
private bool SendToExternalAPI(PrescriptionDTO dto)
{
    using var client = new HttpClient(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = ValidateCertificate
    });
    
    var encryptedPayload = EncryptionService.Encrypt(dto);
    var response = await client.PostAsync(apiUrl, encryptedPayload);
    return response.IsSuccessStatusCode;
}
```

---

#### 4. Implement Proper Audit Logging

**Strategy:** Log access to sensitive data without logging the data itself

```csharp
public void LogSensitiveAccess(string userId, string dataType, string recordId)
{
    var logEntry = new AuditEntry
    {
        Timestamp = DateTime.UtcNow,
        UserId = userId,
        Action = "ACCESS",
        DataType = dataType,      // "SSN", "AuthToken", etc.
        RecordId = recordId,      // Patient ID, not SSN
        IPAddress = GetClientIP()
    };
    
    SecureAuditLog.Write(logEntry);
}
```

---

### Metric Improvement Targets

| Metric | Current | Target | Improvement Strategy |
|--------|---------|--------|---------------------|
| AVR | 0.50 | ≤ 0.30 | Encrypt sensitive attributes, use private setters |
| VCC | 6 | ≤ 3 | Decouple classes, use repositories/interfaces |
| CIVPF | 3 | ≤ 1 | Stop data duplication, pass IDs not objects |
| VA | 0.55 | ≤ 0.40 | Create safe accessor methods, implement masking |

---

## Conclusion

The MediLink system demonstrates **critical security vulnerabilities** across all measured dimensions:

- **AVR of 0.50** indicates half of all attributes are vulnerable
- **CIVPF of 3** shows sensitive data propagates through multiple layers
- **VCC of 6** reveals excessive coupling with vulnerable data exchange
- **VA of 0.55** means most methods expose significant sensitive data

### Vulnerable Attributes Summary

The system contains **12 vulnerable attributes** across 5 classes:

#### PatientRecord (4 vulnerable attributes)
- **`DOB`** (DateTime): Date of birth is Personally Identifiable Information (PII) stored without encryption, enabling identity theft and violating privacy regulations like HIPAA.
- **`SSN`** (string): Social Security Number stored in plain text is a critical PII violation. SSNs should be encrypted at rest and only decrypted when absolutely necessary with proper authorization.
- **`MedicalHistory`** (string): Protected Health Information (PHI) accessible without role-based access control or encryption, violating HIPAA requirements for medical record protection.
- **`CreditCardToken`** (string): Despite being named "token," this attribute stores payment information improperly, violating PCI-DSS standards which require proper tokenization or encryption of payment card data.

#### Doctor (3 vulnerable attributes)
- **`AuthToken`** (string): Authentication token stored in plain text without encryption, rotation, or expiration. Compromised tokens grant unauthorized access to doctor privileges.
- **`PrivateKey`** (string): Cryptographic private key exposed as a public property enables signature forgery and breaks the entire prescription signing mechanism's security.
- **`Password`** (string): Password stored in plain text instead of using industry-standard hashing algorithms (bcrypt, Argon2), allowing complete account compromise if the database is breached.

#### Prescription (2 vulnerable attributes)
- **`DoctorAuthToken`** (string): Duplicated from the Doctor class, this creates an additional attack surface and violates the single source of truth principle, making credential rotation impossible.
- **`RawPatientSSN`** (string): Copied from PatientRecord for "convenience," this duplication amplifies the CIVPF score and creates multiple breach points for the same sensitive data.

#### PharmacyAdapter (2 vulnerable attributes)
- **`APIKey`** (string): External API key stored as a class property instead of in a secure vault (Azure Key Vault, AWS Secrets Manager), making it accessible to any code with a class reference.
- **`ConnectionString`** (string): Database connection string containing embedded credentials stored in plain text, violating secure configuration management practices.

#### AuditLogger (1 vulnerable attribute)
- **`LogFilePath`** (string): File system path exposure combined with the class's tendency to log sensitive data creates a persistent security vulnerability where log files become high-value targets.

**Primary Risk:** The combination of high AVR, CIVPF, and VCC creates a "vulnerability cascade" where a breach at any point exposes data across multiple system layers.

**Recommended Action:** Implement Priority 1 fixes immediately, followed by systematic refactoring to reduce coupling and improve encapsulation.

---

*Analysis completed: December 7, 2025*  
*Analyst: Security Metrics Team*  
*Classification: Educational Use Only*
