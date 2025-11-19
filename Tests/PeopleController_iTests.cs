using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Tests.Helpers;

namespace Tests
{
    public class PeopleController_iTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PeopleController_iTests(CustomWebApplicationFactory factory) 
        { 
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Index_ReturnView() 
        {
            HttpResponseMessage response = await _client.GetAsync("/People/Index");

            response.IsSuccessStatusCode.Should().BeTrue();

            string body = await response.Content.ReadAsStringAsync();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(body);
            HtmlNode document = htmlDocument.DocumentNode;
            IEnumerable<HtmlNode> peopleTableElements = document.QuerySelectorAll("table.people");
            
            peopleTableElements.Should().NotBeEmpty();
            peopleTableElements.Should().HaveCount(1);
        }
    }
}
