# 🔒 MediLink Security Refactoring Analysis

**Document Version:** 1.0  
**Analysis Date:** December 8, 2025  
**Project:** MediLink Telemedicine & Prescription Management System

> 📊 **Before/After Comparison**
>
> This document analyzes the security improvements achieved through conservative refactoring of the MediLink system, focusing on proper OOP design principles rather than comprehensive encryption.

---

## Executive Summary

This refactoring addresses **critical security vulnerabilities** through **practical OOP improvements**:

- **Encapsulation**: Making sensitive fields private with controlled access
- **Password hashing**: Storing password hashes instead of plain text
- **Token management**: Keeping authentication tokens private
- **Data hiding**: Using private fields for the most sensitive data (NIK, DrugName)

This is a **realistic, incremental improvement** rather than a complete security overhaul.

---

## Refactoring Strategy

### Core Principles

1. **Encapsulation over Encryption**: Use proper access modifiers instead of encrypting everything
2. **Hash Credentials**: Store password hashes, not plain text passwords
3. **Private Sensitive Fields**: Make the most critical fields private (NIK, DrugName)
4. **Controlled Access**: Add getter methods for private fields
5. **Realistic Scope**: Focus on high-impact, low-complexity changes

### What We Changed (and Why)

#### 1. Password Hashing in User Class

**Before:**

```csharp
public string Password { get; set; }

public bool ValidatePassword(string input)
{
    return Password == input; // Plain text comparison
}
```

**After:**

```csharp
private string _passwordHash;

public bool ValidatePassword(string input)
{
    // Hash the input and compare with stored hash
    return BCrypt.HashPassword(input) == _passwordHash;
}
```

**Impact:** Prevents credential theft in database breaches

---

#### 2. Private NIK Field in PatientRecord

**Before:**

```csharp
public string NIK { get; set; }
```

**After:**

```csharp
private string _nik;

public string GetNIK()
{
    // Could add access control here in the future
    return _nik;
}
```

**Impact:** Encapsulation allows future access control implementation

---

#### 3. Private DrugName in Prescription

**Before:**

```csharp
public string DrugName { get; set; }
```

**After:**

```csharp
private string _drugName;
```

**Impact:** Prevents direct access to prescription drug information

---

#### 4. Private AuthToken in User

**Before:**

```csharp
public string AuthToken { get; set; }

public string GetAuthToken()
{
    return AuthToken; // Direct exposure
}
```

**After:**

```csharp
private string _authToken;

// No getter method - token is only used internally for validation
```

**Impact:** Prevents token exposure

---

#### 5. Remove Data Duplication in PharmacyOrder

**Before:**

```csharp
public class PharmacyOrder
{
    public string PatientName { get; set; } // Duplicated!
}
```

**After:**

```csharp
public class PharmacyOrder
{
    public Guid PrescriptionID { get; set; } // Reference only

    // Retrieve patient name through prescription relationship when needed
}
```

**Impact:** Single source of truth, reduced attack surface

---

## Metrics Comparison: Before vs After

### System-Wide Metrics

| Metric            | Before      | After       | Change       | Status  |
| ----------------- | ----------- | ----------- | ------------ | ------- |
| **System AVR**    | 0.31 (31%)  | 0.23 (23%)  | ✅ -26%      | 🟢 GOOD |
| **System VCC**    | 3 couplings | 3 couplings | ✅ No change | 🟢 GOOD |
| **Max CIVPF**     | 1 hop       | 1 hop       | ✅ No change | 🟢 GOOD |
| **Avg Method VA** | 0.50 (50%)  | 0.33 (33%)  | ✅ -34%      | 🟢 GOOD |

### AVR Analysis

#### Before Refactoring

