using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PeopleDatabase.Controllers;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Tests.Helpers;

namespace Tests
{
    public class PeopleControllerTests
    {
        private readonly ICountriesService _countriesService;
        private readonly IPeopleService _peopleService;
        private readonly IFixture _fixture;
        private readonly IEnumerable<CountryResponse> _countries;
        private readonly IEnumerable<PersonResponse> _responses;
        private readonly PeopleController _controller;
        private readonly Mock<ILogger<PeopleController>> loggerMock = new Mock<ILogger<PeopleController>>();

        public PeopleControllerTests() 
        {
            _fixture = new Fixture();
            _countries = _fixture.CreateMany<CountryResponse>();
            _responses = _fixture.CreateMany<PersonResponse>();
            _countriesService = TestHelper.CreateMockCountriesService(_countries);
            _peopleService = TestHelper.CreateMockPeopleService(_responses);
            _controller = new PeopleController(_peopleService, _countriesService, loggerMock.Object);
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnViewWithPeopleList() 
        {
            string searchBy = _fixture.Create<string>();
            string searchString = _fixture.Create<string>();
            string sortBy = _fixture.Create<string>();
            SortOrderOptions options = _fixture.Create<SortOrderOptions>();
            IActionResult result = await _controller.Index(searchBy, searchString, sortBy, options);

            result.Should().BeOfType<ViewResult>();

            ViewResult viewResult = (ViewResult)result;

            viewResult.ViewData.Model.Should().BeEquivalentTo(_responses);
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_WithoutError()
        {
            PersonAddRequest request = _fixture.Create<PersonAddRequest>();
            IActionResult result = await _controller.Create(request);

            result.Should().BeOfType<RedirectToActionResult>();

            RedirectToActionResult redirectResult = (RedirectToActionResult)result;

            redirectResult.ActionName.Should().Be(nameof(PeopleController.Index));
        }

        #endregion
    }
}
