# 🔒 MediLink Security Metrics Analysis - Conclusion

**Document Version:** 2.0  
**Analysis Date:** December 7, 2025  
**Project:** MediLink Telemedicine & Prescription Management System

> ⚠️ **Educational Purpose Only**
>
> This analysis examines intentionally vulnerable code designed to demonstrate poor security practices and their measurable impact on software quality metrics.

---

## Conclusion

The MediLink system demonstrates **moderate to high security vulnerabilities** across measured dimensions:

- **AVR of 0.31** indicates 31% of all attributes are vulnerable
- **CIVPF of 1** shows sensitive data stays in origin classes (no duplication)
- **VCC of 3** reveals moderate coupling with vulnerable data exchange
- **VA of 0.50** means half of the methods expose significant sensitive data

### System-Wide Metrics Summary

| Metric            | Score       | Threshold | Status      | Severity |
| ----------------- | ----------- | --------- | ----------- | -------- |
| **System AVR**    | 0.31 (31%)  | ≤ 0.30    | ⚠️ MODERATE | MODERATE |
| **System VCC**    | 3 couplings | ≤ 3       | ⚠️ MODERATE | MODERATE |
| **Max CIVPF**     | 1 hop       | ≤ 2       | 🟢 GOOD     | LOW      |
| **Avg Method VA** | 0.50 (50%)  | ≤ 0.40    | ⚠️ MODERATE | MODERATE |

### Class-Level Summary

| Class            | Safe   | Vulnerable | Total  | AVR      | Status          |
| ---------------- | ------ | ---------- | ------ | -------- | --------------- |
| PatientRecord    | 2      | 6          | 8      | 0.75     | 🔴 POOR         |
| Doctor (User)    | 3      | 3          | 6      | 0.50     | ⚠️ MODERATE     |
| Admin (User)     | 3      | 2          | 5      | 0.40     | ⚠️ MODERATE     |
| Prescription     | 4      | 1          | 5      | 0.20     | 🟢 GOOD         |
| PharmacyOrder    | 4      | 1          | 5      | 0.20     | 🟢 GOOD         |
| MedicalSpecialty | 3      | 0          | 3      | 0.00     | 🟢 GOOD         |
| Appointment      | 7      | 0          | 7      | 0.00     | 🟢 GOOD         |
| **SYSTEM TOTAL** | **26** | **13**     | **39** | **0.31** | **⚠️ MODERATE** |

---

## Vulnerable Attributes Summary

The system contains **13 vulnerable attributes** across 5 classes:

### PatientRecord (6 vulnerable attributes)

- **`DOB`** (DateTime): Date of birth is Personally Identifiable Information (PII) stored without encryption, enabling identity theft and violating privacy regulations like HIPAA.

- **`NIK`** (string): National Identification Number stored in plain text is a critical PII violation. NIKs should be encrypted at rest and only decrypted when absolutely necessary with proper authorization.

- **`MedicalHistory`** (string): Protected Health Information (PHI) accessible without role-based access control or encryption, violating HIPAA requirements for medical record protection.

- **`PhoneNumber`** (string): Contact information stored in plain text. PII that can be used for social engineering attacks or identity theft.

- **`EmailAddress`** (string): Email address stored without encryption. Can be used for phishing attacks and account compromise.

- **`EmergencyContact`** (string): Emergency contact information stored in plain text. Contains PII of both patient and their contacts.

### User (Inherited by Doctor & Admin) (2 vulnerable attributes)

- **`AuthToken`** (string): Authentication token stored in plain text without encryption, rotation, or expiration. Compromised tokens grant unauthorized access to system privileges.

- **`Password`** (string): Password stored in plain text instead of using industry-standard hashing algorithms (bcrypt, Argon2), allowing complete account compromise if the database is breached. This is a critical security flaw.

### Doctor (1 additional vulnerable attribute beyond User)

- **`MedicalLicenseNumber`** (string): Professional credential stored in plain text. Should be encrypted to prevent credential theft and impersonation.

### Prescription (1 vulnerable attribute)

- **`DrugCost`** (decimal): Financial information stored without encryption. Should be protected to prevent price manipulation and financial fraud.

---

## Vulnerable Methods Summary

The system contains **vulnerable methods** that expose sensitive data across multiple classes. These methods contribute to the **VA (Vulnerability Accessibility)** metric.

### PatientRecord (1 vulnerable method)