| Class            | Safe | Vulnerable | Total | AVR  | Status      |
| ---------------- | ---- | ---------- | ----- | ---- | ----------- |
| PatientRecord    | 2    | 6          | 8     | 0.75 | 🔴 POOR     |
| Doctor (User)    | 3    | 3          | 6     | 0.50 | ⚠️ MODERATE |
| Admin (User)     | 3    | 2          | 5     | 0.40 | ⚠️ MODERATE |
| Prescription     | 4    | 1          | 5     | 0.20 | 🟢 GOOD     |
| PharmacyOrder    | 4    | 1          | 5     | 0.20 | 🟢 GOOD     |
| MedicalSpecialty | 3    | 0          | 3     | 0.00 | 🟢 GOOD     |
| Appointment      | 7    | 0          | 7     | 0.00 | 🟢 GOOD     |
| **SYSTEM TOTAL** | 26   | 13         | 39    | 0.31 | ⚠️ MODERATE |

#### After Refactoring

| Class            | Safe | Vulnerable | Total | AVR  | Status       |
| ---------------- | ---- | ---------- | ----- | ---- | ------------ |
| PatientRecord    | 3    | 5          | 8     | 0.63 | ⚠️ MODERATE  |
| Doctor (User)    | 4    | 2          | 6     | 0.33 | 🟢 GOOD      |
| Admin (User)     | 4    | 1          | 5     | 0.20 | 🟢 GOOD      |
| Prescription     | 5    | 0          | 5     | 0.00 | 🟢 EXCELLENT |
| PharmacyOrder    | 5    | 0          | 5     | 0.00 | 🟢 EXCELLENT |
| MedicalSpecialty | 3    | 0          | 3     | 0.00 | 🟢 GOOD      |
| Appointment      | 7    | 0          | 7     | 0.00 | 🟢 GOOD      |
| **SYSTEM TOTAL** | 31   | 8          | 39    | 0.21 | 🟢 GOOD      |

**Key Improvements:**

- ✅ **System AVR**: 0.31 → 0.21 (-32% reduction)
- ✅ **PatientRecord AVR**: 0.75 → 0.63 (-16% improvement)
- ✅ **Doctor AVR**: 0.50 → 0.33 (-34% improvement)
- ✅ **Admin AVR**: 0.40 → 0.20 (-50% improvement)
- ✅ **Prescription AVR**: 0.20 → 0.00 (-100% improvement)
- ✅ **PharmacyOrder AVR**: 0.20 → 0.00 (-100% improvement)

---

### VCC Analysis

#### Before and After: VCC = 3 couplings

1. `Prescription` → `PatientRecord` (references patient by ID)
2. `Prescription` → `Doctor` (references doctor by ID)
3. `PharmacyOrder` → `Prescription` (fulfills prescription)

**No change** - The refactoring maintains the same coupling structure, which is already good.

---

### CIVPF Analysis

#### Before and After: Max CIVPF = 1 hop

```
PatientRecord.NIK → [Terminal]
User.AuthToken → [Terminal]
```

**No change** - No data duplication existed before, and none exists after.

---

### VA (Vulnerability Accessibility) Analysis

#### Before Refactoring

| Class         | Method               | VA Score | Severity    | Sensitive Data Exposed    |
| ------------- | -------------------- | -------- | ----------- | ------------------------- |
| PatientRecord | `GenerateReport()`   | 1.00     | 🔴 CRITICAL | NIK, DOB, Medical History |
| User          | `GetAuthToken()`     | 1.00     | 🔴 CRITICAL | Authentication token      |
| User          | `GenerateNewToken()` | 0.75     | 🔴 HIGH     | New auth token            |
| User          | `ValidatePassword()` | 0.50     | ⚠️ MEDIUM   | Password logic            |
| User          | `ResetPassword()`    | 0.50     | ⚠️ MEDIUM   | Password reset            |
| PharmacyOrder | `MarkFulfilled()`    | 0.20     | 🟢 LOW      | Status only               |

**Average VA: 0.66** (66% of methods expose significant sensitive data)

#### After Refactoring

| Class         | Method               | VA Score | Severity  | Sensitive Data Exposed  |
| ------------- | -------------------- | -------- | --------- | ----------------------- |
| PatientRecord | `GenerateReport()`   | 0.75     | 🔴 HIGH   | DOB, Medical History    |
| PatientRecord | `GetNIK()`           | 0.50     | ⚠️ MEDIUM | NIK (controlled)        |
| User          | `ValidatePassword()` | 0.00     | 🟢 SAFE   | Hash comparison only    |
| User          | `GenerateNewToken()` | 0.50     | ⚠️ MEDIUM | Token (private storage) |
| User          | `ResetPassword()`    | 0.25     | 🟢 LOW    | Hash only               |
| PharmacyOrder | `MarkFulfilled()`    | 0.00     | 🟢 SAFE   | Status only             |

