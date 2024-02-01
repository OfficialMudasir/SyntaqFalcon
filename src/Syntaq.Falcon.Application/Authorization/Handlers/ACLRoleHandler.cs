using Abp.Runtime.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Authorization.Requirements;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static Syntaq.Falcon.AccessControlList.ACLManager;

namespace Syntaq.Falcon.Authorization.Handlers
{
	public class ACLRoleHandler : AuthorizationHandler<ACLRoleRequirement>
	{
		private IHttpContextAccessor _contextAccessor;
		private readonly ACLManager _ACLManager;
		public IAbpSession _abpSession { get; set; }


		public ACLRoleHandler(IHttpContextAccessor contextAccessor, ACLManager aCLManager, IAbpSession abpSession)
		{
			_contextAccessor = contextAccessor;
			_ACLManager = aCLManager;
			_abpSession = abpSession;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ACLRoleRequirement requirement)
		{
			MemoryStream httpBodyCopy = new MemoryStream();
			_contextAccessor.HttpContext.Request.Body.CopyToAsync(httpBodyCopy);
			var Query = _contextAccessor.HttpContext.Request.Query;

			var accesstoken = _contextAccessor.HttpContext.Request.Query["AccessToken"].ToString();

			// Can grant access via session object or AccessToken passed in query string
			//if (_abpSession.UserId == null && string.IsNullOrEmpty(accesstoken))
			//{AllowSynchronousIO 
			//    return Task.CompletedTask;
			//}

			string EntityID = null;
			switch (_contextAccessor.HttpContext.Request.Method)
			{
				case "POST":
					if (_contextAccessor.HttpContext.Request.HasFormContentType)
					{
						byte[] formByt = httpBodyCopy.ToArray();
						string formStr = Encoding.UTF8.GetString(formByt);
						NameValueCollection result = System.Web.HttpUtility.ParseQueryString(formStr);
						EntityID = result[requirement.EntityKey.ToString()];
					} 
					else
					{
						byte[] jsonByt = httpBodyCopy.ToArray();
						string jsonStr = Encoding.UTF8.GetString(jsonByt);
						var HttpBody = JsonConvert.DeserializeObject<dynamic>(jsonStr);
						switch (requirement.EntityKey)
						{
							case "Id":
								EntityID = HttpBody.id;
								break;
							case "OriginalId":
								EntityID = HttpBody.GetValue("originalId", StringComparison.InvariantCultureIgnoreCase);
								break;
							case "RecordMatterId":

								// HttpBody is a dynamic string
								JObject j = JObject.Parse(HttpBody);
								//EntityID = jToken.First.Value<string>("RecordMatterId") ?? string.Empty;

								EntityID = Convert.ToString( j["RecordMatterId"]);

								break;
						}
					}
					break;
				case "GET":
					EntityID = Query[requirement.EntityKey.ToString()].ToString();
					break;
				case "DELETE":
					EntityID = Query[requirement.EntityKey.ToString()].ToString();
					break;
			}

			if (!string.IsNullOrEmpty(EntityID) && EntityID.ToString() != "00000000-0000-0000-0000-000000000000")
			{
				switch (requirement.EntityKey)
				{
					case "OriginalId":
						switch (requirement.Role)
						{
							case "Edit":
								List<ACL> EUserACLs = _ACLManager.FetchAllUserACLs(new GetAllACLsInput(){ UserId = (long)_abpSession.UserId });
								if (EUserACLs.Exists(i => i.EntityID.ToString() == EntityID)) // ?Question this had a ! not operation 
								{
									context.Succeed(requirement);
									_contextAccessor.HttpContext.Request.Body = httpBodyCopy;
									_contextAccessor.HttpContext.Request.Body.Position = 0;
									return Task.CompletedTask;
								}
								break;
						}
						break;
					case "RecordMatterId":

						break;
					//default:
					//    ACLCheckDto aCLCheckDto = new ACLCheckDto()
					//    {
					//        Action = requirement.Role,
					//        EntityId = Guid.Parse(EntityID),
					//        UserId = _abpSession.UserId
					//    };

					//    bool IsAuthed = _ACLManager.CheckAccess(aCLCheckDto);

					//    if (IsAuthed)
					//    {
					//        context.Succeed(requirement);
					//    }
					//    break;
				}
				ACLCheckDto aCLCheckDto = new ACLCheckDto()
				{
					Action = requirement.Role,
							EntityId = Guid.Parse(EntityID),
							UserId = _abpSession.UserId,
							AccessToken = accesstoken,
							TenantId = _abpSession.TenantId
						};

				ACLResultDto ACLResult = _ACLManager.CheckAccess(aCLCheckDto);

				if (ACLResult.IsAuthed)
				{
					context.Succeed(requirement);
				}
			}
			else
			{
				switch (requirement.EntityKey)
				{
					case "Id":
						if (requirement.Role == "Edit" && (string.IsNullOrEmpty(EntityID)) || EntityID == "00000000-0000-0000-0000-000000000000")
						{
							context.Succeed(requirement);
							_contextAccessor.HttpContext.Request.Body = httpBodyCopy;
							_contextAccessor.HttpContext.Request.Body.Position = 0;
							return Task.CompletedTask;
						}
						break;
					case "OriginalId":
						switch (requirement.Role)
						{
							case "Edit":
								if (string.IsNullOrEmpty(EntityID.ToString()))
								{
									context.Succeed(requirement);
									_contextAccessor.HttpContext.Request.Body = httpBodyCopy;
									_contextAccessor.HttpContext.Request.Body.Position = 0;
									return Task.CompletedTask;
								}
								break;
							case "Delete":
								EntityID = Query["id"].ToString();
								List<ACL> DUserACLs = _ACLManager.FetchAllUserACLs(new GetAllACLsInput() { UserId = (long)_abpSession.UserId });
								if (DUserACLs.Exists(i => i.EntityID.ToString() == EntityID))
								{
									context.Succeed(requirement);
									_contextAccessor.HttpContext.Request.Body = httpBodyCopy;
									_contextAccessor.HttpContext.Request.Body.Position = 0;
									return Task.CompletedTask;
								}
								break;
						}
						break;
				}
				// If entity ID is empty assume the entity is a folder
				var qType = Query["type"].ToString();
				if (!string.IsNullOrEmpty(qType))
				{
					EntityID = EntityID.ToString() == "00000000-0000-0000-0000-000000000000" ? _ACLManager.FetchUserRootFolder((long)_abpSession.UserId, qType).ToString() : EntityID;
					ACLCheckDto aCLCheckDto = new ACLCheckDto()
					{
						Action = requirement.Role,
						EntityId = Guid.Parse(EntityID),
						UserId = _abpSession.UserId
					};

					ACLResultDto ACLResult = _ACLManager.CheckAccess(aCLCheckDto);

					if (ACLResult.IsAuthed)
					{
						context.Succeed(requirement);
					}
				}
				else
				{
					_contextAccessor.HttpContext.Request.Body = httpBodyCopy;
					_contextAccessor.HttpContext.Request.Body.Position = 0;

					return Task.CompletedTask;
				}
			}

			//if(requirement.Role == "Edit" && requirement.EntityKey == "Id" && (EntityID == "" || EntityID == null))
			//{
			//    context.Succeed(requirement);
			//}
			//else if (requirement.Role == "Edit" && requirement.EntityKey == "OriginalId")
			//{
			//    List<ACL> UserACLs = _ACLManager.FetchAllUserACRs((long)_abpSession.UserId);
			//    if (UserACLs.Exists(i => i.EntityID.ToString() == EntityID)) // ?Question this had a ! not operation 
			//    {
			//        context.Succeed(requirement);
			//    }
			//}
			//else if (requirement.Role == "Delete" && requirement.EntityKey == "OriginalId")
			//{
			//    EntityID = Query["id"].ToString();
			//    List<ACL> UserACLs = _ACLManager.FetchAllUserACRs((long)_abpSession.UserId);
			//    if (UserACLs.Exists(i => i.EntityID.ToString() == EntityID))
			//    {
			//        context.Succeed(requirement);
			//    }
			//}
			//else
			//{

			//    // If entity ID is empty assume the entity is a folder
			//    EntityID = string.IsNullOrEmpty(EntityID) ? _ACLManager.FetchUserRootFolder((long)_abpSession.UserId, "R").ToString() : EntityID;

			//    ACLCheckDto aCLCheckDto = new ACLCheckDto()
			//    {
			//        Action = requirement.Role,
			//        EntityId = Guid.Parse(EntityID),
			//        UserId = _abpSession.UserId
			//    };

			//    bool IsAuthed = _ACLManager.CheckAccess(aCLCheckDto);

			//    if (IsAuthed)
			//    {
			//        context.Succeed(requirement);
			//    }
			//}

			_contextAccessor.HttpContext.Request.Body = httpBodyCopy;
			_contextAccessor.HttpContext.Request.Body.Position = 0;

			return Task.CompletedTask;
		}
	}
}
