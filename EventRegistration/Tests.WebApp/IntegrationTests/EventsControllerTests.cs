using App.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Tests.WebApp.IntegrationTests
{
    public class EventsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EventsControllerTests(WebApplicationFactory<Program> factory)
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            _client = factory.CreateClient(options);
        }

        [Fact]
        public async Task Get_Create_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/Events/Create");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Ürituse lisamine");
        }

        [Fact]
        public async Task Post_Create_RedirectsToHomeIndex()
        {
            var formData = GetValidEventFormData();

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/Events/Create", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location!.ToString().Should().Be("/");

        }

        [Fact]
        public async Task Get_Edit_ReturnsSuccess_WhenEventExists()
        {
            var eventId = await CreateTestEventAsync();

            var response = await _client.GetAsync($"/Events/Edit/{eventId}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Ürituse muutmine");
        }

        [Fact]
        public async Task Post_Edit_RedirectsToHomeIndex()
        {
            var eventId = await CreateTestEventAsync();

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Id", eventId.ToString()),
                new KeyValuePair<string, string>("Name", "Updated Event"),
                new KeyValuePair<string, string>("DateTime", DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-ddTHH:mm")),
                new KeyValuePair<string, string>("Location", "Tartu"),
                new KeyValuePair<string, string>("AdditionalInfo", "Updated Info")
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync($"/Events/Edit/{eventId}", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location!.ToString().Should().Be("/");

        }

        [Fact]
        public async Task Get_Delete_ReturnsSuccess_WhenEventExists()
        {
            var eventId = await CreateTestEventAsync();

            var response = await _client.GetAsync($"/Events/Delete/{eventId}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Kas oled kindel, et soovid selle ürituse kustutada?");
        }

        [Fact]
        public async Task Post_DeleteConfirmed_DeletesEvent_AndRedirects()
        {
            var eventId = await CreateTestEventAsync();

            var response = await _client.PostAsync($"/Events/Delete/{eventId}", new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()));

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location!.ToString().Should().Be("/");

        }

        private async Task<Guid> CreateTestEventAsync()
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", "Test Event"),
                new KeyValuePair<string, string>("DateTime", DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm")),
                new KeyValuePair<string, string>("Location", "Tallinn"),
                new KeyValuePair<string, string>("AdditionalInfo", "Test Info")
            };

            var content = new FormUrlEncodedContent(formData);
    
            var request = new HttpRequestMessage(HttpMethod.Post, "/Events/Create")
            {
                Content = content
            };
            request.Headers.Add("Accept", "application/json");

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdEvent = JsonConvert.DeserializeObject<Event>(responseContent);

            return createdEvent!.Id;
        }


        private List<KeyValuePair<string, string>> GetValidEventFormData()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", "Temporary Event"),
                new KeyValuePair<string, string>("DateTime", DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-ddTHH:mm")),
                new KeyValuePair<string, string>("Location", "Narva"),
                new KeyValuePair<string, string>("AdditionalInfo", "Temporary Info")
            };
        }
        [Fact]
        public async Task Get_Edit_ReturnsNotFound_WhenEventDoesNotExist()
        {
            var nonExistingId = Guid.NewGuid();

            var response = await _client.GetAsync($"/Events/Edit/{nonExistingId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_Delete_ReturnsNotFound_WhenEventDoesNotExist()
        {
            var nonExistingId = Guid.NewGuid();

            var response = await _client.GetAsync($"/Events/Delete/{nonExistingId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        
        [Fact]
        public async Task Post_Create_InvalidModel_ReturnsViewWithValidationErrors()
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", ""), // <<<< Tühi nimi
                new KeyValuePair<string, string>("DateTime", DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm")),
                new KeyValuePair<string, string>("Location", "Tallinn"),
                new KeyValuePair<string, string>("AdditionalInfo", "Test Info")
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/Events/Create", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK); // Tagasi samale vormile
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Ürituse lisamine"); // Kontrollime, et jääme samale lehele
            responseBody.Should().Contain("Name"); 
        }
        
        [Fact]
        public async Task Post_Create_InvalidDate_ReturnsViewWithValidationError()
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", "Test Event"),
                new KeyValuePair<string, string>("DateTime", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm")), // <<< MINEVIKUS
                new KeyValuePair<string, string>("Location", "Tallinn"),
                new KeyValuePair<string, string>("AdditionalInfo", "Test Info")
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _client.PostAsync("/Events/Create", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Ürituse lisamine"); // Vorm jääb ette
            responseBody.Should().Contain("Toimumisaeg peab olema tulevikus"); // Kontrollime veateadet
        }


    }
    
}