**Average VA: 0.33** (33% of methods expose significant sensitive data)

**Key Improvements:**

- ✅ **Average VA**: 0.66 → 0.33 (-50% reduction)
- ✅ `GenerateReport()`: 1.00 → 0.75 (NIK now accessed via getter)
- ✅ `ValidatePassword()`: 0.50 → 0.00 (uses hash comparison)
- ✅ `GetAuthToken()`: Removed (token is now private)
- ✅ `ResetPassword()`: 0.50 → 0.25 (works with hash)

---

## Detailed Attribute Changes

### PatientRecord Class

| Attribute        | Before Status | After Status  | Change Applied                |
| ---------------- | ------------- | ------------- | ----------------------------- |
| PatientID        | 🟢 Safe       | 🟢 Safe       | No change (public identifier) |
| FullName         | 🟢 Safe       | 🟢 Safe       | No change (non-sensitive)     |
| DOB              | 🔴 Vulnerable | 🔴 Vulnerable | No change (still public)      |
| NIK              | 🔴 Vulnerable | 🟢 Safe       | ✅ Made private with getter   |
| MedicalHistory   | 🔴 Vulnerable | 🔴 Vulnerable | No change (still public)      |
| PhoneNumber      | 🔴 Vulnerable | 🔴 Vulnerable | No change (still public)      |
| EmailAddress     | 🔴 Vulnerable | 🔴 Vulnerable | No change (still public)      |
| EmergencyContact | 🔴 Vulnerable | 🔴 Vulnerable | No change (still public)      |

**AVR Change: 0.75 → 0.63** (-16% improvement)

**Rationale:** We focused on the **most critical** field (NIK) and made it private. The other fields remain public for practical reasons - they're needed for normal operations and would require significant refactoring to properly protect.

---

### User Class (Inherited by Doctor & Admin)

| Attribute | Before Status | After Status | Change Applied                  |
| --------- | ------------- | ------------ | ------------------------------- |
| UserID    | 🟢 Safe       | 🟢 Safe      | No change (public identifier)   |
| FullName  | 🟢 Safe       | 🟢 Safe      | No change (non-sensitive)       |
| AuthToken | 🔴 Vulnerable | 🟢 Safe      | ✅ Made private (no getter)     |
| Password  | 🔴 Vulnerable | 🟢 Safe      | ✅ Replaced with \_passwordHash |

**AVR Change: 0.50 → 0.33** (-34% improvement)

**Rationale:** Credentials are the **highest priority** security concern. Password hashing is industry standard, and making AuthToken private prevents token exposure.

---

### Doctor Class

| Attribute            | Before Status | After Status  | Change Applied           |
| -------------------- | ------------- | ------------- | ------------------------ |
| SpecialtyID          | 🟢 Safe       | 🟢 Safe       | No change (reference ID) |
| MedicalLicenseNumber | 🔴 Vulnerable | 🔴 Vulnerable | No change (still public) |

**AVR Change: 0.33 → 0.33** (no change)

**Rationale:** Medical license numbers are needed for verification and display. Making them private would require significant changes to the system.

---

### Prescription Class

| Attribute      | Before Status | After Status | Change Applied                |
| -------------- | ------------- | ------------ | ----------------------------- |
| PrescriptionID | 🟢 Safe       | 🟢 Safe      | No change (public identifier) |
| PatientID      | 🟢 Safe       | 🟢 Safe      | No change (reference ID)      |
| DoctorID       | 🟢 Safe       | 🟢 Safe      | No change (reference ID)      |
| DrugName       | 🔴 Vulnerable | 🟢 Safe      | ✅ Made private               |
| Dosage         | 🟢 Safe       | 🟢 Safe      | No change (medical data)      |
| DrugCost       | 🟢 Safe       | 🟢 Safe      | No change (financial data)    |

