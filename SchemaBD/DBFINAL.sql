CREATE DATABASE BankSaDiDB;
GO

USE BankSaDiDB;
GO

-- USERS
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    NationalID VARCHAR(20) NOT NULL UNIQUE CHECK (LEN(NationalID) >= 8),
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARBINARY(64) NOT NULL,
    UserType VARCHAR(10) NOT NULL CHECK (UserType IN ('Client', 'Admin')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

INSERT INTO Users (NationalID, FirstName, LastName, Email, PasswordHash, UserType, CreatedAt)
VALUES (
    '99999999',                            
    'Admin',                              
    'User',                               
    'admin@banksadi.com',                 
    HASHBYTES('SHA2_256', 'Admin1234'),    
    'Admin',                              
    GETDATE()                            
);

-- ACCOUNT TYPE
CREATE TABLE AccountType (
    TypeID INT IDENTITY(1,1) PRIMARY KEY,
    TypeName VARCHAR(100) NOT NULL UNIQUE
);

-- ACCOUNTS (AQUÍ YA INCLUIMOS NameAccount)
CREATE TABLE Accounts (
    AccountNumber INT PRIMARY KEY,
    UserID INT NOT NULL,
    TypeID INT NOT NULL,
    NameAccount VARCHAR(20) NOT NULL,  -- <- NUEVA COLUMNA
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (TypeID) REFERENCES AccountType(TypeID)
);

-- SAVING ACCOUNT
CREATE TABLE Saving_Account (
    AccountNumber INT NOT NULL PRIMARY KEY,
    InterestRate DECIMAL(5,2) NOT NULL CHECK (InterestRate >= 0),
    FOREIGN KEY (AccountNumber) REFERENCES Accounts(AccountNumber)
);

-- CHECKING ACCOUNT
CREATE TABLE Checking_Account (
    AccountNumber INT NOT NULL PRIMARY KEY,
    OverDraft_Limit DECIMAL(18,2) NOT NULL CHECK (OverDraft_Limit >= 0),
    FOREIGN KEY (AccountNumber) REFERENCES Accounts(AccountNumber)
);

-- TYPE OF MOVEMENT
CREATE TABLE TypeOfMovement (
    TransactionT_ID INT IDENTITY(1,1) PRIMARY KEY,
    TypeTransID VARCHAR(20) NOT NULL UNIQUE
);

-- MOVEMENTS
CREATE TABLE Movements (
    IdTransaction INT IDENTITY(1,1) PRIMARY KEY,
    OriginAccount INT NULL,
    DestinyAccount INT NOT NULL,
    TransactionT_ID INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    TransactionDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (DestinyAccount) REFERENCES Accounts(AccountNumber),
    FOREIGN KEY (OriginAccount) REFERENCES Accounts(AccountNumber),
    FOREIGN KEY (TransactionT_ID) REFERENCES TypeOfMovement(TransactionT_ID)
);

-- CHANGES HISTORY
CREATE TABLE ChangesHistory (
    ChangesID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    ChangeDate DATETIME NOT NULL DEFAULT GETDATE(),
    ChangeType VARCHAR(50) NOT NULL,
    TableAffected VARCHAR(50) NOT NULL,
    RecordID INT,
    Description VARCHAR(255) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);


INSERT INTO AccountType (TypeName)
VALUES ('SavingAccount'), ('CheckingAccount');

INSERT INTO TypeOfMovement (TypeTransID)
VALUES ('Deposit'), ('Withdraw'), ('Transfer');
