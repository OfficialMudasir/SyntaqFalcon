namespace Syntaq.Falcon.Web.Controllers
{
    using Abp.Configuration;
    using Abp.Dependency;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.Runtime.Session;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Syntaq.Falcon.Configuration;
    using Syntaq.Falcon.Records;
    using Syntaq.Falcon.Users;
    using System;
    using System.Linq;

    public class UserAcceptanceFilter : ActionFilterAttribute
	{

        private IAbpSession _abpSession;
        private ISettingManager _settingManager;
        private IUnitOfWorkManager _unitOfWorkManager;
        private IRepository<UserAcceptance, Guid> _userAcceptanceRepository;
        private IRepository<UserAcceptanceType, Guid> _userAcceptanceTypeRepository;
        private IRepository<RecordMatterContributor, Guid> _recordMatterContributorRepository;

        private RecordMatterContributor _rmi = null;

        public UserAcceptanceFilter() : this(
            IocManager.Instance.Resolve<IAbpSession>(),
            IocManager.Instance.Resolve<ISettingManager>(),
            IocManager.Instance.Resolve<IUnitOfWorkManager>(),
            IocManager.Instance.Resolve<IRepository<UserAcceptance, Guid>> (),
            IocManager.Instance.Resolve<IRepository<UserAcceptanceType, Guid>>(),
            IocManager.Instance.Resolve<IRepository<RecordMatterContributor, Guid>>()){
        }

        public UserAcceptanceFilter(
            IAbpSession session,
            ISettingManager settingManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserAcceptance, Guid> userAcceptanceRepository,
            IRepository<UserAcceptanceType, Guid> userAcceptanceTypeRepository,
            IRepository<RecordMatterContributor, Guid> recordMatterContributorRepository){
            _abpSession = session;
            _settingManager = settingManager;
            _unitOfWorkManager = unitOfWorkManager;
            _userAcceptanceRepository = userAcceptanceRepository;
            _userAcceptanceTypeRepository = userAcceptanceTypeRepository;
            _recordMatterContributorRepository = recordMatterContributorRepository;            
        }

        public override void OnActionExecuting(ActionExecutingContext context)
		{
			var controller = context.Controller as ControllerBase;

            if (_settingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsUserAcceptanceRequired).Result)
            {

                if (_abpSession.ImpersonatorUserId == null)
                {
                    var accesstoken = Convert.ToString(context.HttpContext.Request.Query["AccessToken"]) ;

                    if (!controller.RouteData.Values.Values.Contains("UserAcceptance") &&
                           !controller.RouteData.Values.Values.Contains("Login") &&
                           !controller.RouteData.Values.Values.Contains("Logout") &&
                           (!string.IsNullOrEmpty(accesstoken) || _abpSession.UserId != null ))
                        {

                        if (hasRequiredAcceptances(accesstoken))
                        {                           
                            var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;

                            var area = string.Empty;
                            if (!controller.RouteData.Values.ContainsKey("area"))
                            {
                                area = "Falcon";
                            }

                            context.Result = controller.RedirectToAction(
                                actionName: "UserAcceptance",
                                controllerName: $"UserAcceptances",
                                new { area = area, ReturnUrl = returnUrl, RecordMatterContributorId = _rmi?.Id }
                            );

 

                        }
                    }
                }

            }

            base.OnActionExecuting(context);
		}

        private bool hasRequiredAcceptances(string accesstoken)
        {
                 
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
            _rmi = GetRecordMatterContributor(accesstoken);
            var rmiId = _rmi?.Id;  
            var filteredUserAcceptanceTypes = _userAcceptanceTypeRepository.GetAll().Where(e => e.Active);
 
            bool result = (from o in filteredUserAcceptanceTypes
                            join o1 in _userAcceptanceRepository.GetAll()
                            .Where(e => _abpSession.UserId == null || e.UserId == _abpSession.UserId)
                            .Where(e => rmiId == null || e.RecordMatterContributorId == rmiId)
                            on o.Id equals o1.UserAcceptanceTypeId into j1
                            from s1 in j1.DefaultIfEmpty()
                            select s1 == null ? true : false).Any(e => e);

            _unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant);

            return result;
 
        }

        private RecordMatterContributor GetRecordMatterContributor(string accesstoken)
        {
 
            if (! string.IsNullOrEmpty(accesstoken))
            {
                var rmc = _recordMatterContributorRepository.GetAll().FirstOrDefault(e => e.AccessToken == accesstoken);
                return rmc;
            }

            return null;

        }
	}
}