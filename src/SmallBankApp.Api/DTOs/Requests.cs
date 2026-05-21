namespace SmallBankApp.Api.DTOs;

public record RegisterRequest(string Email, string PhoneNumber, string Password, string LegalFirstName, string LegalLastName, DateOnly DateOfBirth);
public record LoginRequest(string Email, string Password, string? MfaCode);
public record TransferRequest(Guid FromAccountId, Guid? ToInternalAccountId, Guid? ToExternalAccountId, decimal Amount, string Memo);
public record LinkExternalAccountRequest(string InstitutionName, string RoutingNumber, string AccountNumber);
public record AlertPreferencesRequest(bool PushEnabled, bool SmsEnabled, bool EmailEnabled, decimal LowBalanceThreshold, bool TransactionAlerts, bool SecurityAlerts);
public record EKycRequest(string DocumentType, string DocumentNumber, string IssuingState, string SelfieImageBase64, string DocumentImageBase64);
