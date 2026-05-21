CREATE DATABASE SmallBankAppDb;
GO
USE SmallBankAppDb;
GO
CREATE TABLE BankAccounts (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    AccountNumberMasked NVARCHAR(20) NOT NULL,
    RoutingNumber NVARCHAR(9) NOT NULL,
    AccountType INT NOT NULL,
    CurrentBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
    AvailableBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
    Currency CHAR(3) NOT NULL DEFAULT 'USD',
    IsFrozen BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);
GO
CREATE TABLE Transactions (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    AccountId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES BankAccounts(Id),
    Type INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Currency CHAR(3) NOT NULL DEFAULT 'USD',
    Description NVARCHAR(250) NULL,
    MerchantCategoryCode NVARCHAR(4) NULL,
    Status INT NOT NULL,
    PostedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    ExternalReference NVARCHAR(60) NULL
);
GO
CREATE TABLE AmlCases (
    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NULL,
    TransactionId UNIQUEIDENTIFIER NULL,
    RuleCode NVARCHAR(50) NOT NULL,
    RiskLevel NVARCHAR(20) NOT NULL,
    Reason NVARCHAR(500) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Open',
    CreatedAt DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET()
);
GO
CREATE INDEX IX_Transactions_Account_PostedAt ON Transactions(AccountId, PostedAt DESC);
GO
CREATE INDEX IX_AmlCases_Status_Risk ON AmlCases(Status, RiskLevel, CreatedAt DESC);
GO
CREATE OR ALTER PROCEDURE dbo.PostTransfer
    @FromAccountId UNIQUEIDENTIFIER,
    @ToAccountId UNIQUEIDENTIFIER,
    @Amount DECIMAL(18,2),
    @Memo NVARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
      UPDATE BankAccounts SET AvailableBalance = AvailableBalance - @Amount, CurrentBalance = CurrentBalance - @Amount WHERE Id = @FromAccountId AND AvailableBalance >= @Amount AND IsFrozen = 0;
      IF @@ROWCOUNT = 0 THROW 50001, 'Insufficient funds or account unavailable.', 1;
      UPDATE BankAccounts SET AvailableBalance = AvailableBalance + @Amount, CurrentBalance = CurrentBalance + @Amount WHERE Id = @ToAccountId;
      INSERT INTO Transactions(AccountId, Type, Amount, Description, Status) VALUES (@FromAccountId, 2, @Amount, @Memo, 1), (@ToAccountId, 3, @Amount, @Memo, 1);
    COMMIT TRANSACTION;
END
GO
