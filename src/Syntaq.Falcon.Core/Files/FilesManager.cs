using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Files.Dtos;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Storage;
using Syntaq.Falcon.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Files
{
	public class FilesManager : FalconDomainServiceBase
	{
		public IAbpSession _abpSession { get; set; }

		private readonly ACLManager _ACLManager;
		public IConfiguration Configuration { get; }
		private readonly IOptions<StorageConnection> _storageConnection;
		private readonly IOptions<FileValidationService> _fileValidationService;
		private readonly ITempFileCacheManager _tempFileCacheManager;
		private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;

		public FilesManager(ACLManager aclManager, IConfiguration configuration, IOptions<StorageConnection> storageConnection, IOptions<FileValidationService> fileValidationService, ITempFileCacheManager tempFileCacheManager, IRepository<RecordMatterItem, Guid> recordMatterItemRepository)
		{
			_ACLManager = aclManager;
			Configuration = configuration;
			_storageConnection = storageConnection;
			_fileValidationService = fileValidationService;
			_tempFileCacheManager = tempFileCacheManager;
			_recordMatterItemRepository = recordMatterItemRepository;

		}

        // Needs to be accessed via  an anonymous user
		//[Authorize(Policy = "EditByRecordMatterId")]
		public void SaveFile(SaveFileDto saveFileDto) //FilesDto?
		{
			ACLCheckDto aCLCheckDto = new ACLCheckDto()
			{
				Action = "Edit",
				EntityId = saveFileDto.RecordMatterId,
				UserId = _abpSession.UserId,
                AccessToken = saveFileDto.AccessToken
			};

			bool IsAuthed = _ACLManager.CheckAccess(aCLCheckDto).IsAuthed;

			if (IsAuthed)
			{

				byte[] byteArray;
				var fileBytes = _tempFileCacheManager.GetFile(saveFileDto.File.Token.ToString());

				if (fileBytes == null)
				{
					throw new UserFriendlyException("There is no such image file with the token: " + saveFileDto.File.Token);
				}
				else
				{

					if (ValidateFile(fileBytes, saveFileDto.File.Name).Result )
                    {
						//files stored related to groupId, 
						//groupId = recordmatterId, parsing from form schema, project, recordmatterId=recordmatterId, but for form, rmiId is different from form schema rmiId
						var rmi = _recordMatterItemRepository.GetAll().Where(rm => rm.Id == (Guid)saveFileDto.RecordMatterItemGroupId).FirstOrDefault();
						var groupIdUrl = saveFileDto.RecordMatterItemGroupId;
						if (rmi != null) groupIdUrl = rmi.GroupId;

						//if rmiId in the form is null
						if (groupIdUrl == new Guid("00000000-0000-0000-0000-000000000000")) {
							var gIdtmp = _recordMatterItemRepository.GetAll().Where(rm => rm.RecordMatterId == (Guid)saveFileDto.RecordMatterId).OrderByDescending(i => i.CreationTime).FirstOrDefault();
							groupIdUrl = gIdtmp != null ? gIdtmp.GroupId : new Guid("00000000-0000-0000-0000-000000000000");
						}

						using (var stream = new MemoryStream())
						{
							byteArray = fileBytes.ToArray();
						}

						CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnection.Value.ConnectionString);
						string cloudStorageAccountTable = _storageConnection.Value.BlobStorageContainer;

						CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
						CloudBlobContainer container = blobClient.GetContainerReference(cloudStorageAccountTable);

						// New Blob
						CloudBlockBlob blockBlob = container.GetBlockBlobReference("file-uploads" + "/" + saveFileDto.RecordId + "/" + saveFileDto.RecordMatterId + "/" + groupIdUrl + "/" + saveFileDto.File.Name);
						blockBlob.UploadFromStream(new MemoryStream(byteArray));
                    }
                    else
                    {
						throw new UserFriendlyException("Malware scan failed. ");
					}

				}
			}
			else
			{
				throw new Exception();
			}
		}

		public async Task<bool> ValidateFile(byte[] file, string fname)
		{

			bool result = true;
			var url = _fileValidationService.Value.Url;

            if (!string.IsNullOrEmpty(url))
            {
				using (var httpClient = new HttpClient())
				{

					var OcpApimSubscriptionKey = _fileValidationService.Value.OcpApimSubscriptionKey;
					var initiatedby = _fileValidationService.Value.ApiGovtNzInitiatedBy;
 
					using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
					{
						request.Headers.TryAddWithoutValidation("Transfer-Encoding", "chunked");
						request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", OcpApimSubscriptionKey); // rfx
						request.Headers.TryAddWithoutValidation("api-govt-nz-InitiatedBy", initiatedby);

						var multipartContent = new MultipartFormDataContent();

						var filecontent = new ByteArrayContent(file);
						multipartContent.Add(filecontent, fname);
						request.Content = multipartContent;

						var response = await httpClient.SendAsync(request);

						if (response.StatusCode == System.Net.HttpStatusCode.OK)
						{
							result = true;
						}
                        else
                        {
							result = false;
                        }

					}
				}
            }

			return result;

		}
	}
}