using Abp.Application.Services.Dto;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using Syntaq.Falcon.ASIC;
using Syntaq.Falcon.Web;
using Microsoft.Extensions.Options;
using Syntaq.Falcon.Configuration;

namespace Syntaq.Falcon.Tests.ASIC
{
    // ReSharper disable once InconsistentNaming
    public class AsicAppService_Tests : AppTestBase
    {
        private readonly IAsicAppService _asicRepository;
        private  IOptions<GetEdgeConfig> _getEdgeConfig;
        public AsicAppService_Tests()
        {
            _asicRepository = Resolve<IAsicAppService>();
            _getEdgeConfig = Options.Create(new GetEdgeConfig()
            {
                XAuthEdge = "GlvlM6lgEzhP17hzAIo8ohPKWLHgQB4pY24KXlg6AszlC368ZVXi42sv5fQp",
                GetEdgeAPI = "https://getedge.com.au/api/v1"
            });
        }

        [Fact]
        public async Task Should_CheckName()
       {
            //Act
            var output = await _asicRepository.CheckName("SYNTAQ");

            //Assert
            output.ShouldBe(3);
        }

        [Fact]
        public async Task Should_Create_EntityVersionHistories_FormCreate()
        {
            //var initialTaskCount = UsingDbContext(context => context.Tasks.Count());
          
        }

        [Fact]
        public async Task Should_Create_EntityVersionHistories_FormEdit()
        {

           
        }


    }
}