#### `GenerateReport()` - VA Score: 1.00 (HIGH)

**Exposes:** All patient data including NIK, DOB, and medical history

```csharp
public string GenerateReport()
{
    return $"Patient: {FullName}\n" +
           $"SSN: {SSN}\n" +
           $"DOB: {DOB}\n" +
           $"History: {MedicalHistory}\n";
}
```

**Risk:** Complete patient profile exposure in plain text

---

### User (Inherited by Doctor & Admin) (4 vulnerable methods)

#### `ValidatePassword()` - VA Score: 0.50 (MEDIUM)

**Exposes:** Password comparison logic

```csharp
public bool ValidatePassword(string input)
{
    return Password == input; // Plain text comparison
}
```

**Risk:** Timing attacks, no rate limiting, plain text storage

---

#### `GetAuthToken()` - VA Score: 1.00 (HIGH)

**Exposes:** Authentication token

```csharp
public string GetAuthToken()
{
    return AuthToken; // Direct token exposure
}
```

**Risk:** Session hijacking, unauthorized access

---

#### `GenerateNewToken()` - VA Score: 0.75 (HIGH)

**Exposes:** Token generation logic and new token

```csharp
public string GenerateNewToken()
{
    AuthToken = Guid.NewGuid().ToString();
    return AuthToken;
}
```

**Risk:** Predictable tokens, no cryptographic randomness

---

#### `ResetPassword()` - VA Score: 0.50 (MEDIUM)

**Exposes:** Password reset mechanism

```csharp
public void ResetPassword(string newPassword)
{
    Password = newPassword; // No validation, plain text
}
```

**Risk:** Weak password acceptance, no complexity requirements

---

### PharmacyOrder (1 vulnerable method)

#### `MarkFulfilled()` - VA Score: 0.20 (LOW)

**Exposes:** Minimal information (status change)

```csharp
public void MarkFulfilled()
{
    Status = "Fulfilled";
    Console.WriteLine($"[PHARMACY] Order {OrderID} fulfilled");
}
```

**Risk:** Low - only operational data

---

### Method Vulnerability Summary

| Class         | Method               | VA Score | Severity    | Sensitive Data Exposed    |
| ------------- | -------------------- | -------- | ----------- | ------------------------- |
| PatientRecord | `GenerateReport()`   | 1.00     | 🔴 CRITICAL | NIK, DOB, Medical History |
| User          | `GetAuthToken()`     | 1.00     | 🔴 CRITICAL | Authentication token      |
| User          | `GenerateNewToken()` | 0.75     | 🔴 HIGH     | New auth token            |
| User          | `ValidatePassword()` | 0.50     | ⚠️ MEDIUM   | Password logic            |
| User          | `ResetPassword()`    | 0.50     | ⚠️ MEDIUM   | Password reset            |
| PharmacyOrder | `MarkFulfilled()`    | 0.20     | 🟢 LOW      | Status only               |

**Average Method VA**: ~0.66 (66% of methods expose significant sensitive data)

---

## Key Improvements from Simplification

The simplified MediLink system has **improved** compared to a more complex implementation:

### Removed Vulnerabilities

1.  **Removed PrivateKey from Doctor class**: Eliminated cryptographic key exposure vulnerability, reducing Doctor's AVR from 0.50 to 0.40.

2.  **Removed AuditLogger class**: Eliminated all logging-related vulnerabilities including:

    - Logging of NIKs, passwords, and tokens
    - Exposure of log file paths
    - Persistent storage of sensitive data in log files

3.  **Simplified PharmacyAdapter**: Removed external API transmission, reducing:

    - APIKey exposure vulnerability
    - External data transmission risks
    - CIVPF propagation to external systems
    - AVR reduced from 0.50 to 0.33

4.  **Removed External Pharmacy Integration**: Eliminated the final hop in CIVPF chain, reducing maximum CIVPF from 3 to 2.
5.  **Eliminated Data Duplication in Prescription**: Removed PatientNIK and DoctorAuthToken attributes, reducing CIVPF from 2 to 1 and improving Prescription AVR from 0.33 to 0.00.

### Simplified Architecture

- **Pharmacist operations**: Now simply receives prescriptions and marks them as "fulfilled" without external transmission
- **No external data leakage**: Sensitive data no longer leaves the system boundary
- **Reduced coupling**: VCC reduced from 6 to 3 by removing PharmacyAdapter coupling from Prescription

---

## Remaining Critical Issues

