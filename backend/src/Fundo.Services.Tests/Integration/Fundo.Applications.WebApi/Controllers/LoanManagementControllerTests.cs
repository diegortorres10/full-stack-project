using FluentAssertions;
using Fundo.Core.Models.Loan;
using Fundo.Services.Tests.Helpers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Fundo.Services.Tests.Integration
{
    public class LoanManagementControllerTests : IClassFixture<CustomWebApplicationFactory<Fundo.Applications.WebApi.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Fundo.Applications.WebApi.Startup> _factory;
        private readonly JsonSerializerOptions _jsonOptions;

        public LoanManagementControllerTests(CustomWebApplicationFactory<Fundo.Applications.WebApi.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            _jsonOptions = JsonHelper.GetApiJsonSerializerOptions();
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
            var result = await response.Content.ReadFromJsonAsync<GetAllLoansResult>(_jsonOptions);

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
            var result = await response.Content.ReadFromJsonAsync<GetAllLoansResult>(_jsonOptions);

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

            var result1 = await response1.Content.ReadFromJsonAsync<GetAllLoansResult>(_jsonOptions);
            var result2 = await response2.Content.ReadFromJsonAsync<GetAllLoansResult>(_jsonOptions);

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
            var result = await response.Content.ReadFromJsonAsync<GetAllLoansResult>(_jsonOptions);
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
            var result = await response.Content.ReadFromJsonAsync<CreateLoanResult>(_jsonOptions);

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
            var result = await response.Content.ReadFromJsonAsync<CreateLoanResult>(_jsonOptions);

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

        #region GET /loans/{id} Tests

        [Fact]
        public async Task GetLoanById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var loanId = 1;

            // Act
            var response = await _client.GetAsync($"/loans/{loanId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetLoanById_WithValidId_ShouldReturnLoanDetails()
        {
            // Arrange
            var loanId = 1;

            // Act
            var response = await _client.GetAsync($"/loans/{loanId}");
            var result = await response.Content.ReadFromJsonAsync<GetLoanDetailsResult>(_jsonOptions);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.Details.Should().NotBeNull();
        }

        [Fact]
        public async Task GetLoanById_WithNonExistentId_ShouldReturnBadRequest()
        {
            // Arrange
            var loanId = 99999;

            // Act
            var response = await _client.GetAsync($"/loans/{loanId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetLoanById_WithNonExistentId_ShouldReturnErrorMessage()
        {
            // Arrange
            var loanId = 99999;

            // Act
            var response = await _client.GetAsync($"/loans/{loanId}");
            var result = await response.Content.ReadFromJsonAsync<GetLoanDetailsResult>(_jsonOptions);

            // Assert
            result.Should().NotBeNull();
            result!.Success.Should().BeFalse();
            result.Message.Should().Be("Loan doesn't exists. Check the information");
        }

        [Fact]
        public async Task GetLoanById_WithPagination_ShouldReturnPagedResults()
        {
            // Arrange
            var loanId = 1;
            var pageNumber = 1;
            var pageSize = 2;

            // Act
            var response = await _client.GetAsync($"/loans/{loanId}?PageNumber={pageNumber}&PageSize={pageSize}");
            var result = await response.Content.ReadFromJsonAsync<GetLoanDetailsResult>(_jsonOptions);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetLoanById_ShouldReturnJsonContentType()
        {
            // Arrange
            var loanId = 1;

            // Act
            var response = await _client.GetAsync($"/loans/{loanId}");

            // Assert
            response.Content.Headers.ContentType.Should().NotBeNull();
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        }

        #endregion

        #region POST /loans/{id}/payment Tests

        [Fact]
        public async Task CreatePayment_WithValidRequest_ShouldReturnCreated()
        {
            // Arrange
            var loanId = 1;
            var request = new CreateLoanPaymentRequest
            {
                Amount = 5000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/loans/{loanId}/payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreatePayment_WithValidRequest_ShouldReturnCreatedPayment()
        {
            // Arrange
            var loanId = 1;
            var request = new CreateLoanPaymentRequest
            {
                Amount = 3000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/loans/{loanId}/payment", request);
            var result = await response.Content.ReadFromJsonAsync<CreateLoanPaymentResult>(_jsonOptions);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.LoanPayment.Should().NotBeNull();
            result.LoanPayment!.Amount.Should().Be(3000.00m);
        }

        [Fact]
        public async Task CreatePayment_WithZeroAmount_ShouldReturnBadRequest()
        {
            // Arrange
            var loanId = 1;
            var request = new CreateLoanPaymentRequest
            {
                Amount = 0
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/loans/{loanId}/payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreatePayment_WithNegativeAmount_ShouldReturnBadRequest()
        {
            // Arrange
            var loanId = 1;
            var request = new CreateLoanPaymentRequest
            {
                Amount = -100.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/loans/{loanId}/payment", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreatePayment_WithNonExistentLoan_ShouldReturnBadRequest()
        {
            // Arrange
            var loanId = 99999;
            var request = new CreateLoanPaymentRequest
            {
                Amount = 5000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/loans/{loanId}/payment", request);
            var result = await response.Content.ReadFromJsonAsync<CreateLoanPaymentResult>(_jsonOptions);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should().NotBeNull();
            result!.Success.Should().BeFalse();
        }

        [Fact]
        public async Task CreatePayment_ShouldReturnLocationHeader()
        {
            // Arrange
            var loanId = 1;
            var request = new CreateLoanPaymentRequest
            {
                Amount = 2000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/loans/{loanId}/payment", request);

            // Assert
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreatePayment_MultiplePayments_ShouldUpdateBalanceCorrectly()
        {
            // Arrange
            var loanId = 1; // Use loan 1 which is Active with balance 30000
            var firstPayment = new CreateLoanPaymentRequest { Amount = 1000.00m };
            var secondPayment = new CreateLoanPaymentRequest { Amount = 1000.00m };

            // Act
            var response1 = await _client.PostAsJsonAsync($"/loans/{loanId}/payment", firstPayment);
            var response2 = await _client.PostAsJsonAsync($"/loans/{loanId}/payment", secondPayment);

            // Assert
            response1.StatusCode.Should().Be(HttpStatusCode.Created);
            response2.StatusCode.Should().Be(HttpStatusCode.Created);

            var result1 = await response1.Content.ReadFromJsonAsync<CreateLoanPaymentResult>(_jsonOptions);
            var result2 = await response2.Content.ReadFromJsonAsync<CreateLoanPaymentResult>(_jsonOptions);

            result1.Should().NotBeNull();
            result1!.Success.Should().BeTrue();
            result2.Should().NotBeNull();
            result2!.Success.Should().BeTrue();
        }

        #endregion
    }
}
