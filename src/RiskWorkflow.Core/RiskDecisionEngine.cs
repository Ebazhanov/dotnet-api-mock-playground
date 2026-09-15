namespace RiskWorkflow.Core;

public enum RiskDecision
{
    Approved,
    ManualReview,
    Rejected
}

public class RiskRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int CreditScore { get; set; }
    public bool IsExistingCustomer { get; set; }
}

public class RiskDecisionEngine
{
    public RiskDecision EvaluateRisk(RiskRequest request)
    {
        // ISSUE 1 (PR Review Candidate): Missing null check for the request object (can throw NullReferenceException)
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // ISSUE 2 (PR Review Candidate): Invalid business validation check
        if (request.Amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(request.Amount));
        }

        // ISSUE 3 (PR Review Candidate): Off-by-one / boundary error (Credit score 600 should pass, but '<=' rejects it)
        if (request.CreditScore <= 600) 
        {
            return RiskDecision.Rejected;
        }

        // ISSUE 4 (PR Review Candidate): Hardcoded threshold and missing edge-case coverage for existing customers
        if (request.Amount > 10000m && !request.IsExistingCustomer)
        {
            return RiskDecision.ManualReview;
        }

        return RiskDecision.Approved;
    }
}