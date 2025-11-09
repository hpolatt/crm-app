using CrmApp.Core.DTOs.Reports;
using FluentAssertions;

namespace CrmApp.UnitTests.Mapping;

public class ReportDtoTests
{
    [Fact]
    public void SalesReportDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new SalesReportDto
        {
            TotalRevenue = 500000,
            TotalDeals = 45,
            AverageDealSize = 11111.11m,
            MonthlySales = new List<MonthlySalesDto>(),
            SalesByStage = new List<SalesByStageDto>(),
            SalesByUser = new List<SalesByUserDto>()
        };

        // Assert
        dto.TotalRevenue.Should().Be(500000);
        dto.TotalDeals.Should().Be(45);
        dto.AverageDealSize.Should().Be(11111.11m);
        dto.MonthlySales.Should().NotBeNull();
        dto.SalesByStage.Should().NotBeNull();
        dto.SalesByUser.Should().NotBeNull();
    }

    [Fact]
    public void MonthlySalesDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new MonthlySalesDto
        {
            Year = 2024,
            Month = 3,
            MonthName = "March",
            Revenue = 50000,
            DealCount = 10
        };

        // Assert
        dto.Year.Should().Be(2024);
        dto.Month.Should().Be(3);
        dto.MonthName.Should().Be("March");
        dto.Revenue.Should().Be(50000);
        dto.DealCount.Should().Be(10);
    }

    [Fact]
    public void SalesByStageDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new SalesByStageDto
        {
            Stage = "Proposal",
            Count = 15,
            Value = 150000,
            Percentage = 33.5
        };

        // Assert
        dto.Stage.Should().Be("Proposal");
        dto.Count.Should().Be(15);
        dto.Value.Should().Be(150000);
        dto.Percentage.Should().Be(33.5);
    }

    [Fact]
    public void SalesByUserDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new SalesByUserDto
        {
            UserId = Guid.NewGuid(),
            UserName = "John Sales",
            DealCount = 20,
            Revenue = 200000,
            WonCount = 15,
            LostCount = 5,
            WinRate = 75.0
        };

        // Assert
        dto.UserId.Should().NotBeEmpty();
        dto.UserName.Should().Be("John Sales");
        dto.DealCount.Should().Be(20);
        dto.Revenue.Should().Be(200000);
        dto.WonCount.Should().Be(15);
        dto.LostCount.Should().Be(5);
        dto.WinRate.Should().Be(75.0);
    }

    [Fact]
    public void CustomerReportDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new CustomerReportDto
        {
            TotalCustomers = 250,
            ActiveCustomers = 220,
            NewCustomersThisMonth = 15,
            CustomersBySource = new List<CustomersBySourceDto>(),
            CustomersByIndustry = new List<CustomersByIndustryDto>(),
            CustomerGrowth = new List<CustomerGrowthDto>()
        };

        // Assert
        dto.TotalCustomers.Should().Be(250);
        dto.ActiveCustomers.Should().Be(220);
        dto.NewCustomersThisMonth.Should().Be(15);
        dto.CustomersBySource.Should().NotBeNull();
        dto.CustomersByIndustry.Should().NotBeNull();
        dto.CustomerGrowth.Should().NotBeNull();
    }

    [Fact]
    public void CustomersBySourceDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new CustomersBySourceDto
        {
            Source = "Referral",
            Count = 45,
            Percentage = 18.5
        };

        // Assert
        dto.Source.Should().Be("Referral");
        dto.Count.Should().Be(45);
        dto.Percentage.Should().Be(18.5);
    }

    [Fact]
    public void CustomersByIndustryDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new CustomersByIndustryDto
        {
            Industry = "Technology",
            Count = 80,
            Percentage = 32.0
        };

        // Assert
        dto.Industry.Should().Be("Technology");
        dto.Count.Should().Be(80);
        dto.Percentage.Should().Be(32.0);
    }

    [Fact]
    public void CustomerGrowthDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new CustomerGrowthDto
        {
            Year = 2024,
            Month = 3,
            MonthName = "March",
            NewCustomers = 15,
            TotalCustomers = 250
        };

        // Assert
        dto.Year.Should().Be(2024);
        dto.Month.Should().Be(3);
        dto.MonthName.Should().Be("March");
        dto.NewCustomers.Should().Be(15);
        dto.TotalCustomers.Should().Be(250);
    }
}