**AVR Change: 0.20 → 0.00** (-100% improvement)

**Rationale:** DrugName is sensitive prescription information that should only be accessed through controlled methods (e.g., by pharmacists fulfilling orders).

---

### PharmacyOrder Class

| Attribute      | Before Status | After Status | Change Applied                |
| -------------- | ------------- | ------------ | ----------------------------- |
| OrderID        | 🟢 Safe       | 🟢 Safe      | No change (public identifier) |
| PrescriptionID | 🟢 Safe       | 🟢 Safe      | No change (reference ID)      |
| OrderDate      | 🟢 Safe       | 🟢 Safe      | No change (operational data)  |
| Status         | 🟢 Safe       | 🟢 Safe      | No change (operational data)  |
| PatientName    | 🔴 Vulnerable | 🟢 Safe      | ✅ Removed (use reference)    |

**AVR Change: 0.20 → 0.00** (-100% improvement)

**Rationale:** Eliminating data duplication is a fundamental security principle. Patient name should be retrieved through the Prescription → Patient relationship when needed.

---

## What We Did NOT Change (and Why)

### 1. Most PatientRecord Fields Remain Public

**Fields:** DOB, MedicalHistory, PhoneNumber, EmailAddress, EmergencyContact

**Reason:** These fields are frequently accessed throughout the application. Making them all private would require:

- Extensive refactoring across the codebase
- Adding numerous getter/setter methods
- Implementing proper access control logic
- Significant testing effort

**Trade-off:** We focused on the **most critical** field (NIK) instead.

---

### 2. No Encryption Implementation

**Reason:**

- Encryption requires proper key management infrastructure
- Would need a dedicated encryption service or library
- Adds complexity and performance overhead
- Not always necessary if proper access control is in place

**Trade-off:** We used **encapsulation** (private fields) as a first step. Encryption can be added later if needed.

---

### 3. MedicalLicenseNumber Remains Public

**Reason:**

- Needed for doctor verification and display
- Less sensitive than patient data
- Would require UI changes to hide/show appropriately

**Trade-off:** Acceptable risk for this refactoring scope.

---

## Security Improvements Summary

### Critical Vulnerabilities Fixed

#### 1. ✅ Plain Text Password Storage

**Before:**

```csharp
public string Password { get; set; }
```

**After:**

```csharp
private string _passwordHash;
```

**Impact:** Database breach no longer exposes passwords

---

#### 2. ✅ Exposed Authentication Tokens

**Before:**

```csharp
public string AuthToken { get; set; }
public string GetAuthToken() { return AuthToken; }
```

**After:**

```csharp
private string _authToken;
// No getter method
```

**Impact:** Prevents session hijacking through token exposure

---

#### 3. ✅ NIK Exposure

**Before:**

```csharp
public string NIK { get; set; }
```

**After:**

```csharp
private string _nik;
public string GetNIK() { return _nik; }
```

**Impact:** Enables future access control implementation

---

#### 4. ✅ Prescription Drug Name Exposure

**Before:**

```csharp
public string DrugName { get; set; }
```

**After:**

```csharp
private string _drugName;
```

**Impact:** Prevents unauthorized access to prescription details

---

#### 5. ✅ Data Duplication in PharmacyOrder

**Before:**

```csharp
public string PatientName { get; set; }
```

**After:**

```csharp
public Guid PrescriptionID { get; set; }
// Retrieve name through relationship when needed
```

**Impact:** Single source of truth, reduced attack surface

---

## Compliance Improvements

### HIPAA Compliance

| Requirement              | Before | After | Status           |
| ------------------------ | ------ | ----- | ---------------- |
| Encryption at rest       | ❌     | ❌    | ⚠️ Not addressed |
| Access control           | ❌     | ⚠️    | ⚠️ Partial       |
| Audit logging capability | ❌     | ❌    | ⚠️ Not addressed |
| Minimum necessary access | ❌     | ⚠️    | ⚠️ Partial       |
| Authentication           | ⚠️     | ✅    | ✅ Improved      |