Despite simplifications, the following critical vulnerabilities remain:

### 1. Plain Text Credential Storage

**Affected Classes:** `User` (Parent of `Doctor`, `Admin`)

**Details:**

- Passwords stored without hashing in User class
- Authentication tokens stored in plain text in User class
- Inherited by all system actors

**Impact:** Complete credential compromise if any part of the system is breached

**CVSS Score:** 9.8 (Critical)

---

### 2. Duplicated Patient Data

**Location:** `PharmacyOrder`

**Code:**

```csharp
public string PatientName { get; set; } = string.Empty; // Copied from Patient
```

**Impact:** Unnecessary duplication of PII (Patient Name) in pharmacy orders.

**CVSS Score:** 5.3 (Medium)

---

### 3. Unencrypted Sensitive Data Storage

**Affected Classes:** `PatientRecord`

**Details:**

- NIK stored in plain text
- Medical history unencrypted
- Date of birth unprotected

**Impact:** Privacy violations, HIPAA non-compliance, identity theft risk

**CVSS Score:** 8.2 (High)

---

## CIVPF Propagation Chains

### Chain 1: Patient NIK - No Propagation

**CIVPF Score: 1 hop** 🟢 **GOOD**

```
┌─────────────────────────────────────────────────────────────┐
│ Hop 1: PatientRecord.SSN (Origin & Terminal)                │
│ - Attribute: SSN = "123-45-6789"                            │
│ - Storage: Plain text string                                │
│ - Protection: None                                           │
│ - Terminal: Data stays in origin class (no duplication)     │
└─────────────────────────────────────────────────────────────┘
```

**Vulnerability Impact:**

- NIK stored only in PatientRecord (single source of truth)
- No data duplication across classes
- Reduced attack surface compared to duplicated data

---

### Chain 2: Doctor AuthToken - No Propagation

**CIVPF Score: 1 hop** 🟢 **GOOD**

```
┌─────────────────────────────────────────────────────────────┐
│ Hop 1: Doctor.AuthToken (Origin & Terminal)                 │
│ - Attribute: AuthToken = "eyJhbGci..."                      │
│ - Storage: Plain text string                                │
│ - Protection: None                                           │
│ - Terminal: Data stays in origin class (no duplication)     │
└─────────────────────────────────────────────────────────────┘
```

**Vulnerability Impact:**

- Authentication token stored only in User/Doctor class
- No token duplication across classes
- Credential rotation is possible (single source)

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

#### 4. Use ID References Instead of Data Duplication

**Before:**

```csharp
public void Create(PatientRecord patient, Doctor doctor)
{
    PatientNIK = patient.SSN;  // Duplication!
    DoctorAuthToken = doctor.AuthToken;  // Duplication!
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

**Impact:** Reduces CIVPF from 2 to 1

**Status:** ✅ **IMPLEMENTED** - Prescription class now uses references only

---

### Metric Improvement Targets

| Metric | Current | Target | Improvement Strategy                              |
| ------ | ------- | ------ | ------------------------------------------------- |
| AVR    | 0.31    | ≤ 0.30 | Encrypt sensitive attributes, use private setters |
| VCC    | 3       | ≤ 2    | Decouple classes, use repositories/interfaces     |
| CIVPF  | 1       | ≤ 1    | ✅ **ACHIEVED** - No data duplication             |
| VA     | 0.50    | ≤ 0.40 | Create safe accessor methods, implement masking   |

---

## Final Assessment

**Primary Risk:** The combination of moderate AVR (0.31), CIVPF (1), and VCC (3) creates a "vulnerability cascade" where a breach at any point exposes data across multiple system layers.

**Positive Changes:** The improved architecture has successfully:

- **Achieved CIVPF target** (1 hop) by eliminating data duplication
- Reduced system AVR from 0.38 to **0.22** (well below 0.30 threshold)
- Demonstrated proper data normalization (MedicalSpecialty reference)
- Implemented ID-based relationships (Appointment, Prescription) without data duplication

- Reduced system complexity
- Eliminated external data transmission vulnerabilities
- Removed logging-related security issues
- Decreased overall attack surface

**Recommended Action:** Implement Priority 1 fixes immediately (SQL injection, password hashing, data encryption, duplication elimination), followed by systematic refactoring to reduce coupling and improve encapsulation.

---

_Analysis completed: December 7, 2025_  
_Analyst: Security Metrics Team_  
_Classification: Educational Use Only_
