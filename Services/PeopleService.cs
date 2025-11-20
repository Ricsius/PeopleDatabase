using CsvHelper;
using CsvHelper.Configuration;
using Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.Globalization;
using System.Linq.Expressions;

namespace Services
{
    public class PeopleService : IPeopleService
    {
        private readonly IPeopleRepository _repository;
        private readonly ILogger<PeopleService> _logger;

        public PeopleService(IPeopleRepository repository, ILogger<PeopleService> logger)
        {
            _repository = repository;
            _logger = logger;

            ExcelPackage.License.SetNonCommercialPersonal("Placeholder");
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
        {
            _logger.LogInformation($"{nameof(AddPerson)} of {nameof(PeopleService)} called");

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ModelValidation(request);

            Person person = request.ToPerson();
            person.Id = Guid.NewGuid();

            //_database.Sp_InsertPerson(person);
            await _repository.AddPerson(person);

            PersonResponse response = person.ToPersonResponse();

            return response;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? request)
        {
            _logger.LogInformation($"{nameof(UpdatePerson)} of {nameof(PeopleService)} called");

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ModelValidation(request);

            Person? person = await _repository.GetPersonById(request.PersonId);

            if (person == null)
            {
                throw new ArgumentException("Given person ID doesn't exist");
            }

            person.Name = request.Name;
            person.Email = request.Email;
            person.DateOfBirth = request.DateOfBirth;
            person.Gender = request.Gender.ToString();
            person.CountryId = request.CountryId;
            person.Address = request.Address;
            person.ReceiveNewsLetters = request.ReceiveNewsLetters;

            Person? updatedPerson = await _repository.UpdatePerson(person);

            return updatedPerson!.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? id)
        {
            _logger.LogInformation($"{nameof(DeletePerson)} of {nameof(PeopleService)} called");

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _repository.DeletePersonById(id.Value);
        }

        public async Task<IEnumerable<PersonResponse>> GetAllPersons()
        {
            _logger.LogInformation($"{nameof(GetAllPersons)} of {nameof(PeopleService)} called");

            //Person[] people = _database.Sp_GetAllPeople();
            IEnumerable<Person> people = await _repository.GetAllPersons();

            return people.Select(p => p.ToPersonResponse());
        }

        public async Task<PersonResponse?> GetPersonById(Guid? id)
        {
            _logger.LogInformation($"{nameof(GetPersonById)} of {nameof(PeopleService)} called");

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Person? person = await _repository.GetPersonById(id.Value);

            return person?.ToPersonResponse();
        }

        public async Task<IEnumerable<PersonResponse>> SearchPeople(string? searchBy, string? searchString)
        {
            _logger.LogInformation($"{nameof(SearchPeople)} of {nameof(PeopleService)} called");

            Expression<Func<Person, bool>> predicate;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                predicate = p => true;
            }
            else
            {
                if (searchBy == nameof(PersonResponse.DateOfBirth)) 
                {
                    IEnumerable<Person> people = (await _repository.GetAllPersons()).ToArray();
                    
                    return people
                        .Where(p => p.DateOfBirth.ToString("yyyy MM dd").Contains(searchString))
                        .Select(p => p.ToPersonResponse());
                }

                predicate = searchBy switch
                {
                    nameof(PersonResponse.Name) =>
                    p => !string.IsNullOrEmpty(p.Name)
                            ? p.Name.Contains(searchString!)
                            : false,

                    nameof(PersonResponse.Email) =>
                    p => !string.IsNullOrEmpty(p.Email)
                        ? p.Email.Contains(searchString!)
                        : false,

                    nameof(PersonResponse.Gender) =>
                        p => !string.IsNullOrEmpty(p.Gender)
                            ? p.Gender.Equals(searchString!)
                            : false,

                    nameof(PersonResponse.CountryName) =>
                        p => !string.IsNullOrEmpty(p.Country!.Name)
                            ? p.Country.Name.Contains(searchString!)
                            : false,

                    nameof(PersonResponse.Address) =>
                        p => !string.IsNullOrEmpty(p.Address)
                            ? p.Address.Contains(searchString!)
                            : false,

                    _ =>
                    p => true,
                };
            }

            IEnumerable<Person> foundPeople = await _repository.SearchPeople(predicate);
            IEnumerable<PersonResponse> matchingPeople = foundPeople
                    .Select(p => p.ToPersonResponse());

            return matchingPeople;
        }

