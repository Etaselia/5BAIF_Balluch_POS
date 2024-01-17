CREATE TABLE Addresses (
    AddressId VARCHAR(36) NOT NULL,
    Street VARCHAR(255) NOT NULL,
    City VARCHAR(255) NOT NULL,
    State VARCHAR(255) NOT NULL,
    ZipCode VARCHAR(255) NOT NULL,
    PRIMARY KEY (AddressId)
);

CREATE TABLE Moneys (
    MoneyId VARCHAR(36) NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    Currency VARCHAR(255) NOT NULL,
    PRIMARY KEY (MoneyId)
);

CREATE TABLE Names (
    NameId VARCHAR(36) NOT NULL,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    PRIMARY KEY (NameId)
);

CREATE TABLE Clinicians (
    ClinicianId VARCHAR(36) NOT NULL,
    NameId VARCHAR(36) NOT NULL,
    Specialty VARCHAR(255) NOT NULL,
    PRIMARY KEY (ClinicianId),
    FOREIGN KEY (NameId) REFERENCES Names (NameId) ON DELETE CASCADE
);

CREATE TABLE Patients (
    PatientId VARCHAR(36) NOT NULL,
    NameId VARCHAR(36) NOT NULL,
    AddressId VARCHAR(36) NOT NULL,
    DateOfBirth DATE NOT NULL,
    PhoneNumber VARCHAR(255) NOT NULL,
    PRIMARY KEY (PatientId),
    FOREIGN KEY (NameId) REFERENCES Names (NameId) ON DELETE CASCADE,
    FOREIGN KEY (AddressId) REFERENCES Addresses (AddressId) ON DELETE CASCADE
);

CREATE TABLE Appointments (
    AppointmentId VARCHAR(36) NOT NULL,
    AppointmentDate DATETIME NOT NULL,
    ReasonForVisit TEXT NOT NULL,
    ClinicianId VARCHAR(36) NOT NULL,
    PatientId VARCHAR(36) NOT NULL,
    PRIMARY KEY (AppointmentId),
    FOREIGN KEY (ClinicianId) REFERENCES Clinicians (ClinicianId) ON DELETE CASCADE,
    FOREIGN KEY (PatientId) REFERENCES Patients (PatientId) ON DELETE CASCADE
);

CREATE TABLE MedicalRecords (
    MedicalRecordId VARCHAR(36) NOT NULL,
    PatientId VARCHAR(36) NOT NULL,
    PRIMARY KEY (MedicalRecordId),
    FOREIGN KEY (PatientId) REFERENCES Patients (PatientId) ON DELETE CASCADE
);

CREATE TABLE Treatments (
    TreatmentId VARCHAR(36) NOT NULL,
    Description TEXT NOT NULL,
    DateAdministered DATETIME NOT NULL,
    ClinicianId VARCHAR(36) NOT NULL,
    PatientId VARCHAR(36) NOT NULL,
    CostMoneyId VARCHAR(36) NOT NULL,
    MedicalRecordId VARCHAR(36) NOT NULL,
    PRIMARY KEY (TreatmentId),
    FOREIGN KEY (ClinicianId) REFERENCES Clinicians (ClinicianId) ON DELETE CASCADE,
    FOREIGN KEY (PatientId) REFERENCES Patients (PatientId) ON DELETE CASCADE,
    FOREIGN KEY (CostMoneyId) REFERENCES Moneys (MoneyId) ON DELETE CASCADE,
    FOREIGN KEY (MedicalRecordId) REFERENCES MedicalRecords (MedicalRecordId) ON DELETE CASCADE
);

CREATE TABLE Invoices (
    InvoiceId VARCHAR(36) NOT NULL,
    PatientId VARCHAR(36) NOT NULL,
    DateIssued DATETIME NOT NULL,
    TotalAmountMoneyId VARCHAR(36) NOT NULL,
    TreatmentId VARCHAR(36) NOT NULL,
    PRIMARY KEY (InvoiceId),
    FOREIGN KEY (PatientId) REFERENCES Patients (PatientId) ON DELETE CASCADE,
    FOREIGN KEY (TotalAmountMoneyId) REFERENCES Moneys (MoneyId) ON DELETE CASCADE,
    FOREIGN KEY (TreatmentId) REFERENCES Treatments (TreatmentId) ON DELETE CASCADE
);
