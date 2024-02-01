using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Organizations;
using Abp.Runtime.Session;
using Syntaq.Falcon.Organizations.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Syntaq.Falcon.Teams
{
    public class TeamManager : FalconDomainServiceBase
    {
        public IAbpSession _abpSession { get; set; }

        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        public TeamManager(IRepository<OrganizationUnit, long> organizationUnitRepository, IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository)
        {
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
        }

        public OrganizationUnitDto GetTeamById(long id)
        {
            OrganizationUnitDto Team = ObjectMapper.Map<OrganizationUnitDto>(_organizationUnitRepository.Get(id)); ;
            return Team;
        }

        //public List<UserOrganizationUnit> GetUserTeams(long UserId)
        //{
        //    var UserTeams = _userOrganizationUnitRepository.GetAll().Where(i => i.UserId == UserId).ToList();
        //    return UserTeams;
        //}
    }
}