        public async Task<IEnumerable<PersonResponse>> GetSortedPeople(IEnumerable<PersonResponse> people, string sortBy, SortOrderOptions sortOrder)
        {
            _logger.LogInformation($"{nameof(GetSortedPeople)} of {nameof(PeopleService)} called");

            IEnumerable<PersonResponse> sortedPeople;

            if (string.IsNullOrEmpty(sortBy))
            {
                return people;
            }

            switch (sortBy)
            {
                case nameof(PersonResponse.Name):
                    sortedPeople = people.OrderBy(p => p.Name);
                    break;

                case nameof(PersonResponse.Email):
                    sortedPeople = people.OrderBy(p => p.Email);
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    sortedPeople = people.OrderBy(p => p.DateOfBirth);
                    break;

                case nameof(PersonResponse.Gender):
                    sortedPeople = people.OrderBy(p => p.Gender); ;
                    break;

                case nameof(PersonResponse.CountryName):
                    sortedPeople = people.OrderBy(p => p.CountryName);
                    break;

                case nameof(PersonResponse.Address):
                    sortedPeople = people.OrderBy(p => p.Address);
                    break;

                default:
                    sortedPeople = people;
                    break;
            }

            if (sortOrder == SortOrderOptions.Descending)
            {
                sortedPeople = sortedPeople.Reverse();
            }
            ;
            return await Task.FromResult(sortedPeople);
        }

        public async Task<MemoryStream> GetPeopleCsv()
        {
            _logger.LogInformation($"{nameof(GetPeopleCsv)} of {nameof(PeopleService)} called");

            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            IEnumerable<PersonResponse> people = await GetAllPersons();
            MemoryStream stream = new MemoryStream();
            CsvConfiguration csvConfiguration = new CsvConfiguration(cultureInfo);

            using (StreamWriter streamWriter = new StreamWriter(stream, leaveOpen: true))
            using (CsvWriter csvWriter = new CsvWriter(streamWriter, configuration: csvConfiguration, leaveOpen: true))
            {
                csvWriter.WriteField(nameof(PersonResponse.Name));
                csvWriter.WriteField(nameof(PersonResponse.Email));
                csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
                csvWriter.WriteField(nameof(PersonResponse.Age));
                csvWriter.WriteField(nameof(PersonResponse.Gender));
                csvWriter.WriteField(nameof(PersonResponse.CountryName));
                csvWriter.WriteField(nameof(PersonResponse.Address));
                csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
                csvWriter.NextRecord();
                csvWriter.Flush();

                foreach (PersonResponse person in people)
                {
                    csvWriter.WriteField(person.Name);
                    csvWriter.WriteField(person.Email);
                    csvWriter.WriteField(person.DateOfBirth.ToString("yyyy-MM-dd"));
                    csvWriter.WriteField(person.Age);
                    csvWriter.WriteField(person.Gender);
                    csvWriter.WriteField(person.CountryName);
                    csvWriter.WriteField(person.Address);
                    csvWriter.WriteField(person.ReceiveNewsLetters);
                    csvWriter.NextRecord();
                    csvWriter.Flush();
                }
            }

            stream.Position = 0;

            return stream;
        }

        public async Task<MemoryStream> GetPeopleExcel()
        {
            _logger.LogInformation($"{nameof(GetPeopleExcel)} of {nameof(PeopleService)} called");

            MemoryStream stream = new MemoryStream();

            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PeopleSheet");

                workSheet.Cells["A1"].Value = nameof(PersonResponse.Name);
                workSheet.Cells["B1"].Value = nameof(PersonResponse.Email);
                workSheet.Cells["C1"].Value = nameof(PersonResponse.DateOfBirth);
                workSheet.Cells["D1"].Value = nameof(PersonResponse.Age);
                workSheet.Cells["E1"].Value = nameof(PersonResponse.Gender);
                workSheet.Cells["F1"].Value = nameof(PersonResponse.CountryName);
                workSheet.Cells["G1"].Value = nameof(PersonResponse.Address);
                workSheet.Cells["H1"].Value = nameof(PersonResponse.ReceiveNewsLetters);

                using (ExcelRange headerCells = workSheet.Cells["A1:H1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;
                }

                int row = 2;
                IEnumerable<PersonResponse> people = await GetAllPersons();

                foreach (PersonResponse person in people)
                {
                    workSheet.Cells[row, 1].Value = person.Name;
                    workSheet.Cells[row, 2].Value = person.Email;
                    workSheet.Cells[row, 3].Value = person.DateOfBirth.ToString("yyyy-MM-dd");
                    workSheet.Cells[row, 4].Value = person.Age;
                    workSheet.Cells[row, 5].Value = person.Gender;
                    workSheet.Cells[row, 6].Value = person.CountryName;
                    workSheet.Cells[row, 7].Value = person.Address;
                    workSheet.Cells[row, 8].Value = person.ReceiveNewsLetters;

                    row++;
                }

                workSheet.Cells[$"A1:H{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            stream.Position = 0;

            return stream;
        }
    }
}
