# Privacy and Personal Data

This document describes how NuciGenerators.Text at https://github.com/hmlendea/nucigenerators.text handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## 📑 Table of Contents

- [What This Document Covers](#what-this-document-covers)
- [Data We Handle](#data-we-handle)
- [Processing and Use](#processing-and-use)
- [Storage, Retention, and Deletion](#storage-retention-and-deletion)
- [External Processing and Integrations](#external-processing-and-integrations)
- [Document Changes](#document-changes)
- [Contact](#contact)

## 🔎 What This Document Covers

This document describes how NuciGenerators.Text at https://github.com/hmlendea/nucigenerators.text handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## 📥 Data We Handle

### Data Provided to the Application

No personal data is requested by the library. The library operates on word fragments and generation schemas provided by the consuming application.

### Data Generated or Collected by the Application

No personal data is generated or collected automatically by the library. The library produces generated text strings based on input wordlists and validation rules configured by the consuming application.

### Data Received from Integrations

No personal data is received from integrations or third parties. The library has no built-in external integrations.

## 🧭 Processing and Use

The application processes the data described above for these verified functions:
- Text generation — word fragments and schemas provided by the consuming application
- Validation and filtering — candidate strings evaluated against configured rules

## 🗄️ Storage, Retention, and Deletion

The library does not persist any data. Generated strings are returned to the consuming application in memory. The consuming application controls storage, retention, and deletion of any data it provides to or receives from the library.

## 🔗 External Processing and Integrations

The library has no built-in external data transfer, telemetry, update checks, or third-party integrations.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| None | N/A | N/A | N/A |

## 🛡️ Data Protection and Security

The library performs no network operations, file I/O, or persistent storage. It operates entirely in memory on data provided by the consuming application. Security of any personal data processed by applications using this library is the responsibility of those applications.

The original maintainers are not responsible for changes, modifications, or privacy issues introduced by downstream forks.

## 🔄 Document Changes

Update this document when application data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/nucigenerators.text/blob/master/PRIVACY.md.

## 📬 Contact

For questions about application data handling, contact the project maintainers. For a self-hosted instance, contact the instance operator, unless the project explicitly handles the request. Include the repository name and version; do not send passwords, access tokens, or other secrets.