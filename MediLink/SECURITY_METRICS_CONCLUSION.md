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

- **AVR of 0.43** indicates 43% of all attributes are vulnerable
- **CIVPF of 2** shows sensitive data propagates through 2 layers (data duplication)
- **VCC of 3** reveals moderate coupling with vulnerable data exchange
- **VA of 0.50** means half of the methods expose significant sensitive data

### System-Wide Metrics Summary

| Metric | Score | Threshold | Status | Severity |
|--------|-------|-----------|--------|----------|
| **System AVR** | 0.43 (43%) | ≤ 0.30 | ⚠️ MODERATE | MODERATE |
| **System VCC** | 3 couplings | ≤ 3 | ⚠️ MODERATE | MODERATE |
| **Max CIVPF** | 2 hops | ≤ 2 | ⚠️ MODERATE | MODERATE |
| **Avg Method VA** | 0.50 (50%) | ≤ 0.40 | ⚠️ MODERATE | MODERATE |

### Class-Level Summary

| Class | Safe | Vulnerable | Total | AVR | Status |
|-------|------|------------|-------|-----|--------|
| PatientRecord | 2 | 4 | 6 | 0.67 | 🔴 POOR |
| Doctor | 3 | 2 | 5 | 0.40 | ⚠️ MODERATE |
| Prescription | 4 | 2 | 6 | 0.33 | ⚠️ MODERATE |
| PharmacyAdapter | 2 | 1 | 3 | 0.33 | ⚠️ MODERATE |
| **SYSTEM TOTAL** | **11** | **9** | **20** | **0.45** | **⚠️ MODERATE** |

---

## Vulnerable Attributes Summary

The system contains **9 vulnerable attributes** across 4 classes:

### PatientRecord (4 vulnerable attributes)

- **`DOB`** (DateTime): Date of birth is Personally Identifiable Information (PII) stored without encryption, enabling identity theft and violating privacy regulations like HIPAA.

- **`SSN`** (string): Social Security Number stored in plain text is a critical PII violation. SSNs should be encrypted at rest and only decrypted when absolutely necessary with proper authorization.

- **`MedicalHistory`** (string): Protected Health Information (PHI) accessible without role-based access control or encryption, violating HIPAA requirements for medical record protection.

- **`CardToken`** (string): Despite being named "token," this attribute stores payment card information improperly. Payment data should follow proper tokenization standards or be encrypted according to PCI-DSS requirements.

### Doctor (2 vulnerable attributes)

- **`AuthToken`** (string): Authentication token stored in plain text without encryption, rotation, or expiration. Compromised tokens grant unauthorized access to doctor privileges and should be stored securely with proper lifecycle management.

- **`Password`** (string): Password stored in plain text instead of using industry-standard hashing algorithms (bcrypt, Argon2), allowing complete account compromise if the database is breached. This is a critical security flaw.

### Prescription (2 vulnerable attributes)

- **`DoctorAuthToken`** (string): Duplicated from the Doctor class, this creates an additional attack surface and violates the single source of truth principle. Data duplication makes credential rotation impossible and amplifies the CIVPF score.

- **`RawPatientSSN`** (string): Copied from PatientRecord for "convenience," this duplication amplifies the CIVPF score from 1 to 2 hops and creates multiple breach points for the same sensitive data. This is a prime example of how data duplication increases vulnerability.

### PharmacyAdapter (1 vulnerable attribute)

- **`ConnectionString`** (string): Database connection string containing embedded credentials stored in plain text, violating secure configuration management practices. Connection strings should be encrypted and stored in secure configuration systems or vaults.

---

## Key Improvements from Simplification

The simplified MediLink system has **improved** compared to a more complex implementation:

### Removed Vulnerabilities

1. **Removed PrivateKey from Doctor class**: Eliminated cryptographic key exposure vulnerability, reducing Doctor's AVR from 0.50 to 0.40.

2. **Removed AuditLogger class**: Eliminated all logging-related vulnerabilities including:
   - Logging of SSNs, passwords, and tokens
   - Exposure of log file paths
   - Persistent storage of sensitive data in log files

3. **Simplified PharmacyAdapter**: Removed external API transmission, reducing:
   - APIKey exposure vulnerability
   - External data transmission risks
   - CIVPF propagation to external systems
   - AVR reduced from 0.50 to 0.33