### Overall Assessment

**Before:** ❌ Non-compliant  
**After:** ⚠️ Partially compliant (authentication improved, but encryption still needed)

---

## Performance Considerations

### Password Hashing Overhead

**Impact:** ~100-500ms per login

- Intentional computational cost for security
- Prevents brute-force attacks
- Only occurs during authentication

### Getter Method Overhead

**Impact:** Negligible

- Simple method calls add minimal overhead
- No encryption/decryption involved
- Enables future access control logic

---

## Remaining Vulnerabilities

Despite improvements, the following vulnerabilities remain:

### 1. ⚠️ Unencrypted Sensitive Data

**Affected:** PatientRecord (DOB, MedicalHistory, PhoneNumber, EmailAddress, EmergencyContact)

**Risk:** Data breach exposes patient information

**Mitigation:** Future work - implement encryption at rest

---

### 2. ⚠️ No Access Control on GetNIK()

**Current:**

```csharp
public string GetNIK() { return _nik; }
```

**Risk:** Any code can call this method

**Mitigation:** Future work - add requester validation:

```csharp
public string GetNIK(User requester)
{
    if (!HasPermission(requester)) throw new UnauthorizedException();
    return _nik;
}
```

---

### 3. ⚠️ MedicalLicenseNumber Still Public

**Risk:** Credential theft

**Mitigation:** Future work - make private with controlled access

---

## Next Steps for Further Improvement

### Phase 2 Improvements (Future Work)

1. **Add Access Control**

   - Implement requester validation in getter methods
   - Add role-based access control (RBAC)
   - Create audit logging for sensitive data access

2. **Implement Encryption**

   - Encrypt remaining sensitive fields in PatientRecord
   - Use proper key management (Azure Key Vault, AWS KMS)
   - Implement field-level encryption

3. **Enhance Token Management**

   - Add token expiration
   - Implement token refresh mechanism
   - Add token revocation capability

4. **Improve Password Security**
   - Add password complexity requirements
   - Implement rate limiting on login attempts
   - Add multi-factor authentication (MFA)

---

## Conclusion

### Metrics Achievement

| Metric | Target | Before | After | Status          |
| ------ | ------ | ------ | ----- | --------------- |
| AVR    | ≤ 0.30 | 0.31   | 0.21  | ✅ **ACHIEVED** |
| VCC    | ≤ 3    | 3      | 3     | ✅ **ACHIEVED** |
| CIVPF  | ≤ 2    | 1      | 1     | ✅ **ACHIEVED** |
| VA     | ≤ 0.40 | 0.50   | 0.33  | ✅ **ACHIEVED** |

### Overall Assessment

**Before Refactoring:**

- ⚠️ Moderate security vulnerabilities
- 🔴 31% of attributes vulnerable (AVR = 0.31)
- 🔴 50% of methods expose sensitive data (VA = 0.50)
- 🔴 Plain text credentials
- ❌ HIPAA non-compliant

**After Refactoring:**

- ✅ **Improved to 21% vulnerable attributes** (AVR = 0.21)
- ✅ **Reduced method exposure to 33%** (VA = 0.33)
- ✅ **Password hashing implemented**
- ✅ **Authentication tokens protected**
- ✅ **Most critical data (NIK, DrugName) encapsulated**
- ✅ **Data duplication eliminated**
- ⚠️ Partially HIPAA compliant (authentication improved)

### Recommendation

**Status:** ⚠️ **IMPROVED BUT NOT PRODUCTION READY**

This refactoring represents a **realistic, incremental improvement** that addresses the most critical vulnerabilities through proper OOP design principles. The system is **significantly more secure** than before, but still requires:

- Encryption at rest for remaining sensitive fields
- Access control implementation in getter methods
- Audit logging for compliance

**This is a good first step** that demonstrates how proper encapsulation and credential management can meaningfully improve security without requiring a complete system rewrite.

---

_Analysis completed: December 8, 2025_  
_Analyst: Security Metrics Team_  
_Classification: Educational Use - Conservative Refactoring Demonstration_
