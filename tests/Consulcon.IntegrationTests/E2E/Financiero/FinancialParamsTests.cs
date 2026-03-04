using System.Net.Http.Json;
using Consulcon.Application.DTOs.Financiero;
using System.Net;
using Xunit;

namespace Consulcon.IntegrationTests.E2E.Financiero;

[Collection("E2E Tests")]
public class FinancialParamsTests(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task FinancialConfig_ShouldBeManageable()
    {
        var condominiumId = _fixture.TestCondominioId;

        // 1. Get Initial Config (Should be default 0)
        var response = await _client.GetAsync($"/api/FinancialConfig/penalties/{condominiumId}");
        response.EnsureSuccessStatusCode();
        var config = await response.Content.ReadFromJsonAsync<FinancialConfigDto>();
        
        Assert.NotNull(config);
        Assert.Equal(0, config.GraceDays);
        Assert.Equal(0, config.MonthlyInterestRate);

        // 2. Update Config
        var updateDto = new UpdateFinancialConfigDto
        {
            GraceDays = 5,
            MonthlyInterestRate = 2.5m
        };

        var putResponse = await _client.PutAsJsonAsync($"/api/FinancialConfig/penalties/{condominiumId}", updateDto);
        putResponse.EnsureSuccessStatusCode();

        // 3. Verify Update
        var response2 = await _client.GetAsync($"/api/FinancialConfig/penalties/{condominiumId}");
        var updatedConfig = await response2.Content.ReadFromJsonAsync<FinancialConfigDto>();

        Assert.NotNull(updatedConfig);
        Assert.Equal(5, updatedConfig.GraceDays);
        Assert.Equal(2.5m, updatedConfig.MonthlyInterestRate);
    }

    [Fact]
    public async Task ChargeConcepts_ShouldBeManageable()
    {
        var condominiumId = _fixture.TestCondominioId;

        // 1. Create a Concept
        var createDto = new CreateChargeConceptDto
        {
            Name = "Expensas Test",
            Code = "EXP-01",
            IsRecurrent = true
        };

        var postResponse = await _client.PostAsJsonAsync($"/api/FinancialConfig/concepts/{condominiumId}", createDto);
        postResponse.EnsureSuccessStatusCode();
        
        // Extract ID from response (CreatedAtAction usually returns the object or ID)
        // If ApiController returns { Id = ... } anonymous object
        var resultObj = await postResponse.Content.ReadFromJsonAsync<CreatedResponse>();
        Assert.NotNull(resultObj);
        var createdId = resultObj.Id;

        // 2. Get All Concepts
        var getResponse = await _client.GetAsync($"/api/FinancialConfig/concepts/{condominiumId}");
        getResponse.EnsureSuccessStatusCode();
        var concepts = await getResponse.Content.ReadFromJsonAsync<IEnumerable<ChargeConceptDto>>();

        Assert.NotNull(concepts);
        Assert.Contains(concepts, c => c.Id == createdId && c.Name == "Expensas Test");

        // 3. Update Concept
        var updateDto = new UpdateChargeConceptDto
        {
            Name = "Expensas Updated",
            Code = "EXP-01",
            IsRecurrent = false,
            IsActive = true
        };

        var putResponse = await _client.PutAsJsonAsync($"/api/FinancialConfig/concepts/{createdId}", updateDto);
        putResponse.EnsureSuccessStatusCode();

        // 4. Verify Update
        var getResponse2 = await _client.GetAsync($"/api/FinancialConfig/concepts/{condominiumId}");
        var concepts2 = await getResponse2.Content.ReadFromJsonAsync<IEnumerable<ChargeConceptDto>>();
        
        Assert.NotNull(concepts2);
        Assert.Contains(concepts2, c => c.Id == createdId && c.Name == "Expensas Updated" && !c.IsRecurrent);

        // 5. Delete Concept
        var deleteResponse = await _client.DeleteAsync($"/api/FinancialConfig/concepts/{createdId}");
        deleteResponse.EnsureSuccessStatusCode();

        // 6. Verify Deletion (Soft Delete -> Should not be in list if endpoint filters Active)
        // Our Service.GetChargeConceptsAsync uses IsActive filter.
        var getResponse3 = await _client.GetAsync($"/api/FinancialConfig/concepts/{condominiumId}");
        var concepts3 = await getResponse3.Content.ReadFromJsonAsync<IEnumerable<ChargeConceptDto>>();
        
        Assert.NotNull(concepts3);
        Assert.DoesNotContain(concepts3, c => c.Id == createdId);
    }
    
    private class CreatedResponse
    {
        public int Id { get; set; }
    }
}
