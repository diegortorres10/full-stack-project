using FluentAssertions;
using Fundo.Core.Models.Loan;
using Fundo.Services.Tests.Helpers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public class LoanManagementControllerTests : IClassFixture<CustomWebApplicationFactory<Fundo.Applications.WebApi.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Fundo.Applications.WebApi.Startup> _factory;

        public LoanManagementControllerTests(CustomWebApplicationFactory<Fundo.Applications.WebApi.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAllLoans_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/loans");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllLoans_ShouldReturnSuccessResult()
        {
            // Act
            var response = await _client.GetAsync("/loans");
            var result = await response.Content.ReadFromJsonAsync<GetAllLoansResult>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllLoans_ShouldReturnExpectedMessage()
        {
            // Act
            var response = await _client.GetAsync("/loans");
            var result = await response.Content.ReadFromJsonAsync<GetAllLoansResult>();

            // Assert
            result.Should().NotBeNull();
            result!.Message.Should().Be("Loans retrieved successfully");
        }

        [Fact]
        public async Task GetAllLoans_ShouldReturnJsonContentType()
        {
            // Act
            var response = await _client.GetAsync("/loans");

            // Assert
            response.Content.Headers.ContentType.Should().NotBeNull();
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }

        [Fact]
        public async Task GetAllLoans_MultipleRequests_ShouldReturnConsistentResults()
        {
            // Act
            var response1 = await _client.GetAsync("/loans");
            var response2 = await _client.GetAsync("/loans");

            var result1 = await response1.Content.ReadFromJsonAsync<GetAllLoansResult>();
            var result2 = await response2.Content.ReadFromJsonAsync<GetAllLoansResult>();

            // Assert
            response1.StatusCode.Should().Be(HttpStatusCode.OK);
            response2.StatusCode.Should().Be(HttpStatusCode.OK);
            result1.Should().NotBeNull();
            result2.Should().NotBeNull();
            result1!.Success.Should().Be(result2!.Success);
        }

        [Fact]
        public async Task GetAllLoans_ShouldHandleRequestWithoutErrors()
        {
            // Act
            var act = async () => await _client.GetAsync("/loans");

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllLoans_ResponseContent_ShouldBeDeserializable()
        {
            // Act
            var response = await _client.GetAsync("/loans");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            content.Should().NotBeNullOrEmpty();
            var result = await response.Content.ReadFromJsonAsync<GetAllLoansResult>();
            result.Should().NotBeNull();
        }

        #region POST Tests

        [Fact]
        public async Task CreateLoan_WithValidRequest_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreateLoanRequest
            {
                ApplicantName = "New Borrower",
                Amount = 60000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/loans", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateLoan_WithValidRequest_ShouldReturnCreatedLoan()
        {
            // Arrange
            var request = new CreateLoanRequest
            {
                ApplicantName = "John Smith",
                Amount = 45000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/loans", request);
            var result = await response.Content.ReadFromJsonAsync<CreateLoanResult>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.Loan.Should().NotBeNull();
            result.Loan!.AppllicantName.Should().Be("John Smith");
            result.Loan.Amount.Should().Be(45000.00m);
            result.Loan.CurrentBalance.Should().Be(45000.00m);
        }

        [Fact]
        public async Task CreateLoan_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateLoanRequest
            {
                ApplicantName = "",
                Amount = 0
            };

            // Act
            var response = await _client.PostAsJsonAsync("/loans", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateLoan_WithZeroAmount_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateLoanRequest
            {
                ApplicantName = "Test User",
                Amount = 0
            };

            // Act
            var response = await _client.PostAsJsonAsync("/loans", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateLoan_WithEmptyName_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateLoanRequest
            {
                ApplicantName = "",
                Amount = 50000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/loans", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateLoan_ShouldSetInitialBalanceEqualToAmount()
        {
            // Arrange
            var request = new CreateLoanRequest
            {
                ApplicantName = "Balance Test User",
                Amount = 80000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/loans", request);
            var result = await response.Content.ReadFromJsonAsync<CreateLoanResult>();

            // Assert
            result.Should().NotBeNull();
            result!.Loan.Should().NotBeNull();
            result.Loan!.CurrentBalance.Should().Be(result.Loan.Amount);
        }

        [Fact]
        public async Task CreateLoan_ShouldReturnLocationHeader()
        {
            // Arrange
            var request = new CreateLoanRequest
            {
                ApplicantName = "Location Test User",
                Amount = 55000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/loans", request);

            // Assert
            response.Headers.Location.Should().NotBeNull();
        }

        #endregion
    }
}
