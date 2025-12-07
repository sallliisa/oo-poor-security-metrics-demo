# 🏥 MediLink: Telemedicine & Prescription Management System

> ⚠️ **WARNING: This application is intentionally INSECURE!**
> 
> This project is designed for **educational purposes only** to demonstrate poor software security practices and analyze security metrics (AVR, CIVPF, VCC, VA).
>
> **DO NOT use this code as a template for production systems.**

---

## 📋 Overview

MediLink is a minimal telemedicine application designed to manage digital prescription workflows between patients, doctors, and pharmacies. The system intentionally contains security vulnerabilities for academic study of code quality metrics.

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│                   (PharmacyAdapter.cs)                       │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│                     Business Layer                           │
│              (Prescription.cs, PrescriptionDTO.cs)           │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│                      Data Layer                              │
│              (PatientRecord.cs, Doctor.cs)                   │
└─────────────────────────────────────────────────────────────┘
```

## 📊 Security Metrics

### Metric Definitions

| Metric | Full Name | Description |
|--------|-----------|-------------|
| **AVR** | Attribute Vulnerability Ratio | Ratio of vulnerable attributes to total attributes in a class |
| **CIVPF** | Classified Information Vulnerability Propagation Factor | Measures how sensitive data propagates through class couplings |
| **VCC** | Vulnerable Class Coupling | Number of classes a vulnerable class is coupled with |
| **VA** | Vulnerability Amplification | How method access patterns amplify data exposure risk |

### System Metrics Summary

| Metric | Value | Status |
|--------|-------|--------|
| System AVR | 0.50 (50%) | 🔴 POOR |
| System VCC | 6 vulnerable couplings | 🔴 POOR |
| Max CIVPF Path Length | 3 hops | 🔴 POOR |
| Average Method VA | 0.55 | 🔴 POOR |
| Critical VA Methods (≥0.50) | 6 | 🔴 POOR |

### AVR by Class

| Class | Safe Attrs | Vuln Attrs | Total | AVR |
|-------|------------|------------|-------|-----|
| PatientRecord | 2 | 4 | 6 | **0.67** |
| Prescription | 4 | 2 | 6 | **0.33** |
| Doctor | 3 | 3 | 6 | **0.50** |
| PharmacyAdapter | 2 | 2 | 4 | **0.50** |

## 🔴 Intentional Security Flaws

### Data Protection Failures
- ❌ Plain text SSN storage
- ❌ Plain text password storage
- ❌ Exposed private keys
- ❌ Credit card data in plain text
- ❌ SSN duplication across classes

### Code Quality Failures
- ❌ SQL Injection vulnerability
- ❌ Sensitive data logging
- ❌ No input validation
- ❌ No encryption
- ❌ Hardcoded credentials

### Architecture Failures
- ❌ Data coupling (High CIVPF)
- ❌ God methods (High VA)
- ❌ No separation of concerns (High VCC)
- ❌ No encapsulation (High AVR)

## 📁 Project Structure

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

## 🔍 CIVPF Flow Diagram

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
```

## 📚 Educational Purpose

This project demonstrates:

1. **AVR Analysis**: How to count vulnerable vs total attributes per class
2. **CIVPF Analysis**: How to trace sensitive data flow through class couplings
3. **VCC Analysis**: How to count class coupling relationships
4. **VA Analysis**: How to calculate method-level vulnerability amplification

### Expected Poor Metric Results
- **AVR > 0.30** indicates poor data classification
- **VCC > 3** indicates excessive coupling
- **CIVPF > 2** indicates dangerous data propagation
- **VA > 0.40** indicates methods expose too much sensitive data

---

*Document Version: 1.0 | Created for Security Metrics Analysis*
