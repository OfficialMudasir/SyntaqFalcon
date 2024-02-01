//using Abp.Domain.Repositories;
//using Abp.Domain.Uow;
//using Microsoft.EntityFrameworkCore;
//using Syntaq.Falcon.Teams.Dtos;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using System.Threading.Tasks;

//namespace Syntaq.Falcon.Teams
//{
//    public class TeamUserRolesManager : FalconDomainServiceBase
//    {
//        private readonly IRepository<TeamUserRole, long> _teamUserRoleRepository;
//        private readonly IUnitOfWorkManager _unitOfWorkManager;


//        public TeamUserRolesManager(IRepository<TeamUserRole, long> teamUserRoleRepository, IUnitOfWorkManager unitOfWorkManager)
//        {
//            _teamUserRoleRepository = teamUserRoleRepository;
//            _unitOfWorkManager = unitOfWorkManager;
//        }

//        //[SfaAuthorize] //Can't get right reference?
//        public async Task AssignOrUpdateRoles(CreateOrUpdateTeamUserRoleInput input) //Is DTO Okay to be used in Domain Service?
//        {
//            input.AssignedTeamUserRoles.Where(n => n.Assigned == "true").ToList().ForEach(async i =>
//            {
//                var teamUserRole = new TeamUserRole
//                {
//                    OrganizationUnitId = input.TeamUser.OrganizationUnitId,
//                    TenantId = input.TeamUser.TenantId,
//                    UserId = input.TeamUser.UserId,
//                    RoleId = i.Id
//                };

//                if (!_teamUserRoleRepository.GetAll().Any(n => n.UserId == teamUserRole.UserId
//                && n.RoleId == teamUserRole.RoleId
//                && n.OrganizationUnitId == teamUserRole.OrganizationUnitId
//                && (teamUserRole.TenantId == null || n.TenantId == teamUserRole.TenantId)))
//                {
//                    await AssignRoles(teamUserRole);
//                }
//            }
//            );

//            input.AssignedTeamUserRoles.Where(n => n.Assigned == "false").ToList().ForEach(async i =>
//            {
//                if (_teamUserRoleRepository.GetAll().Any(n => n.UserId == input.TeamUser.UserId
//                    && n.RoleId == i.Id
//                    && n.OrganizationUnitId == input.TeamUser.OrganizationUnitId
//                    && (input.TeamUser.TenantId == null || n.TenantId == input.TeamUser.TenantId)))
//                {
//                    var teamUserRole = _teamUserRoleRepository.Single(n => n.UserId == input.TeamUser.UserId
//                        && n.RoleId == i.Id
//                        && n.OrganizationUnitId == input.TeamUser.OrganizationUnitId
//                        && (input.TeamUser.TenantId == null || n.TenantId == input.TeamUser.TenantId));

//                    await RemoveRoles(teamUserRole);
//                }
//            }
//            );
//        }

//        private async Task AssignRoles(TeamUserRole teamUserRole)
//        {
//            try
//            {
//                await _teamUserRoleRepository.InsertAsync(teamUserRole);
//                _unitOfWorkManager.Current.SaveChanges();
//            }
//            catch (DbUpdateException e)
//            {
//                //SqlException s = e.InnerException.InnerException as SqlException;
//            }
//        }

//        private async Task RemoveRoles(TeamUserRole teamUserRole)
//        {
//            try
//            {
//                await _teamUserRoleRepository.DeleteAsync(teamUserRole);
//                _unitOfWorkManager.Current.SaveChanges();
//            }
//            catch (DbUpdateException e)
//            {
//                //SqlException s = e.InnerException.InnerException as SqlException;
//            }
//        }
//    }
//}
