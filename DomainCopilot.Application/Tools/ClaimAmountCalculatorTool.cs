using DomainCopilot.Application.Services;

namespace DomainCopilot.Application.Tools;

public class ClaimAmountCalculatorTool
{
    private readonly ClaimAmountCalculator _calculator;

    public ClaimAmountCalculatorTool(
        ClaimAmountCalculator calculator)
    {
        _calculator = calculator;
    }

    public decimal Execute(
        decimal claimedAmount,
        decimal coverageLimit,
        decimal deductible)
    {
        return _calculator.CalculateApprovedAmount(
            claimedAmount,
            coverageLimit,
            deductible);
    }
}