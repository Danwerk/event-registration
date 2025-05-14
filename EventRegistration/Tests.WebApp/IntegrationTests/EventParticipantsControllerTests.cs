using App.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace Tests.WebApp.IntegrationTests
{
    public class EventParticipantsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EventParticipantsControllerTests(WebApplicationFactory<Program> factory)
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            _client = factory.CreateClient(options);
        }

        [Fact]
        public async Task Get_Index_ReturnsSuccess_WhenEventExists()
        {
            var eventId = await CreateTestEventAsync();

            var response = await _client.GetAsync($"/EventParticipants?eventId={eventId}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Osav\u00F5tjad");
        }

        [Fact]
        public async Task Post_CreatePrivateParticipant_RedirectsToIndex()
        {
            var eventId = await CreateTestEventAsync();
            var paymentMethodId = await CreateTestPaymentMethodAsync();

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("EventId", eventId.ToString()),
                new KeyValuePair<string, string>("ParticipantType", "private"),
                new KeyValuePair<string, string>("FirstName", "Test"),
                new KeyValuePair<string, string>("LastName", "Person"),
                new KeyValuePair<string, string>("PersonalCode", "12345678901"),
                new KeyValuePair<string, string>("PaymentMethodId", paymentMethodId.ToString())
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/EventParticipants/Create", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location!.ToString().Should().Contain($"/EventParticipants?eventId={eventId}");
        }

        [Fact]
        public async Task Post_CreateLegalParticipant_RedirectsToIndex()
        {
            var eventId = await CreateTestEventAsync();
            var paymentMethodId = await CreateTestPaymentMethodAsync();

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("EventId", eventId.ToString()),
                new KeyValuePair<string, string>("ParticipantType", "legal"),
                new KeyValuePair<string, string>("CompanyName", "Test Company"),
                new KeyValuePair<string, string>("RegistryCode", "12345678"),
                new KeyValuePair<string, string>("NumberOfAttendees", "5"),
                new KeyValuePair<string, string>("PaymentMethodId", paymentMethodId.ToString())
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/EventParticipants/Create", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location!.ToString().Should().Contain($"/EventParticipants?eventId={eventId}");
        }

        [Fact]
        public async Task Post_CreateParticipant_MissingPaymentMethod_ReturnsViewWithError()
        {
            var eventId = await CreateTestEventAsync();
            await CreateTestPaymentMethodAsync(); // <-- Lisa makseviis, et View ei crashiks

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("EventId", eventId.ToString()),
                new KeyValuePair<string, string>("ParticipantType", "private"),
                new KeyValuePair<string, string>("FirstName", "Test"),
                new KeyValuePair<string, string>("LastName", "Person"),
                new KeyValuePair<string, string>("PersonalCode", "12345678901")
            };

            var content = new FormUrlEncodedContent(formData);
            var response = await _client.PostAsync("/EventParticipants/Create", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("Palun vali korrektne makseviis.");
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

        private async Task<Guid> CreateTestPaymentMethodAsync()
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", "Test PaymentMethod")
            };

            var content = new FormUrlEncodedContent(formData);

            var request = new HttpRequestMessage(HttpMethod.Post, "/PaymentMethods/Create")
            {
                Content = content
            };
            request.Headers.Add("Accept", "application/json");

            var response = await _client.SendAsync(request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdPaymentMethod = JsonConvert.DeserializeObject<PaymentMethod>(responseContent);

            return createdPaymentMethod!.Id;
        }


    }
}