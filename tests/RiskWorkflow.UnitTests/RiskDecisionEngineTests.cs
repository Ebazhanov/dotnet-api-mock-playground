using System;
using Allure.Net.Commons.Attributes;
using RiskWorkflow.Core;
using Shouldly;
using Xunit;

namespace RiskWorkflow.UnitTests;

[AllureSuite("Unit Tests")]
[AllureFeature("Business logic")]
public class RiskDecisionEngineTests
{
    private readonly RiskDecisionEngine _engine = new();

    [Fact]
    public void EvaluateRisk_ShouldApprove_WhenValidExistingCustomer()
    {
        // Arrange
        var request = new RiskRequest
        {
            CustomerId = "CUST123",
            Amount = 5000m,
            CreditScore = 750,
            IsExistingCustomer = true
        };

        // Act
        var result = _engine.EvaluateRisk(request);

        // Assert
        result.ShouldBe(RiskDecision.Approved);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void EvaluateRisk_ShouldThrowArgumentException_WhenAmountIsZeroOrNegative(decimal invalidAmount)
    {
        // Arrange
        var request = new RiskRequest { Amount = invalidAmount, CreditScore = 700 };

        // Act & Assert
        Should.Throw<ArgumentException>(() => _engine.EvaluateRisk(request));
    }

    [Fact]
    public void EvaluateRisk_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _engine.EvaluateRisk(null!));
    }

    [Fact]
    public void EvaluateRisk_ShouldRequireManualReview_WhenAmountExceedsThresholdForNewCustomer()
    {
        // Arrange
        var request = new RiskRequest
        {
            CustomerId = "CUST999",
            Amount = 15000m,
            CreditScore = 720,
            IsExistingCustomer = false
        };

        // Act
        var result = _engine.EvaluateRisk(request);

        // Assert
        result.ShouldBe(RiskDecision.ManualReview);
    }
}