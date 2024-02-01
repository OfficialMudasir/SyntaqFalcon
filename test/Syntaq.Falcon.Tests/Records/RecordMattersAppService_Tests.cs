using Abp.Application.Services.Dto;
using Shouldly;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Syntaq.Falcon.Tests.Records
{
    public class RecordMattersAppService_Tests : AppTestBase
    {
        private readonly IRecordMattersAppService _recordMattersAppService;

        public RecordMattersAppService_Tests()
        {
            _recordMattersAppService = Resolve<IRecordMattersAppService>();
        }

        [Fact]
        public async Task Should_Get_All_Record_Matters_1()
        {
            //Arrange
            var exitingRecord = UsingDbContext(context => context.Records.FirstOrDefault(a => a.RecordName == "Record To Update"));
            GetAllRecordMattersInput input = new GetAllRecordMattersInput()
            {
                Id = exitingRecord.Id
            };

            //Act
            var output = await _recordMattersAppService.GetAll(input);

            //Assert
            output.Items.Count.ShouldBe(1);
        }

        [Fact]
        public void Should_Get_All_Record_Matters_2()
        {
            //Arrange
            var exitingRecord = UsingDbContext(context => context.Records.FirstOrDefault(a => a.RecordName == "Record To Update"));
            GetAllRecordMattersInput input = new GetAllRecordMattersInput()
            {
                Id = exitingRecord.Id
            };

            //Act
            var output = _recordMattersAppService.GetAllByRecord(input);

            //Assert
            output.Items.Count.ShouldBe(1);
        }

        //[Fact]
        //public async Task Should_Get_Record_Matter_For_Edit()
        //{
        //    //Arrange
        //    var exitingRecordMatter = UsingDbContext(context => context.RecordMatters.FirstOrDefault(a => a.RecordMatterName == "Record Matter To Update"));

        //    //Act
        //    var output = await _recordMattersAppService.GetRecordMatterForEdit(new EntityDto<Guid>(Guid.Parse(exitingRecordMatter.Id.ToString())));

        //    //Assert
        //    output.RecordMatter.RecordMatterName.ShouldBe("Record Matter To Update");
        //    output.RecordMatter.ShouldNotBeNull();
        //}

        [Fact]
        public async Task Should_Create_Record_Matter()
        {
            //Arrange
            var exitingRecord = UsingDbContext(context => context.Records.FirstOrDefault(a => a.RecordName == "Record To Update"));
            //var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));
            CreateOrEditRecordMatterDto recordMatter = new CreateOrEditRecordMatterDto()
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                RecordMatterName = "Unit Test Record Matter",
                Comments = "Record Matter for Unit Testing",
                RecordId = exitingRecord.Id,
                Data = "{}"
            };

            //Act
            await _recordMattersAppService.CreateOrEdit(recordMatter);

            //Assert
            UsingDbContext(context =>
            {
                var newRecordMatter = context.RecordMatters.FirstOrDefault(a => a.RecordMatterName == "Unit Test Record Matter");
                newRecordMatter.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Update_Record_Matter()
        {
            //Arrange
            var exitingRecordMatter = UsingDbContext(context => context.RecordMatters.FirstOrDefault(a => a.RecordMatterName == "Record Matter To Update"));
            CreateOrEditRecordMatterDto recordMatter = new CreateOrEditRecordMatterDto()
            {
                Id = Guid.Parse(exitingRecordMatter.Id.ToString()),
                RecordMatterName = "Record Matter Is Updated",
                Comments = exitingRecordMatter.Comments,
                RecordId = exitingRecordMatter.RecordId,
                Data = exitingRecordMatter.Data
            };

            //Act
            await _recordMattersAppService.CreateOrEdit(recordMatter);

            //Assert
            UsingDbContext(context => context.RecordMatters.FirstOrDefault(a => a.Id == exitingRecordMatter.Id)).RecordMatterName.ShouldBe("Record Matter Is Updated");
        }

        //GetRecordJSONData
        [Fact]
        public async Task Should_Get_Record_Matter_JSON_Data()
        {
            //Arrange
            var exitingRecordMatter = UsingDbContext(context => context.RecordMatters.FirstOrDefault(a => a.RecordMatterName == "Record Matter To Update"));

            //Act
            var output = await _recordMattersAppService.GetRecordMatterJsonData(new EntityDto<Guid>(Guid.Parse(exitingRecordMatter.Id.ToString())));

            //Assert
            //output.ShouldBe("{\"Data\": {\"Trust_Name_txt\": \"Example\",\"submit\": true}}");
            output.ShouldBe("{\r\n  \"Data\": {\r\n    \"Trust_Name_txt\": \"Example\",\r\n    \"submit\": true\r\n  },\r\n  \"AccessToken\": null,\r\n  \"MyUserDataId\": 2,\r\n  \"MyUserDataABN\": null,\r\n  \"MyUserDataAddressCO\": null,\r\n  \"MyUserDataAddressCountry\": null,\r\n  \"MyUserDataAddressLine1\": null,\r\n  \"MyUserDataAddressLine2\": null,\r\n  \"MyUserDataAddressPostCode\": null,\r\n  \"MyUserDataAddressState\": null,\r\n  \"MyUserDataAddressSub\": null,\r\n  \"MyUserDataBillingAddressCountry\": null,\r\n  \"MyUserDataBillingAddressLine1\": null,\r\n  \"MyUserDataBillingAddressLine2\": null,\r\n  \"MyUserDataBillingAddressPostCode\": null,\r\n  \"MyUserDataBillingAddressState\": null,\r\n  \"MyUserDataBillingName\": null,\r\n  \"MyUserDataEmailAddress\": \"admin@defaulttenant.com\",\r\n  \"MyUserDataEmailAddressWork\": null,\r\n  \"MyUserDataEntityAddressCO\": null,\r\n  \"MyUserDataEntityAddressCountry\": null,\r\n  \"MyUserDataEntityAddressLine1\": null,\r\n  \"MyUserDataEntityAddressLine2\": null,\r\n  \"MyUserDataEntityAddressPostCode\": null,\r\n  \"MyUserDataEntityAddressState\": null,\r\n  \"MyUserDataEntityAddressSub\": null,\r\n  \"MyUserDataFax\": null,\r\n  \"MyUserDataFullName\": \" admin\",\r\n  \"MyUserDataJobTitle\": null,\r\n  \"MyUserDataLegalABN\": null,\r\n  \"MyUserDataName\": \"\",\r\n  \"MyUserDataNormalizedEmailAddress\": \"ADMIN@DEFAULTTENANT.COM\",\r\n  \"MyUserDataNormalizedUserName\": \"ADMIN\",\r\n  \"MyUserDataPhoneNumber\": null,\r\n  \"MyUserDataPhoneNumberMobile\": null,\r\n  \"MyUserDataPhoneNumberWork\": null,\r\n  \"MyUserDataSurname\": \"admin\",\r\n  \"MyUserDataTitle\": null,\r\n  \"MyUserDataUserName\": \"admin\",\r\n  \"MyUserDataWebsiteURL\": null\r\n}");
        }

            [Fact]
        public async Task Should_Delete_Record_Matter()
        {
            //Arrange
            var exitingRecordMatter = UsingDbContext(context => context.RecordMatters.FirstOrDefault(a => a.RecordMatterName == "Record Matter To Delete"));

            //Act
            await _recordMattersAppService.Delete(new EntityDto<Guid>(Guid.Parse(exitingRecordMatter.Id.ToString())));

            //Assert
            UsingDbContext(context => context.RecordMatters.FirstOrDefault(a => a.Id == exitingRecordMatter.Id)).IsDeleted.ShouldBe(true);
        }
    }
}
