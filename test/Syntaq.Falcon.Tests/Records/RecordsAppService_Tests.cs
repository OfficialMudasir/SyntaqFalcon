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
    public class RecordsAppService_Tests : AppTestBase
    {
        private readonly IRecordsAppService _recordsAppService;

        public RecordsAppService_Tests()
        {
            _recordsAppService = Resolve<IRecordsAppService>();
        }

        [Fact(Skip = "IDEK")]
        //[Fact]
        public async Task Should_Get_All_Records()
        {
            //Act
            var output = await _recordsAppService.GetAll(new GetAllRecordsInput());

            //Assert
            output.Items.Count.ShouldBe(4);
        }

        //GetRecordJSONData
        [Fact]
        public async Task Should_Get_Record_JSON_Data()
        {
            //Arrange
            var exitingRecord = UsingDbContext(context => context.Records.FirstOrDefault(a => a.RecordName == "Record To Update"));

            //Act
            var output = await _recordsAppService.GetRecordJSONData(exitingRecord.Id);

            //Assert
            output.ShouldBe("{\"Data\": {\"Trust_Name_txt\": \"Example\",\"submit\": true}}");
        }

        [Fact]
        public async Task Should_Create_Record()
        {
            //Arrange
            var exitingFolder = UsingDbContext(context => context.Folders.FirstOrDefault(a => a.Name == "Your Records"));
            CreateOrEditRecordDto record = new CreateOrEditRecordDto()
            {
                RecordName = "Unit Test Record",
                Comments = "Record for Unit Testing",
                FolderId = exitingFolder.Id,
                Data = "{}"
            };

            //Act
            await _recordsAppService.CreateOrEdit(record);

            //Assert
            UsingDbContext(context =>
            {
                var newRecord = context.Records.FirstOrDefault(a => a.RecordName == "Unit Test Record");
                newRecord.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Update_Record()
        {
            //Arrange
            var exitingRecord = UsingDbContext(context => context.Records.FirstOrDefault(a => a.RecordName == "Record To Update"));
            CreateOrEditRecordDto record = new CreateOrEditRecordDto()
            {
                Id = Guid.Parse(exitingRecord.Id.ToString()),
                RecordName = "Record Is Updated",
                Comments = exitingRecord.Comments,
                FolderId = (Guid)exitingRecord.FolderId,
                Data = exitingRecord.Data
            };

            //Act
            await _recordsAppService.CreateOrEdit(record);

            //Assert
            UsingDbContext(context => context.Records.FirstOrDefault(a => a.Id == exitingRecord.Id)).RecordName.ShouldBe("Record Is Updated");
        }

        [Fact]
        public async Task Should_Get_Record_For_Edit()
        {
            //Arrange
            var exitingRecord = UsingDbContext(context => context.Records.FirstOrDefault(a => a.RecordName == "Record To Update"));

            //Act
            var output = await _recordsAppService.GetRecordForEdit(new EntityDto<Guid>(Guid.Parse(exitingRecord.Id.ToString())));

            //Assert
            output.Record.RecordName.ShouldBe("Record To Update");
            output.Record.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Delete_Record()
        {
            //Arrange
            var exitingRecord = UsingDbContext(context => context.Records.FirstOrDefault(a => a.RecordName == "Record To Delete"));

            //Act
            await _recordsAppService.Delete(new EntityDto<Guid>(Guid.Parse(exitingRecord.Id.ToString())));

            //Assert
            UsingDbContext(context => context.Records.FirstOrDefault(a => a.Id == exitingRecord.Id)).IsDeleted.ShouldBe(true);
        }
    }
}