4. **Removed GetForPharmacy() and Transmit() methods**: Eliminated the final hop in CIVPF chain, reducing maximum CIVPF from 3 to 2.

### Simplified Architecture

- **Pharmacist operations**: Now simply receives prescriptions and marks them as "fulfilled" without external transmission
- **No external data leakage**: Sensitive data no longer leaves the system boundary
- **Reduced coupling**: VCC reduced from 6 to 3 by removing PharmacyAdapter coupling from Prescription

---

## Remaining Critical Issues

Despite simplifications, the following critical vulnerabilities remain:

### 1. Plain Text Credential Storage

**Affected Classes:** `Doctor`, `PharmacyAdapter`

**Details:**
- Passwords stored without hashing
- Authentication tokens stored in plain text
- Database connection strings with embedded credentials

**Impact:** Complete credential compromise if any part of the system is breached

**CVSS Score:** 9.8 (Critical)

---

### 2. SQL Injection Vulnerability

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

### 3. Data Duplication (CIVPF Amplification)

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
- Amplifies CIVPF from 1 to 2 hops

**CVSS Score:** 7.5 (High)

---

### 4. Unencrypted Sensitive Data Storage

**Affected Classes:** `PatientRecord`

**Details:**
- SSN stored in plain text
- Medical history unencrypted
- Date of birth unprotected
- Card tokens improperly secured

**Impact:** Privacy violations, HIPAA non-compliance, identity theft risk

**CVSS Score:** 8.2 (High)

---

## CIVPF Propagation Chains

### Chain 1: Patient SSN Propagation

**CIVPF Score: 2 hops** ⚠️ **MODERATE**

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
│ Hop 2: Prescription.RawPatientSSN (Terminal)                │
│ - Attribute: RawPatientSSN = "123-45-6789"                  │
│ - Duplication: Data copied for "convenience"                │
│ - Protection: None                                           │
│ - Terminal: Data stays within system (no external leak)     │
└─────────────────────────────────────────────────────────────┘
```

**Vulnerability Impact:**
- SSN exposed at 2 different points in the system
- Each hop increases breach surface area
- Data duplication violates single source of truth

---

### Chain 2: Doctor AuthToken Propagation

**CIVPF Score: 2 hops** ⚠️ **MODERATE**

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
│ Hop 2: Prescription.DoctorAuthToken (Terminal)              │
│ - Attribute: DoctorAuthToken = "eyJhbGci..."                │
│ - Duplication: Token copied to prescription                 │
│ - Protection: None                                           │
│ - Terminal: Data stays within system (no external leak)     │
└─────────────────────────────────────────────────────────────┘
```

**Vulnerability Impact:**
- Authentication token duplicated in prescription
- Token could be intercepted from multiple locations
- No token scoping or expiration
- Credential rotation becomes impossible

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

#### 4. Eliminate Data Duplication

**Before:**
```csharp
public void Create(PatientRecord patient, Doctor doctor)
{
    RawPatientSSN = patient.SSN;  // Duplication!
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

---

### Metric Improvement Targets

| Metric | Current | Target | Improvement Strategy |
|--------|---------|--------|---------------------|
| AVR | 0.45 | ≤ 0.30 | Encrypt sensitive attributes, use private setters |
| VCC | 3 | ≤ 2 | Decouple classes, use repositories/interfaces |
| CIVPF | 2 | ≤ 1 | Stop data duplication, pass IDs not objects |
| VA | 0.50 | ≤ 0.40 | Create safe accessor methods, implement masking |

---

## Final Assessment

**Primary Risk:** The combination of moderate AVR (0.45), CIVPF (2), and VCC (3) creates a "vulnerability cascade" where a breach at any point exposes data across multiple system layers.

**Positive Changes:** The simplified architecture has successfully:
- Reduced system complexity
- Eliminated external data transmission vulnerabilities
- Removed logging-related security issues
- Decreased overall attack surface

**Recommended Action:** Implement Priority 1 fixes immediately (SQL injection, password hashing, data encryption, duplication elimination), followed by systematic refactoring to reduce coupling and improve encapsulation.

---

*Analysis completed: December 7, 2025*  
*Analyst: Security Metrics Team*  
*Classification: Educational Use Only*
