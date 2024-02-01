using Syntaq.Falcon.Auditing;
using Syntaq.Falcon.Test.Base;
using Shouldly;
using Xunit;

namespace Syntaq.Falcon.Tests.Auditing
{
    // ReSharper disable once InconsistentNaming
    public class NamespaceStripper_Tests: AppTestBase
    {
        private readonly INamespaceStripper _namespaceStripper;

        public NamespaceStripper_Tests()
        {
            _namespaceStripper = Resolve<INamespaceStripper>();
        }

        [Fact]
        public void Should_Stripe_Namespace()
        {
            var controllerName = _namespaceStripper.StripNameSpace("Syntaq.Falcon.Web.Controllers.HomeController");
            controllerName.ShouldBe("HomeController");
        }

        [Theory]
        [InlineData("Syntaq.Falcon.Auditing.GenericEntityService`1[[Syntaq.Falcon.Storage.BinaryObject, Syntaq.Falcon.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null]]", "GenericEntityService<BinaryObject>")]
        [InlineData("CompanyName.ProductName.Services.Base.EntityService`6[[CompanyName.ProductName.Entity.Book, CompanyName.ProductName.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[CompanyName.ProductName.Services.Dto.Book.CreateInput, N...", "EntityService<Book, CreateInput>")]
        [InlineData("Syntaq.Falcon.Auditing.XEntityService`1[Syntaq.Falcon.Auditing.AService`5[[Syntaq.Falcon.Storage.BinaryObject, Syntaq.Falcon.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[Syntaq.Falcon.Storage.TestObject, Syntaq.Falcon.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],]]", "XEntityService<AService<BinaryObject, TestObject>>")]
        public void Should_Stripe_Generic_Namespace(string serviceName, string result)
        {
            var genericServiceName = _namespaceStripper.StripNameSpace(serviceName);
            genericServiceName.ShouldBe(result);
        }
    }
}
