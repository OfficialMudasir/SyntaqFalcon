using System;
using System.Xml.Linq;

namespace Syntaq.Falcon.Documents.Models
{
	public class DocumentAssemblyResponse
	{
		public string Key { get; set; }
		public string[] Error { get; set; }
		public string Document { get; set; }
	}

	public class AssemblyWorkItem
	{
		public string Key { get; set; }
		public XElement Data { get; set; }
		public Guid? ContributorId { get; set; }
		public string DocumentId { get; set; }
		public string DocumentTemplateId { get; set; }
		public byte[] Document { get; set; }
		public string[] Errors { get; set; }
		public bool IsCompleted { get; set; }
		public string MText { get; set; } // XML String

		public bool? AllowWord { get; set; }
		public bool? AllowWordPaid { get; set; }
		public bool? AllowPdf { get; set; }
		public bool? AllowHTML { get; set; }

	}

	public class PostProcessingItem
	{
		public string DocumentTemplateId { get; set; }
		public string DocumentId { get; set; }
		public string Name { get; set; }
		public byte[] Document { get; set; }
		public bool AllowWord { get; set; }
		public byte[] PdfDocument { get; set; }
		public bool AllowPdf { get; set; }
		public byte[] HTMLDocument { get; set; }
		public bool AllowHTML { get; set; }

        public string AllowWordAssignees { get; set; }
        public string AllowPdfAssignees { get; set; }
        public string AllowHtmlAssignees { get; set; }
		public int Order { get; set; }
    }

	public class AppClass
	{
		public AppClass()
		{
			//Set Required Classes Here
		}
		public string Id { get; set; }
        public string FormId { get; set; }
        public string DataURL { get; set; }
		public string AnonAuthToken { get; set; }
		//public JObject data { get; set; }
		public dynamic data { get; set; }
	}

	public class RecordSet
	{
		public long? UserId { get; set; }
		public string ACLPermission { get; set; }

		public long? OrganizationId { get; set; }
		public int? TenantId { get; set; }

		public string SubmissionData { get; set; }

		public Guid RecordId { get; set; }
		public string RecordName { get; set; }

		public Guid RecordMatterId { get; set; }
		public string RecordMatterName { get; set; }
		public bool HasFiles { get; set; }

		public Guid RecordMatterItemId { get; set; }
		public Guid RecordMatterItemGroupId { get; set; }
		public Guid SubmissionId { get; set; }
		public Guid? FormId { get; set; }
		public Guid? AppId { get; set; }

		public Guid FolderId { get; set; }
		public Guid? ParentFolderId { get; set; }
		public string FolderName { get; set; }
		public string FolderType { get; set; }

		public string AnonAuthToken { get; set; }
		public string DocumentName { get; set; }

        public bool LockOnBuild { get; set; }
 

	}

	//public class JobClass
	//{
	//    public JobClass()
	//    {
	//        //Set Required Classes Here
	//    }
	//    public string UserID { get; set; }
	//    public List<XElement> Files { get; set; }
	//    public List<DocumentsDocument> Documents { get; set; }
	//    public DocumentsForm Form { get; set; }
	//    public List<DocumentsRecordMatter> RecordMatters { get; set; }
	//    public Workflow Workflow { get; set; }
	//}

	//public class DocumentsDocument
	//{
	//    public DocumentsDocument(string documentName)
	//    {
	//        DocumentName = documentName;
	//    }
	//    public Uri DocumentTemplateUrl { get; set; }
	//    public string DocumentSummaryTemplateUrl { get; set; }
	//    public string DocumentName { get; set; }
	//    public bool AllowWord { get; set; }
	//    public bool AllowWordPaid { get; set; }
	//    public bool AllowPdf { get; set; }
	//    public decimal DocumentFee { get; set; }
	//    public bool SummaryDoc { get; set; }
	//}

	//public class DocumentsForm
	//{
	//    public DocumentsForm()
	//    {
	//        //Set Required Classes Here
	//    }
	//    public Guid FormId { get; set; }
	//    public string FormName { get; set; }
	//    public string ProjectUrl { get; set; }
	//    public long PaymentValue { get; set; }
	//    public long PaymentValueCents { get; set; }
	//    public string MergeTextUser { get; set; }
	//}

	//public class DocumentsRecordMatter
	//{
	//    public DocumentsRecordMatter()
	//    {
	//        //Set Required Classes Here
	//    }
	//    public long UserId { get; set; }
	//    public long? OrganizationId { get; set; }
	//    public int? TenantId { get; set;}
	//    public Guid RecordId { get; set; }
	//    public string RecordName { get; set; }
	//    public string FileDescription { get; set; }
	//    public Guid RecordMatterId { get; set; }
	//    public string RecordMatterName { get; set; }
	//    public Guid RecordMatterItemId { get; set; }
	//    public Guid RecordMatterItemGroupID { get; set; }
	//    public bool GenerateInvoice { get; set; }
	//    public bool SaveToFormOwner { get; set; }
	//    public string CustomSaveFolderId { get; set; }
	//    public Guid FolderId { get; set; }
	//    public string FolderName { get; set; }
	//}

	//public class Workflow
	//{
	//    public List<Email> Emails { get; set; }
	//    public List<WorkflowItem> AfterAssembly { get; set; }
	//}

	//public class WorkflowItem
	//{
	//    public string URL { get; set; }
	//    public bool Async { get; set; }
	//    public string Rest { get; set; }
	//    public List<APIHeader> Headers { get; set; }
	//    public string BodyType { get; set; }
	//    public string BodyContent { get; set; }
	//}

	//public class APIHeader
	//{
	//    public string Key { get; set; }
	//    public string Value { get; set; }
	//    public string Description { get; set; }
	//}

	//public class Email
	//{
	//    public string EmailTemplateURL { get; set; }
	//    public string TemplatePartID { get; set; }
	//    public string DocumentEmailFrom { get; set; }
	//    public string DocumentEmailSubject { get; set; }
	//    public string DocumentEmailMessage { get; set; }
	//    public string DocumentEmailRecipients { get; set; }
	//    public string DocumentEmailCCRecipients { get; set; }
	//    public string DocumentEmailBCCRecipients { get; set; }
	//    public string UserEmail { get; set; }
	//    public string EmailType { get; set; }
	//    public bool EmailPublisher { get; set; }
	//}
}
