using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Authorization.Users.Dto
{
    public interface IGetLoginAttemptsInput: ISortedResultRequest
    {
        string Filter { get; set; }
    }
}