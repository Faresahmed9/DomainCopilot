using DomainCopilot.Application.Services;

namespace DomainCopilot.Tests;

public class ClaimAmountCalculatorTests
{
    [Fact]
    public void CalculateApprovedAmount_ShouldApplyDeductibleAndLimit()
    {
        // Arrange
        var calculator = new ClaimAmountCalculator();

        // Act
        var result = calculator.CalculateApprovedAmount(
            claimedAmount: 10000,
            coverageLimit: 7000,
            deductible: 1000);

        // Assert
        Assert.Equal(7000, result);
    }
}