using DomainCopilot.Application.Services;
using DomainCopilot.Application.Tools;

namespace DomainCopilot.Tests;

public class DeterministicCalculationEvaluationTests
{
    [Fact]
    public void GoldenQuestion24_ShouldCalculateApprovedAmountDeterministically()
    {
        var calculator = new ClaimAmountCalculator();

        var result = calculator.CalculateApprovedAmount(
            claimedAmount: 10000,
            coverageLimit: 7000,
            deductible: 1000);

        Assert.Equal(7000, result);
    }

    [Fact]
    public void ClaimAmountCalculatorTool_ShouldReturnDeterministicResult()
    {
        var calculator = new ClaimAmountCalculator();

        var tool =
            new ClaimAmountCalculatorTool(calculator);

        var result = tool.Execute(
            claimedAmount: 10000,
            coverageLimit: 7000,
            deductible: 1000);

        Assert.Equal(7000, result);
    }

    [Fact]
    public void ApprovedAmount_ShouldNeverExceedCoverageLimit()
    {
        var calculator = new ClaimAmountCalculator();

        var result = calculator.CalculateApprovedAmount(
            claimedAmount: 50000,
            coverageLimit: 7000,
            deductible: 1000);

        Assert.True(result <= 7000);
    }

    [Fact]
    public void Deductible_ShouldBeAppliedBeforeCoverageLimit()
    {
        var calculator = new ClaimAmountCalculator();

        var result = calculator.CalculateApprovedAmount(
            claimedAmount: 5000,
            coverageLimit: 7000,
            deductible: 1000);

        Assert.Equal(4000, result);
    }
}