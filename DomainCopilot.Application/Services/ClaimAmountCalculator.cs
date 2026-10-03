namespace DomainCopilot.Application.Services;

public class ClaimAmountCalculator
{
    public decimal CalculateApprovedAmount(
        decimal claimedAmount,
        decimal coverageLimit,
        decimal deductible)
    {
        if (claimedAmount <= 0)
        {
            return 0;
        }

        if (coverageLimit < 0)
        {
            throw new ArgumentException(
                "Coverage limit cannot be negative.");
        }

        if (deductible < 0)
        {
            throw new ArgumentException(
                "Deductible cannot be negative.");
        }

        var amountAfterDeductible = claimedAmount - deductible;

        if (amountAfterDeductible <= 0)
        {
            return 0;
        }

        return Math.Min(
            amountAfterDeductible,
            coverageLimit);
    }
}