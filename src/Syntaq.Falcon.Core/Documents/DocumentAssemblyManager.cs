using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.Webhooks;
using AngleSharp.Html.Parser;
using Aspose.Words;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stripe;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.API;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Configuration.Tenants.Dto;
using Syntaq.Falcon.Documents.Models;
using Syntaq.Falcon.Files.Dtos;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.MergeTexts;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Utility;
using Syntaq.Falcon.Web;
using Syntaq.Falcon.WebHooks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
 
namespace Syntaq.Falcon.Documents 
{
    public class DocumentAssemblyManager : FalconDomainServiceBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly IRepository<ACL> _aclRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IRepository<Form, Guid> _formRepository;
        private readonly IRepository<Submission, Guid> _submissionsRepository;
        private readonly IRepository<Record, Guid> _recordsRepository;
        private readonly IRepository<RecordMatter, Guid> _recordMattersRepository;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IRepository<RecordMatterContributor, Guid> _recordMatterContributorRepository;
        private readonly IRepository<RecordMatterItemHistory, Guid> _recordMatterItemHistoryRepository;
        private readonly SubmissionManager _submissionManager;
        private readonly RecordManager _recordManager;
        private readonly IEmailSender _emailSender;
        private readonly APIManager _APIManager;

        private readonly IOptions<StorageConnection> _storageConnection;
        private readonly IOptions<AssemblyFunctionConnection> _assemblyFunctionConnection;
        private readonly IOptions<AppConfig> _appConfig;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAbpSession _AbpSession;
        private readonly IWebhookPublisher _webHookPublisher;

        private string DocumentName = "_Debugged";
        private HttpClient Client = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10)
        };

        public DocumentAssemblyManager(
            IUnitOfWorkManager unitOfWorkManager
            , IRepository<ACL> aclRepository
            , IRepository<Submission, Guid> submissionsRepository
            , IRepository<Project, Guid> projectRepository
            , IRepository<Record, Guid> recordsRepository
            , IRepository<RecordMatter, Guid> recordMattersRepository
            , IRepository<RecordMatterContributor, Guid> recordMatterContributorRepository
            , IRepository<RecordMatterItem, Guid> recordMatterItemRepository
            , IRepository<RecordMatterItemHistory, Guid> recordMatterItemHistoryRepository
            , IRepository<Form, Guid> formRepository
            , SubmissionManager submissionManager
            , IRepository<MergeText, long> mergeTextRepository
            , RecordManager recordManager
            , IEmailSender emailSender
            , APIManager aPIManager
            , IOptions<StorageConnection> storageConnection
            , IOptions<AssemblyFunctionConnection> assemblyFunctionConnection
            , IOptions<AppConfig> appConfig
            , IHttpContextAccessor httpContextAccessor
            , IAbpSession AbpSession
            , IWebhookPublisher webHookPublisher
            )
        {

            _unitOfWorkManager = unitOfWorkManager;
            _submissionsRepository = submissionsRepository;
            _aclRepository = aclRepository;
            _recordsRepository = recordsRepository;
            _recordMattersRepository = recordMattersRepository;
            _recordMatterItemRepository = recordMatterItemRepository;
            _recordMatterContributorRepository = recordMatterContributorRepository;
            _recordMatterItemHistoryRepository = recordMatterItemHistoryRepository;
            _formRepository = formRepository;
            _projectRepository = projectRepository;
            _submissionManager = submissionManager;
            _recordManager = recordManager;
            _emailSender = emailSender;
            _APIManager = aPIManager;
            _storageConnection = storageConnection;
            _assemblyFunctionConnection = assemblyFunctionConnection;
            _appConfig = appConfig;
            _httpContextAccessor = httpContextAccessor;

            _AbpSession = AbpSession;
            _webHookPublisher = webHookPublisher;

        }

        /// <summary>
        /// Step 2: "Creating the Assembly Tasks" - Spawns both an assembly task and an awaiting callback task for each recieved XElement file
        /// </summary>
        /// <param name="XData">List of XElement XData to assemble</param>
        public async void AssemblyManager(CreateOrEditAppJobDto jobClass)
        {

            License license = new License();
            license.SetLicense("Aspose.Words.lic");

            JObject jobData = JObject.Parse(jobClass.Data);
            //decimal voucherAmount = jobData.ContainsKey("VoucherAmount") ? jobData.Value<decimal>("VoucherAmount") : 0;

            //var _RequiresPayment = !string.IsNullOrEmpty(GetJArrayValue(SubData, "PaymentProcess")) ? true : false;
            _submissionManager.UpdateSubmission(new CreateOrEditSubmissionDto()
            {
                Id = jobClass.RecordMatter.First().SubmissionId,
                //RequiresPayment = _RequiresPayment,
                //PaymentStatus = Convert.ToBoolean(GetJArrayValue(SubData, "IsPaid")) == true ? "Paid" : _RequiresPayment == true ? "Required" : "Not Required",
                UserId = jobClass.User.ID,
                TenantId = jobClass.TenantId,
                FormId = jobClass.FormId != null ? new Guid(Convert.ToString(jobClass.FormId)) : (Guid?)null,
                AppId = jobClass.AppId != null ? new Guid(Convert.ToString(jobClass.AppId)) : (Guid?)null,
                RecordId = jobClass.RecordMatter.First().RecordId,
                RecordMatterId = jobClass.RecordMatter.First().RecordMatterId,
                SubmissionStatus = "Assembling",
                // VoucherAmount = voucherAmount,
            });

            List<AssemblyWorkItem> AssemblyCollection = new List<AssemblyWorkItem>();
            var TaskList = new List<Task<dynamic>>();

            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.ConnectionClose = false;

            #region Old Code STQ Modified
            //var i = 0;
            //jobClass.XData.ForEach(j =>
            //{

            //    //Create Assembly Work Item and Add to dynamic Collection
            //    AssemblyWorkItem assemblyWorkItem = new AssemblyWorkItem()
            //    {
            //        Key = new Random().Next(10000, 420000).ToString("X"),
            //        ContributorId = jobClass.ContributorId,
            //        Data = j,
            //        DocumentId = j.Element("System") != null ? Convert.ToString(j.Element("System").Element("Document").Element("DocumentId").Value) : "",
            //        DocumentTemplateId = j.Element("System") != null ? Convert.ToString(j.Element("System").Element("Document").Element("DocumentTemplateURL").Value) : "",
            //        Document = null,
            //        Errors = null,
            //        IsCompleted = false,
            //        MText = j.Element("System") != null ? Convert.ToString(j.Element("System").Element("Document").Element("MergeText")) : "", // NULL Test 
            //    };

            //    if (!string.IsNullOrEmpty(assemblyWorkItem.DocumentId))
            //    {
            //        AssemblyCollection.Add(assemblyWorkItem);

            //        //Make API call and Add Task to Task Collection
            //        TaskList.Add(AssembleDocumentsAsync(assemblyWorkItem, j));
            //        i++;
            //    }

            //});


            //var CallbackTasks = TaskList.Select(k =>
            //{
            //    return AwaitAndProcessAsync(k, jobClass, AssemblyCollection);
            //});

            //await Task.WhenAll(CallbackTasks);

            //if (AssemblyCollection.Any(l => l.IsCompleted != false))
            //{
            //    AllAssemblyComplete(jobClass, AssemblyCollection);
            //}

            //if (TaskList.Count == 0)
            //{
            //    AllAssemblyComplete(jobClass, AssemblyCollection);
            //}
            #endregion

            #region New Code 10512 Changes STQ Modified
            var i = 0;
            try
            {
                // STQ Modifed
                jobClass.XData.ForEach(j =>
                {
                    //Create Assembly Work Item and Add to dynamic Collection
                    AssemblyWorkItem assemblyWorkItem = new AssemblyWorkItem()
                    {
                        Key = new Random().Next(10000, 420000).ToString("X"),
                        ContributorId = jobClass.ContributorId,
                        Data = j,
                        DocumentId = j.Element("System") != null ? Convert.ToString(j.Element("System").Element("Document").Element("DocumentId").Value) : "",
                        DocumentTemplateId = j.Element("System") != null ? Convert.ToString(j.Element("System").Element("Document").Element("DocumentTemplateURL").Value) : "",
                        Document = null,
                        Errors = null,
                        IsCompleted = false,
                        MText = j.Element("System") != null ? Convert.ToString(j.Element("System").Element("Document").Element("MergeText")) : "", // NULL Test 
                    };

                    if (!string.IsNullOrEmpty(assemblyWorkItem.DocumentId))
                    {
                        AssemblyCollection.Add(assemblyWorkItem);

                        // STQ MOdified
                        //Make API call and Add Task to Task Collection
                        string documentName = j.Element("System")?.Element("Document")?.Element("DocumentName")?.Value;

                        string debuggedDocumentName = documentName + DocumentName;
                        var assemblyDebug = Convert.ToBoolean(j.Element("Assembly_Debug")?.Value);
                        if (assemblyDebug)
                        {
                            j.Element("System")?.Element("Document")?.Element("DocumentName")?.SetValue(debuggedDocumentName);
                            jobClass.IsAssemblyDebug = assemblyDebug;
                        }

                        TaskList.Add(AssembleDocumentsAsync(assemblyWorkItem, j));
                        var updatedXdata = new XElement(j); // Create a deep copy of j

                        if (assemblyDebug) // If it is true
                        {
                            assemblyDebug = false;
                            //documentName += "_Debugged";
                            // Update "DocumentName" with new value
                            updatedXdata.Element("System")?.Element("Document")?.Element("DocumentName")?.SetValue(documentName);

                            // Update "Assembly_Debug" with new value
                            updatedXdata.Element("Assembly_Debug")?.SetValue(assemblyDebug.ToString());

                            //Create Assembly Work Item and Add to dynamic Collection
                            AssemblyWorkItem debugAssemblyWorkItem = new AssemblyWorkItem()
                            {
                                Key = new Random().Next(10000, 420000).ToString("X"),
                                ContributorId = jobClass.ContributorId,
                                Data = updatedXdata,
                                DocumentId = updatedXdata.Element("System") != null ? Convert.ToString(updatedXdata.Element("System").Element("Document").Element("DocumentId").Value) : "",
                                DocumentTemplateId = updatedXdata.Element("System") != null ? Convert.ToString(updatedXdata.Element("System").Element("Document").Element("DocumentTemplateURL").Value) : "",
                                Document = null,
                                Errors = null,
                                IsCompleted = false,
                                MText = updatedXdata.Element("System") != null ? Convert.ToString(updatedXdata.Element("System").Element("Document").Element("MergeText")) : "", // NULL Test 
                            };

                            AssemblyCollection.Add(debugAssemblyWorkItem);
                            //Make API call and Add Task to Task Collection
                            TaskList.Add(AssembleDocumentsAsync(debugAssemblyWorkItem, updatedXdata));
                        }
                        i++;
                    }
                });

                var CallbackTaskas = TaskList.Select( k =>
                {
                    return  AwaitAndProcessAsync(k, jobClass, AssemblyCollection);
                });
                await Task.WhenAll(CallbackTaskas);

                if (AssemblyCollection.Any(l => l.IsCompleted != false))
                {
                    AllAssemblyComplete(jobClass, AssemblyCollection);
                }

                if (TaskList.Count == 0)
                {
                    AllAssemblyComplete(jobClass, AssemblyCollection);
                }
            }
            catch (Exception ex)
            {
              
            }
            #endregion
        }

        /// <summary>
        /// Step 2.5: "Assembly Caller" - calls Assembly Function and passes in XElement Data
        /// </summary>
        /// <param name="URL">URL of Assembly Function</param>
        /// <param name="Data">XElement Data</param>
        /// <returns></returns>
        private async Task<dynamic> AssembleDocumentsAsync(AssemblyWorkItem workitem, XElement Data)
        {
            //removed Http context to allow hangfire jobs to run
            //var RequestAuthority = _httpContextAccessor.HttpContext.Request.Scheme + "://" +  _httpContextAccessor.HttpContext.Request.Host.Value;
            var RequestAuthority = _appConfig.Value.WebSiteRootAddress.Trim('/');
            // GET FROM ENTITY
            Data.Name = "File";
            MultipartFormDataContent RequestContent = new MultipartFormDataContent
            {
                { new StringContent(Convert.ToString(Data), Encoding.UTF8, "application/xml"), "Data" },
                { new StringContent(workitem.Key), "Key" },
                { new StringContent(workitem.MText, Encoding.UTF8, "application/xml"), "MText" },
                { new StringContent(RequestAuthority), "RequestAuthority" },
            };

            string url = _assemblyFunctionConnection.Value.ConnectionString;

            //Logger.Debug(url);

            return await Client.PostAsync(url, RequestContent);

        }

        /// <summary>
        /// Step 4: "Awaiting Callback Task" - recieves returned object from Assembly Function and preporcesses data ready for callback function (Step 4)
        /// </summary>
        /// <param name="Task"></param>
        /// <returns></returns>
        private async Task<dynamic> AwaitAndProcessAsync(Task<dynamic> Task, CreateOrEditAppJobDto jobClass, List<AssemblyWorkItem> AssemblyCollection)
        {
            var Result = await Task;

            string ResultString = Result.Content.ReadAsStringAsync().Result as string;
            DocumentAssemblyResponse ReturnJSON = JsonConvert.DeserializeObject<DocumentAssemblyResponse>(ResultString, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }) as DocumentAssemblyResponse;

            string Key = ReturnJSON.Key;

            string[] Errors = null;
            if (ReturnJSON.Error != null)
            {
                Errors = ReturnJSON.Error;
            }

            byte[] DocBytes = null;
            if (ReturnJSON.Document != null)
            {
                DocBytes = Convert.FromBase64String(ReturnJSON.Document);
            }

            //Split document on WF:DocumentBreakPoints
            // Add each document into the Assembly Collection
            // 1. Generate Key
            // 2. Set isCompleted = true
            // 3. Document = bytes
            // 4. Errors = Errors

            // Check for DocumentBreakPoints 
            Aspose.Words.Document sourcedoc = null;
            sourcedoc = new Aspose.Words.Document(new MemoryStream(DocBytes));

            if (sourcedoc.Range.Bookmarks["DocumentBreakPoint0"] != null || sourcedoc.Range.Bookmarks["DocumentBreakPoint1"] != null)
            {
                int index = 1;

                XElement xdata = XElement.Parse("<File></File>");
                foreach (Bookmark bookmark in sourcedoc.Range.Bookmarks)
                {
                    string bookmarkname = string.IsNullOrEmpty(bookmark.Name) ? string.Empty : bookmark.Name;
                    if (bookmarkname.ToLower().StartsWith("documentbreakpoint"))
                    {

                        ArrayList extractedNodesInclusive = ExtractContent(bookmark.BookmarkStart, bookmark.BookmarkEnd, true);
                        Aspose.Words.Document dstDoc = CloneBookMark(sourcedoc, extractedNodesInclusive);

                        DocumentBuilder builder = new DocumentBuilder(dstDoc);
                        builder.MoveToDocumentStart();
                        builder.Write("wf:donotappend");

                        MemoryStream outStream = new MemoryStream();

                        // Need to get the Allowed Formats here
                        var assemblyworkitem = AssemblyCollection.FirstOrDefault(a => a.Key == Key);

                        if (assemblyworkitem != null)
                        {
                            bool allowWord = false; bool allowPdf = false; bool allowHTML = false;

                            var jobClassDoc = jobClass.Document.FirstOrDefault(n => Convert.ToString(n.DocumentTemplateURL) == assemblyworkitem.DocumentTemplateId);
                            if (jobClassDoc != null)
                            {
                                allowWord = jobClassDoc.AllowWord.GetValueOrDefault(false);
                                allowPdf = jobClassDoc.AllowPdf.GetValueOrDefault(false);
                                allowHTML = jobClassDoc.AllowHTML.GetValueOrDefault(false);
                            }

                            // HOW DOES THIS WORK

                            dstDoc.Save(outStream, SaveFormat.Docx);
                            byte[] docBytes = outStream.ToArray();

                            var documentid = "1"; /// used to set the document permissions and set by the first item in the split
                            int indextoinsert = 0;
                            if (index == 1) // original doc
                            {
                                AssemblyWorkItem workitem = AssemblyCollection.FirstOrDefault(n => n.Key == Key);
                                if (workitem != null)
                                {

                                    indextoinsert = AssemblyCollection.IndexOf(workitem);

                                    xdata = workitem.Data;
                                    documentid = workitem.DocumentId;

                                    workitem.IsCompleted = true;
                                    workitem.Document = docBytes;
                                    workitem.Errors = Errors;

                                }
                            }
                            else
                            {

                                indextoinsert = AssemblyCollection.IndexOf(AssemblyCollection.Last(a => a.DocumentTemplateId == assemblyworkitem.DocumentTemplateId)) + 1;

                                // Add into collection as new document
                                AssemblyWorkItem item = new AssemblyWorkItem()
                                {
                                    Key = new Random().Next(10000, 420000).ToString("X"),
                                    DocumentId = documentid,
                                    DocumentTemplateId = assemblyworkitem.DocumentTemplateId,
                                    Data = xdata,
                                    Document = docBytes,
                                    Errors = Errors,
                                    IsCompleted = true,
                                    AllowHTML = allowHTML,
                                    AllowPdf = allowPdf,
                                    AllowWord = allowWord
                                };

                                AssemblyCollection.Insert(indextoinsert, item);

                            }

                            index++; ;

                        }
                    }
                }
            }
            else
            {
                // No BreakPoints
                AssemblyCollection.ToList().ForEach(i => {
                    if (i.Key == Key)
                    {
                        i.IsCompleted = true;
                        i.Document = DocBytes;
                        i.Errors = Errors;
                    }
                });
            }

            return Result;
        }


        private Aspose.Words.Document CloneBookMark(Aspose.Words.Document srcDoc, ArrayList nodes)
        {

            // Create a blank document.
            //Aspose.Words.Document dstDoc = new Aspose.Words.Document();
            // Clone to include the headers
            Aspose.Words.Document dstDoc = (Aspose.Words.Document)srcDoc.Clone(true);

            // Remove the first paragraph from the empty document.
            dstDoc.FirstSection.Body.RemoveAllChildren();

            // Import each node from the list into the new document. Keep the original formatting of the node.
            NodeImporter importer = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepSourceFormatting);

            foreach (Node node in nodes)
            {
                Node importNode = importer.ImportNode(node, true);
                dstDoc.FirstSection.Body.AppendChild(importNode);
            }

            // Return the generated document.
            return dstDoc;
        }


        public static ArrayList ExtractContent(Node startNode, Node endNode, bool isInclusive)
        {
            // First check that the nodes passed to this method are valid for use.
            VerifyParameterNodes(startNode, endNode);

            // Create a list to store the extracted nodes.
            ArrayList nodes = new ArrayList();

            // Keep a record of the original nodes passed to this method so we can split marker nodes if needed.
            Node originalStartNode = startNode;
            Node originalEndNode = endNode;

            // Extract content based on block level nodes (paragraphs and tables). Traverse through parent nodes to find them.
            // We will split the content of first and last nodes depending if the marker nodes are inline
            while (startNode.ParentNode.NodeType != NodeType.Body)
                startNode = startNode.ParentNode;

            while (endNode.ParentNode.NodeType != NodeType.Body)
                endNode = endNode.ParentNode;

            bool isExtracting = true;
            bool isStartingNode = true;
            bool isEndingNode = false;
            // The current node we are extracting from the document.
            Node currNode = startNode;

            // Begin extracting content. Process all block level nodes and specifically split the first and last nodes when needed so paragraph formatting is retained.
            // Method is little more complex than a regular extractor as we need to factor in extracting using inline nodes, fields, bookmarks etc as to make it really useful.
            while (isExtracting)
            {
                // Clone the current node and its children to obtain a copy.
                Node cloneNode = currNode.Clone(true);
                isEndingNode = currNode.Equals(endNode);

                if ((isStartingNode || isEndingNode) && cloneNode.IsComposite)
                {
                    // We need to process each marker separately so pass it off to a separate method instead.
                    if (isStartingNode)
                    {
                        ProcessMarker((CompositeNode)cloneNode, nodes, originalStartNode, isInclusive, isStartingNode, isEndingNode);
                        isStartingNode = false;
                    }

                    // Conditional needs to be separate as the block level start and end markers maybe the same node.
                    if (isEndingNode)
                    {
                        ProcessMarker((CompositeNode)cloneNode, nodes, originalEndNode, isInclusive, isStartingNode, isEndingNode);
                        isExtracting = false;
                    }
                }
                else
                    // Node is not a start or end marker, simply add the copy to the list.
                    nodes.Add(cloneNode);

                // Move to the next node and extract it. If next node is null that means the rest of the content is found in a different section.
                if (currNode.NextSibling == null && isExtracting)
                {
                    // Move to the next section.
                    Section nextSection = (Section)currNode.GetAncestor(NodeType.Section).NextSibling;
                    currNode = nextSection.Body.FirstChild;
                }
                else
                {
                    // Move to the next node in the body.
                    currNode = currNode.NextSibling;
                }
            }

            // Return the nodes between the node markers.
            return nodes;
        }
        // For complete examples and data files, please go to https://github.com/aspose-words/Aspose.Words-for-.NET

        private static void VerifyParameterNodes(Node startNode, Node endNode)
        {
            // The order in which these checks are done is important.
            if (startNode == null)
                throw new ArgumentException("Start node cannot be null");
            if (endNode == null)
                throw new ArgumentException("End node cannot be null");

            if (!startNode.Document.Equals(endNode.Document))
                throw new ArgumentException("Start node and end node must belong to the same document");

            if (startNode.GetAncestor(NodeType.Body) == null || endNode.GetAncestor(NodeType.Body) == null)
                throw new ArgumentException("Start node and end node must be a child or descendant of a body");

            // Check the end node is after the start node in the DOM tree
            // First check if they are in different sections, then if they're not check their position in the body of the same section they are in.
            Section startSection = (Section)startNode.GetAncestor(NodeType.Section);
            Section endSection = (Section)endNode.GetAncestor(NodeType.Section);

            int startIndex = startSection.ParentNode.IndexOf(startSection);
            int endIndex = endSection.ParentNode.IndexOf(endSection);

            if (startIndex == endIndex)
            {
                if (startSection.Body.IndexOf(startNode) > endSection.Body.IndexOf(endNode))
                    throw new ArgumentException("The end node must be after the start node in the body");
            }
            else if (startIndex > endIndex)
                throw new ArgumentException("The section of end node must be after the section start node");
        }

        private static bool IsInline(Node node)
        {
            // Test if the node is desendant of a Paragraph or Table node and also is not a paragraph or a table a paragraph inside a comment class which is decesant of a pararaph is possible.
            return ((node.GetAncestor(NodeType.Paragraph) != null || node.GetAncestor(NodeType.Table) != null) && !(node.NodeType == NodeType.Paragraph || node.NodeType == NodeType.Table));
        }

        private static void ProcessMarker(CompositeNode cloneNode, ArrayList nodes, Node node, bool isInclusive, bool isStartMarker, bool isEndMarker)
        {
            // If we are dealing with a block level node just see if it should be included and add it to the list.
            if (!IsInline(node))
            {
                // Don't add the node twice if the markers are the same node
                if (!(isStartMarker && isEndMarker))
                {
                    if (isInclusive)
                        nodes.Add(cloneNode);
                }
                return;
            }

            // If a marker is a FieldStart node check if it's to be included or not.
            // We assume for simplicity that the FieldStart and FieldEnd appear in the same paragraph.
            if (node.NodeType == NodeType.FieldStart)
            {
                // If the marker is a start node and is not be included then skip to the end of the field.
                // If the marker is an end node and it is to be included then move to the end field so the field will not be removed.
                if ((isStartMarker && !isInclusive) || (!isStartMarker && isInclusive))
                {
                    while (node.NextSibling != null && node.NodeType != NodeType.FieldEnd)
                        node = node.NextSibling;

                }
            }

            // If either marker is part of a comment then to include the comment itself we need to move the pointer forward to the Comment
            // Node found after the CommentRangeEnd node.
            if (node.NodeType == NodeType.CommentRangeEnd)
            {
                while (node.NextSibling != null && node.NodeType != NodeType.Comment)
                    node = node.NextSibling;
            }

            // Find the corresponding node in our cloned node by index and return it.
            // If the start and end node are the same some child nodes might already have been removed. Subtract the
            // Difference to get the right index.
            int indexDiff = node.ParentNode.ChildNodes.Count - cloneNode.ChildNodes.Count;

            // Child node count identical.
            if (indexDiff == 0)
                node = cloneNode.ChildNodes[node.ParentNode.IndexOf(node)];
            else
                node = cloneNode.ChildNodes[node.ParentNode.IndexOf(node) - indexDiff];

            // Remove the nodes up to/from the marker.
            bool isSkip = false;
            bool isProcessing = true;
            bool isRemoving = isStartMarker;
            Node nextNode = cloneNode.FirstChild;

            while (isProcessing && nextNode != null)
            {
                Node currentNode = nextNode;
                isSkip = false;

                if (currentNode.Equals(node))
                {
                    if (isStartMarker)
                    {
                        isProcessing = false;
                        if (isInclusive)
                            isRemoving = false;
                    }
                    else
                    {
                        isRemoving = true;
                        if (isInclusive)
                            isSkip = true;
                    }
                }

                nextNode = nextNode.NextSibling;
                if (isRemoving && !isSkip)
                    currentNode.Remove();
            }

            // After processing the composite node may become empty. If it has don't include it.
            if (!(isStartMarker && isEndMarker))
            {
                if (cloneNode.HasChildNodes)
                    nodes.Add(cloneNode);
            }

        }

        /// <summary>
        /// Step 5: "All Assembly Tasks Complete" - Final Document Cleanup
        /// </summary>
        private void AllAssemblyComplete(CreateOrEditAppJobDto jobClass, List<AssemblyWorkItem> AssemblyCollection)
        {
            List<PostProcessingItem> PostProcessingCollection = new List<PostProcessingItem>();
            AssemblyCollection.ToList().ForEach(i =>
            {
                if (i.Document != null)
                {
                    #region New Code 10512 Changes STQ Modified
                    PostProcessingItem PostProcessingWorkItem = new PostProcessingItem();
                    if (AssemblyCollection.IndexOf(i) == 0 || AsposeUtility.ShouldNotAppendDocument(i.Document))
                    {
                        PostProcessingWorkItem = GetPostProcessWorkItem(i, jobClass, AssemblyCollection);
                        
                        //Save to PostProcessingCollection
                        PostProcessingCollection.Add(PostProcessingWorkItem);
                    }
                    else
                    {
                        if ((bool)jobClass.IsAssemblyDebug)
                        {
                            PostProcessingWorkItem = GetPostProcessWorkItem(i, jobClass, AssemblyCollection);
                            
                            //Save to PostProcessingCollection
                            PostProcessingCollection.Add(PostProcessingWorkItem);
                        }
                        else
                        {
                            PostProcessingCollection.First().Document = AsposeUtility.AppendDocument(PostProcessingCollection.First().Document, i.Document);
                        }
                    }

                    #endregion
                }
            });

            PostProcessingCollection.ForEach(j =>
            {
                j.Order = PostProcessingCollection.IndexOf(j);
                //Set Continuous Section
                if (AsposeUtility.ContainsContinuousSections(j.Document))
                {
                    j.Document = AsposeUtility.SetContinuousSections(j.Document);
                }

                j.Document = AsposeUtility.JoinNextSection(j.Document);

                //Run Final Document Cleandown
                j.Document = AsposeUtility.FinalCleandown(j.Document);

                j.PdfDocument = AsposeUtility.BytesToPdf(j.Document);
                j.HTMLDocument = AsposeUtility.BytesToHTML(j.Document);
                j.Document = AsposeUtility.BytesToWord(j.Document);
            });


            //Workflow
            if (jobClass.WorkFlow != null)
            {
                if (jobClass.WorkFlow.AfterAssembly != null)
                {
                    jobClass.WorkFlow.AfterAssembly.ForEach(async m =>
                    {
                        try
                        {
                            await _APIManager.TriggerAPI(m, jobClass, PostProcessingCollection);
                        }
                        catch (Exception)
                        {
                            //TODO: Handle Exception
                        }
                    });
                }
            }

            //Save Renamed and Appeneded Documents to RecordMatterItems
            jobClass.RecordMatter.ToList().ForEach(k =>
            {

                // Get the recordGroup if new group there will 1 item with no DocumentName
                RecordMatterItem rmi = null;

                var _recordMatterItemGroupId = jobClass.RecordMatter[0].RecordMatterItemId;

                var recordId = jobClass.RecordMatter[0].RecordId;
                var recordMatterId = jobClass.RecordMatter[0].RecordMatterId;

                // If this is a project there is only every one group
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {
                        if (_projectRepository.GetAll().Any(p => p.RecordId == recordId))
                        {
                            var recordmatteritem = _recordMatterItemRepository.GetAll()
                                .Select(rmi => new { rmi.RecordMatterId, rmi.GroupId })
                                .FirstOrDefault(m => m.RecordMatterId == recordMatterId);
                            if (recordmatteritem != null)
                                _recordMatterItemGroupId = recordmatteritem.GroupId;
                        }

                    }
                    unitOfWork.Complete();
                }

                if (_recordMatterItemGroupId != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    using (var unitOfWork = _unitOfWorkManager.Begin())
                    {
                        using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                        {
                            var recordmatteritem = _recordMatterItemRepository.GetAll()
                                .Select(rmi => new { rmi.Id, rmi.GroupId })
                                .FirstOrDefault(m => m.Id == _recordMatterItemGroupId);
                            if (recordmatteritem != null)
                            {
                                _recordMatterItemGroupId = recordmatteritem.GroupId;
                            }
                        }
                        unitOfWork.Complete();
                    }
                }

                if (_recordMatterItemGroupId == null)
                    _recordMatterItemGroupId = Guid.NewGuid();

                // PostProcessingCollection contains the assembled Documents for this job
                PostProcessingCollection.ForEach(async l =>
                {

                    // Foreach recordmatter item // Create and or Update, IMPT GroupId, Status // Create or update based on Groupid and Document Name
                    // Set GroupId. If any item exists already use original groupId // GroupId can be 00000000-0000-0000-0000-000000000000
                    // Set Allowed Formats // Run Substitute on Record Names
                    var newdocumentname = substituteBrackets(l.Name ?? rmi.DocumentName, jobClass.XData.FirstOrDefault());

                    newdocumentname = ContentCleaner.RemoveHtmlJavaScriptCss(newdocumentname);

                    var RMIFormats = l.AllowWord != false ? "W," : ""; RMIFormats += l.AllowPdf != false ? "P," : ""; RMIFormats += l.AllowHTML != false ? "H," : "";
                    if (RMIFormats.EndsWith(","))
                        RMIFormats = RMIFormats.Remove(RMIFormats.Length - 1);

                    using (var unitOfWork = _unitOfWorkManager.Begin())
                    {
                        using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                        {

                            bool LockOnBuild = false;

                            var formid = jobClass.FormId ?? null;
                            var form = _formRepository.GetAll()
                            .Select(f => new { f.Id, f.LockOnBuild })
                            .FirstOrDefault(e => e.Id == formid);
                            if (form != null)
                            {
                                LockOnBuild = form.LockOnBuild;
                            }

                            rmi = _recordMatterItemRepository.FirstOrDefault(
                                m => m.GroupId == _recordMatterItemGroupId && ((m.DocumentTemplateId == l.DocumentTemplateId || m.DocumentTemplateId == null) && m.Order == l.Order));

                            l.Name = newdocumentname;

                            if (rmi == null)
                            {
                                Guid NewId = Guid.NewGuid();

                                rmi = await _recordManager.CreateAndOrFetchRecordMatterItem(new RecordMatterItem()
                                {
                                    Id = NewId,
                                    AllowedFormats = RMIFormats,
                                    AllowHtmlAssignees = l.AllowHtmlAssignees,
                                    AllowPdfAssignees = l.AllowPdfAssignees,
                                    AllowWordAssignees = l.AllowWordAssignees,
                                    Document = l.Document,
                                    DocumentName = l.Name,
                                    TenantId = k.TenantId,
                                    RecordMatterId = (Guid)k.RecordMatterId,
                                    FormId = jobClass.FormId ?? null,
                                    DocumentTemplateId = l.DocumentTemplateId,
                                    GroupId = (Guid)_recordMatterItemGroupId,
                                    UserId = null,
                                    OrganizationUnitId = k.OrganizationId,
                                    SubmissionId = k.SubmissionId,
                                    LockOnBuild = LockOnBuild,
                                    Order = l.Order,
                                    HasDocument = true
                                });
                            }
                            else
                            {

                                // Update RecordMatterItem
                                rmi.AllowHtmlAssignees = l.AllowHtmlAssignees;
                                rmi.AllowPdfAssignees = l.AllowPdfAssignees;
                                rmi.AllowWordAssignees = l.AllowWordAssignees;
                                rmi.DocumentName = l.Name;
                                rmi.Document = l.Document;
                                rmi.AllowedFormats = RMIFormats;
                                rmi.FormId = jobClass.FormId ?? null;
                                rmi.SubmissionId = k.SubmissionId;
                                rmi.LockOnBuild = LockOnBuild;
                                rmi.Order = l.Order;
                                rmi.HasDocument = true;
                                await _recordMatterItemRepository.UpdateAsync(rmi);

                            }

                            // If Status changed keep a version in History                 

                            var rm = _recordMattersRepository.GetAll()
                                .Select(rm => new { rm.Id, rm.Status })
                                .FirstOrDefault(rm => rm.Id == k.RecordMatterId);

                            var status = rm.Status.ToString();
                            if (rm.Status.ToString() != jobClass.ContributorStatus) status += " - " + jobClass.ContributorStatus;

                            var contrib = _recordMatterContributorRepository.GetAll()
                                .Select(c => new { c.Id, c.Name })
                                .FirstOrDefault(c => c.Id == jobClass.ContributorId);
                            var cname = contrib == null ? "Author" : contrib.Name;

                            var rmih = _recordMatterItemHistoryRepository.FirstOrDefault(rmih => rmih.RecordMatterItemId == rmi.Id && rmih.ContributorId == jobClass.ContributorId && rmih.Status == status);
                            // overwrite existing draft / preview items

                            if (rmih == null)
                            {

                                Guid? contributorId = null;
                                if (jobClass.ContributorId != null) contributorId = (Guid)jobClass.ContributorId;

                                rmih = new RecordMatterItemHistory()
                                {
                                    AllowedFormats = rmi.AllowedFormats,
                                    Document = rmi.Document,
                                    Name = cname,
                                    FormId = rmi.FormId,
                                    GroupId = rmi.GroupId,
                                    Id = Guid.NewGuid(),
                                    ContributorId = contributorId,
                                    CreatorUserId = jobClass.User.ID,
                                    RecordMatterItemId = rmi.Id,
                                    Status = status,
                                    DocumentName = rmi.DocumentName,
                                    SubmissionId = rmi.SubmissionId,
                                    TenantId = rmi.TenantId
                                };
                                _recordMatterItemHistoryRepository.Insert(rmih);
                            }
                            else
                            {
                                rmih.Document = rmi.Document;
                                rmih.CreationTime = DateTime.Now;
                            }

                            rmi.Status = status;

                            CurrentUnitOfWork.SaveChanges();
                            unitOfWork.Complete();

                        }
                    }

                });

                using (var unitOfWork = _unitOfWorkManager.Begin())
                {

                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {

                        // Delete Dynamically Added RecordMatterItems When inserted via a repeat
                        // Loop each group by Document Id the count should be same as the count in recordmatteritems
                        PostProcessingCollection.GroupBy(l => l.DocumentTemplateId).ToList().ForEach(g => {

                            var count = g.Count();
                            var documenttemplateid = g.Key;

                            var rmicount = _recordMatterItemRepository.Count(rmi => rmi.DocumentTemplateId.ToLower() == g.Key.ToLower() && rmi.RecordMatterId == recordMatterId);

                            if (rmicount > count)
                            {

                                var lastN = _recordMatterItemRepository.GetAll().Where(rmi => rmi.DocumentTemplateId == g.Key && rmi.RecordMatterId == recordMatterId)
                                                       .OrderByDescending(rmi => rmi.Order)
                                                       .Take(rmicount - count);

                                lastN.ToList().ForEach(r => {
                                    _recordMatterItemRepository.Delete(r);
                                }
                                );


                            }

                        });

                        CurrentUnitOfWork.SaveChanges();
                        unitOfWork.Complete();

                    }

                }

            });

            ////var _RequiresPayment = !string.IsNullOrEmpty(GetJArrayValue(SubData, "PaymentProcess")) ? true : false;
            JObject jobData = JObject.Parse(jobClass.Data);
            //decimal voucherAmount = jobData.ContainsKey("VoucherAmount") ? jobData.Value<decimal>("VoucherAmount") : 0;
            _submissionManager.UpdateSubmissionStatus(new CreateOrEditSubmissionDto()
            {
                Id = jobClass.RecordMatter.First().SubmissionId,
                UserId = jobClass.User.ID,
                TenantId = jobClass.TenantId,
                FormId = jobClass.FormId != null ? new Guid(Convert.ToString(jobClass.FormId)) : (Guid?)null,
                AppId = jobClass.AppId != null ? new Guid(Convert.ToString(jobClass.AppId)) : (Guid?)null,
                RecordId = jobClass.RecordMatter.First().RecordId,
                RecordMatterId = jobClass.RecordMatter.First().RecordMatterId,
                SubmissionStatus = "Complete",
                // VoucherAmount = voucherAmount
            });

            FinalWorkflow(jobClass, PostProcessingCollection);
            DeleteRecords(jobClass);

            // UNlock the File
            // TODO THIS DOES NOT WORK IF MOVED TO RECORD MANAGER
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    try
                    {
                        if (jobClass.RecordMatter.First().RecordId.HasValue)
                        {
                            var ir = jobClass.RecordMatter.First().RecordId;
                            var grid = (Guid)jobClass.RecordMatter.First().RecordId;

                            var record = _recordsRepository.GetAll().FirstOrDefault(r => r.Id == grid);
                            if (record != null)
                            {
                                record.Locked = null;
                            }

                            CurrentUnitOfWork.SaveChanges();
                            unitOfWork.Complete();
                        }
                    }
                    catch { }
                }
            }

            //everytime when document is assembled, it will trigger webhook event
            //puiblish webhook to specific tenant
            SendWebhooks(jobClass);
        }

        private PostProcessingItem GetPostProcessWorkItem(AssemblyWorkItem i, CreateOrEditAppJobDto jobClass, List<AssemblyWorkItem> AssemblyCollection)
        {
            PostProcessingItem PostProcessingWorkItem = new PostProcessingItem();
            PostProcessingWorkItem.DocumentId = i.DocumentId;
            if (AsposeUtility.ContainsDocumentName(i.Document))
            {
                var result = AsposeUtility.DocumentName(i.Document);
                if (result.Item1 != null && result.Item2 != null)
                {
                    i.Document = result.Item1;
                    PostProcessingWorkItem.Name = string.IsNullOrEmpty(result.Item2) ? "Document" : result.Item2;
                } //Else??
            }
            else
            {
                dynamic testing = JsonConvert.DeserializeObject(JsonConvert.SerializeXNode(i.Data));
                PostProcessingWorkItem.Name = testing.File.System.Document.DocumentName;
            }


            //Remove Do Not Append Flag
            i.Document = AsposeUtility.RemoveDoNotAppend(i.Document);

            //Set Document Bytes
            PostProcessingWorkItem.Document = i.Document;

            //Transfer Allowed Formats
            var DocId = (Int32.Parse(i.DocumentId) - 1) < 0 ? 0 : (Int32.Parse(i.DocumentId) - 1);

            var jobClassDoc = jobClass.Document.FirstOrDefault(n => Convert.ToString(n.DocumentTemplateURL) == i.DocumentTemplateId);

            if (jobClassDoc != null)
            {
                PostProcessingWorkItem.Order = AssemblyCollection.IndexOf(i); //  jobClassDoc.DocumentId;
                PostProcessingWorkItem.DocumentTemplateId = jobClassDoc.DocumentTemplateURL;
                PostProcessingWorkItem.AllowWord = jobClassDoc.AllowWord.GetValueOrDefault(false);
                PostProcessingWorkItem.AllowPdf = jobClassDoc.AllowPdf.GetValueOrDefault(false);
                PostProcessingWorkItem.AllowHTML = jobClassDoc.AllowHTML.GetValueOrDefault(false);

                if (jobClassDoc.AllowWordAssignees != null)
                {
                    PostProcessingWorkItem.AllowWordAssignees = Newtonsoft.Json.JsonConvert.SerializeObject(jobClassDoc.AllowWordAssignees);
                }

                if (jobClassDoc.AllowPdfAssignees != null)
                {
                    PostProcessingWorkItem.AllowPdfAssignees = Newtonsoft.Json.JsonConvert.SerializeObject(jobClassDoc.AllowPdfAssignees);
                }

                if (jobClassDoc.AllowHtmlAssignees != null)
                {
                    PostProcessingWorkItem.AllowHtmlAssignees = Newtonsoft.Json.JsonConvert.SerializeObject(jobClassDoc.AllowHtmlAssignees);
                }
            }
            return PostProcessingWorkItem;
        }

        private void SendWebhooks(CreateOrEditAppJobDto jobClass)
        {
            if (jobClass.SendWebhooksAfterAssembly == true)
            {
                _webHookPublisher.PublishAsync(AppWebHookNames.DocumentsGenerated,
               new CreateOrEditSubmissionDto()
               {
                   Id = jobClass.RecordMatter.First().SubmissionId,
                   UserId = jobClass.User.ID,
                   TenantId = jobClass.TenantId,
                   FormId = jobClass.FormId != null ? new Guid(Convert.ToString(jobClass.FormId)) : (Guid?)null,
                   AppId = jobClass.AppId != null ? new Guid(Convert.ToString(jobClass.AppId)) : (Guid?)null,
                   RecordId = jobClass.RecordMatter.First().RecordId,
                   RecordMatterId = jobClass.RecordMatter.First().RecordMatterId,
                   //Type
                   SubmissionStatus = "Complete"
               },
               jobClass.TenantId
               );
            }

        }

        private void FinalWorkflow(CreateOrEditAppJobDto jobClass, List<PostProcessingItem> DocCollection)
        {
            if (jobClass.WorkFlow?.Email != null)
            {
                jobClass.WorkFlow.Email.ForEach(async i =>
                {

                    JObject keyData = JObject.Parse("{Data:" + jobClass.Data + "}");
                    XmlNode node = null;
                    if (!string.IsNullOrEmpty(i.FilterRule))
                    {
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(Convert.ToString(JsonConvert.DeserializeXNode(Convert.ToString(keyData))));
                            node = doc.DocumentElement.SelectSingleNode(i.FilterRule);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        node = new XmlDocument();
                    }

                    if (node != null || string.IsNullOrEmpty(i.FilterRule))
                    {
                        try
                        {
                            string emailTemplate = string.Empty;
                            switch (i.EmailType)
                            {
                                case "From HTML source":
                                    string url = i.EmailTemplateURL ?? string.Empty;
                                    if (IsValidUri(url))
                                    {
                                        try
                                        {
                                            using (var client = new HttpClient())
                                            {
                                                // Download Template.
                                                emailTemplate = await client.GetStringAsync(url);
                                                if (emailTemplate != null && i.TemplatePartID != "")
                                                {
                                                    var document = new HtmlParser().ParseDocument(emailTemplate);
                                                    emailTemplate = Convert.ToString(document.All.Where(e => e.Id == Convert.ToString(i.TemplatePartID)).ElementAt(0).OuterHtml);
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Carry on
                                        }
                                    }
                                    break;
                                case "Plain Email Body":
                                    emailTemplate = i.DocumentEmailMessage;
                                    break;
                                case "From Assembled Document":
                                    DocCollection.ForEach(m =>
                                    {
                                        string[] iDs = i.EmailBodyDocumentIds.Split(',');
                                        if (iDs.Any(n => n == m.DocumentId) || string.IsNullOrEmpty(i.EmailBodyDocumentIds))
                                        {
                                            MemoryStream stream = new MemoryStream(m.HTMLDocument);
                                            //var fname = Path.GetFileNameWithoutExtension("c:\\temp\\" + m.Name) + ".html";
                                            //var html = new Attachment(stream, fname);

                                            //string _FilePath = Path.GetFileNameWithoutExtension("c:\\temp\\" + m.Name) + ".html";
                                            StreamReader sr = new StreamReader(stream);
                                            string body = sr.ReadToEnd();

                                            emailTemplate += body;
                                        }
                                    });
                                    break;
                            }

                            bool InsertTableStyle = false;
                            if (emailTemplate.Contains("{{MyUserDataSummary}}"))
                            {
                                emailTemplate = emailTemplate.Replace("{{MyUserDataSummary}}", jobClass.SummaryTableHTML);
                                if (!string.IsNullOrEmpty(jobClass.SummaryTableHTML))
                                    InsertTableStyle = true;
                            }

                            XElement Data = jobClass.XData.FirstOrDefault();
                            Regex Pattern = new Regex("{.*?}", RegexOptions.IgnoreCase); //TODO: Revert to {{}}
                            var matches = Pattern.Matches(emailTemplate);
                            matches.ToList().ForEach(i1 =>
                            {
                                emailTemplate = XMLUtility.ReplaceBracketValuesXML(i1, Data, emailTemplate);
                            });

                            matches = Pattern.Matches(i.DocumentEmailSubject);
                            matches.ToList().ForEach(i2 =>
                            {
                                i.DocumentEmailSubject = XMLUtility.ReplaceBracketValuesXML(i2, Data, i.DocumentEmailSubject);
                            });

                            // 
                            matches = Pattern.Matches(i.DocumentEmailSubject);
                            matches.ToList().ForEach(i1 =>
                            {
                                i.DocumentEmailSubject = XMLUtility.ReplaceBracketValuesXML(i1, Data, i.DocumentEmailSubject);
                            });

                            if (InsertTableStyle)
                            {
                                emailTemplate += "<style>.table{width: 100%;margin-bottom: 0;background-color: transparent;color: #212529;border-collapse: collapse}.table-bordered{border: 2px solid #f4f5f8}.table-bordered thead th{border-bottom-width: 2px}.table thead th{vertical-align: bottom;border-bottom: 2px solid #dee2e6}.table thead th, .table thead td{font-weight: 500;padding-top: 1rem;padding-bottom: 1rem}.table-striped tbody tr:nth-of-type(odd){background-color: rgba(0,0,0,.05)}.table-bordered td, .table-bordered th{border: 1px solid #dee2e6}.table td, .table th{padding: .75rem;vertical-align: top}</style>";
                            }

                            MailMessage mail = new MailMessage
                            {
                                Subject = i.DocumentEmailSubject,
                                Body = emailTemplate,
                                IsBodyHtml = true
                            };


                            //substitute wildcards for field values                                    
                            matches = Pattern.Matches(i.DocumentEmailRecipients);
                            matches.ToList().ForEach(i1 =>
                            {
                                i.DocumentEmailRecipients = XMLUtility.ReplaceBracketValuesXML(i1, Data, i.DocumentEmailRecipients);
                            });
                            i.DocumentEmailRecipients = i.DocumentEmailRecipients.Trim();

                            i.DocumentEmailRecipients.Split(';').ToList().ForEach(j =>
                            {
                                if (j != "")
                                {
                                    //substitute wildcards for field values                                    
                                    matches = Pattern.Matches(j);
                                    matches.ToList().ForEach(i1 =>
                                    {
                                        j = XMLUtility.ReplaceBracketValuesXML(i1, Data, j);
                                    });
                                    j = j.Trim();
                                    if (RegexUtility.IsValidEmail(j))
                                    {
                                        mail.To.Add(new MailAddress(j));
                                    }
                                }
                            });

                            //substitute wildcards for field values                                    
                            matches = Pattern.Matches(i.DocumentEmailCCRecipients);
                            matches.ToList().ForEach(i1 =>
                            {
                                i.DocumentEmailCCRecipients = XMLUtility.ReplaceBracketValuesXML(i1, Data, i.DocumentEmailCCRecipients);
                            });
                            i.DocumentEmailCCRecipients = i.DocumentEmailCCRecipients.Trim();

                            i.DocumentEmailCCRecipients.Split(';').ToList().ForEach(k =>
                            {
                                if (k != "")
                                {
                                    matches = Pattern.Matches(k);
                                    matches.ToList().ForEach(i1 =>
                                    {
                                        k = XMLUtility.ReplaceBracketValuesXML(i1, Data, k);
                                    });
                                    k = k.Trim();
                                    if (RegexUtility.IsValidEmail(k))
                                    {
                                        mail.CC.Add(new MailAddress(k));
                                    }

                                }
                            });


                            //substitute wildcards for field values                                    
                            matches = Pattern.Matches(i.DocumentEmailBCCRecipients);
                            matches.ToList().ForEach(i1 =>
                            {
                                i.DocumentEmailBCCRecipients = XMLUtility.ReplaceBracketValuesXML(i1, Data, i.DocumentEmailBCCRecipients);
                            });
                            i.DocumentEmailBCCRecipients = i.DocumentEmailBCCRecipients.Trim();

                            i.DocumentEmailBCCRecipients.Split(';').ToList().ForEach(l =>
                            {
                                if (l != "")
                                {
                                    matches = Pattern.Matches(l);
                                    matches.ToList().ForEach(i1 =>
                                    {
                                        l = XMLUtility.ReplaceBracketValuesXML(i1, Data, l);
                                    });
                                    l = l.Trim();
                                    if (RegexUtility.IsValidEmail(l))
                                    {
                                        mail.Bcc.Add(new MailAddress(l));
                                    }
                                }
                            });

                            i.DocumentEmailFrom = i.DocumentEmailFrom.Trim();
                            if (RegexUtility.IsValidEmail(i.DocumentEmailFrom))
                            {
                                mail.From = new MailAddress(i.DocumentEmailFrom);
                            }


                            if (!string.IsNullOrEmpty(i.DocumentAttachmentIds))
                            {

                                string[] iDs = i.DocumentAttachmentIds.Split(',');

                                // Two formats supported
                                // v1 = 1
                                // v2 = 1|docname.pdf

                                var isversion1 = int.TryParse(iDs[0], out int i1);
                                if (isversion1)
                                {
                                    // v1 Format 
                                    // i.e. 1 numeric only
                                    foreach (string docitem in iDs)
                                    {
                                        DocCollection.Where(d => d.DocumentId == docitem).ToList().ForEach(doc =>
                                        {
                                            if (doc != null)
                                            {
                                                if (doc.AllowWord && doc.Document != null)
                                                {
                                                    MemoryStream stream = new MemoryStream(doc.Document);
                                                    var fname = Path.GetFileNameWithoutExtension("c:\\temp\\" + doc.Name) + ".docx";
                                                    mail.Attachments.Add(new Attachment(stream, fname));
                                                }
                                                if (doc.AllowPdf && doc.PdfDocument != null)
                                                {
                                                    MemoryStream stream = new MemoryStream(doc.PdfDocument);
                                                    var fname = Path.GetFileNameWithoutExtension("c:\\temp\\" + doc.Name) + ".pdf";
                                                    mail.Attachments.Add(new Attachment(stream, fname));
                                                }
                                                if (doc.AllowHTML && doc.HTMLDocument != null)
                                                {
                                                    MemoryStream stream = new MemoryStream(doc.HTMLDocument);
                                                    var fname = Path.GetFileNameWithoutExtension("c:\\temp\\" + doc.Name) + ".html";
                                                    mail.Attachments.Add(new Attachment(stream, fname));
                                                }
                                            }
                                        });
                                    }
                                }
                                else
                                { 
                                  
                                    int j = 0;

                                    // foreach (var documentItem in jobClass.Document)
                                    // Only loop the output Id's not the original collection
                                    // as they might have been consolidated
                                    foreach (string docitem in iDs)
                                    {
                                        string[] docitems = iDs[j].Split('|');
                                        if (docitems.Count() > 1)
                                        {
                                            var docitemid = Convert.ToInt16(docitems[0]);
                                            var documentItem = jobClass.Document.FirstOrDefault(d => d.DocumentId == docitemid);
                                            if (documentItem != null)
                                            {
                                                DocCollection.Where(d => d.DocumentTemplateId == documentItem.DocumentTemplateURL).ToList().ForEach(doc =>
                                                {
                                                    if (doc != null)
                                                    {
                                                        var docitemname = docitems[1];
                                                        docitemname = substituteBrackets(docitemname, Data);
                                                        var ext = System.IO.Path.GetExtension(docitemname);

                                                        docitemname = doc.Name + ext;
                                                        byte[] docbyte = doc.Document;
                                                        if (ext.ToLower() == ".pdf") docbyte = doc.PdfDocument;
                                                        if (ext.ToLower() == ".html") docbyte = doc.HTMLDocument;
                                                        if (ext.ToLower() == ".docx") docbyte = doc.Document;

                                                        MemoryStream stream = new MemoryStream(docbyte);
                                                        var fname = Path.GetFileNameWithoutExtension("c:\\temp\\" + docitemname);
                                                        mail.Attachments.Add(new Attachment(stream, docitemname));

                                                    }
                                                });
                                            }
                                        }
                                        j++;
                                    } 
                                }
                            }

                            // 1. Attach all other recordmatteritems for this record
                            if (Convert.ToBoolean(i.AttachAllRecordMatterItems))
                            {
                                //FileUploadAttachements(jobClass.RecordMatter.First()).ForEach(f => {
                                //    mail.Attachments.Add(f);
                                //});

                                foreach (CreateOrEditAppJobRecordMatterDto rm in jobClass.RecordMatter)
                                {

                                    using (var unitOfWork = _unitOfWorkManager.Begin())
                                    {
                                        using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                                        {

                                            // get each rmi
                                            _recordMatterItemRepository.GetAll().Where(rmi => rmi.RecordMatterId == rm.RecordMatterId).ToList().ForEach(rmi => {

                                                if (rmi.HasDocument)
                                                {
                                                    if (!(jobClass.Document.Any(d => d.DocumentName.Contains(rmi.DocumentName))))
                                                    {
                                                        MemoryStream stream = new MemoryStream(rmi.Document);
                                                        var fname = Path.GetFileNameWithoutExtension("c:\\temp\\" + rmi.DocumentName);
                                                        mail.Attachments.Add(new Attachment(stream, rmi.DocumentName));
                                                    }
                                                }

                                            });

                                        }
                                    }
                                }

                            }

                            // Attach file uplaods if required
                            if (Convert.ToBoolean(i.AttachFileUploads))
                            {
                                FileUploadAttachements(jobClass.RecordMatter.First()).ForEach(f => {
                                    mail.Attachments.Add(f);
                                });
                            }

                            _AbpSession.Use(jobClass.TenantId, null);
                            await _emailSender.SendAsync(mail);

                            //TenantEmailSettingsEditDto emailsettings = GetEmailSettingsAsync(jobClass.TenantId).Result;
                            //if (emailsettings.UseHostDefaultEmailSettings)
                            //{
                            //    await _emailSender.SendAsync(mail);
                            //}
                            //else
                            //{
                            //    mail.From = mail.From == null ? new MailAddress(emailsettings.DefaultFromAddress, emailsettings.DefaultFromDisplayName) : mail.From;
                            //    SmtpClient client = new SmtpClient(emailsettings.SmtpHost);
                            //    client.Port = emailsettings.SmtpPort;
                            //    client.EnableSsl = emailsettings.SmtpEnableSsl;
                            //    client.Credentials = new NetworkCredential(emailsettings.SmtpUserName, emailsettings.SmtpPassword);
                            //    client.Send(mail);
                            //}


                        }
                        catch (Exception ex)
                        {
                            //TODO: Handle Exception here
                        }
                    }
                });
            }
        }

        //private async Task<TenantEmailSettingsEditDto> GetEmailSettingsAsync(int? tenantId)
        //{

        //    if (tenantId == null)
        //    {
        //        return new TenantEmailSettingsEditDto
        //        {
        //            UseHostDefaultEmailSettings = true
        //        };
        //    }
        //    else
        //    {
        //        var smtpPassword = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Password, (int)tenantId);
        //        smtpPassword = SimpleStringCipher.Instance.Decrypt(smtpPassword);

        //        return new TenantEmailSettingsEditDto
        //        {
        //            UseHostDefaultEmailSettings = false,
        //            DefaultFromAddress = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.DefaultFromAddress, (int)tenantId),
        //            DefaultFromDisplayName = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.DefaultFromDisplayName, (int)tenantId),
        //            SmtpHost = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Host, (int)tenantId),
        //            SmtpPort = Convert.ToInt16(await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Port, (int)tenantId)),
        //            SmtpUserName = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.UserName, (int)tenantId),
        //            SmtpPassword = smtpPassword,
        //            SmtpDomain = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Domain, (int)tenantId),
        //            SmtpEnableSsl = Convert.ToBoolean(await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.EnableSsl, (int)tenantId)),
        //            SmtpUseDefaultCredentials = Convert.ToBoolean(await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.UseDefaultCredentials, (int)tenantId))
        //        };
        //    }

        //}

        private async Task DeleteRecords(CreateOrEditAppJobDto jobClass)
        {

            if ((bool)jobClass.DeleteRecordsAfterAssembly)
            {

                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                    {

                        _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete);

                        foreach (CreateOrEditAppJobRecordMatterDto recordmatter in jobClass.RecordMatter)
                        {

                            var submissionaclids = _aclRepository.GetAll().Where(e => e.EntityID == (Guid)recordmatter.SubmissionId).Select(n => n.Id).ToList();
                            foreach (var aclid in submissionaclids)
                            {
                                _aclRepository.Delete(aclid);
                            }

                            _submissionsRepository.Delete((Guid)recordmatter.SubmissionId);

                            var recordmatteritemids = _recordMatterItemRepository.GetAll().Where(e => e.GroupId == recordmatter.RecordMatterItemGroupId).Select(n => n.Id).ToList();
                            foreach (var id in recordmatteritemids)
                            {
                                var recordmatteritemaclids = _aclRepository.GetAll().Where(e => e.EntityID == id).Select(n => n.Id).ToList();
                                foreach (var aclid in recordmatteritemaclids)
                                {
                                    _aclRepository.Delete(aclid);
                                }

                                _recordMatterItemRepository.Delete(id);

                            }

                            var recordmatterids = _recordMattersRepository.GetAll().Where(e => e.RecordId == recordmatter.RecordId).Select(n => n.Id).ToList();
                            foreach (var id in recordmatterids)
                            {

                                var recordmatteraclids = _aclRepository.GetAll().Where(e => e.EntityID == id).Select(n => n.Id).ToList();
                                foreach (var aclid in recordmatteraclids)
                                {
                                    _aclRepository.Delete(aclid);
                                }

                                _recordMattersRepository.Delete(id);
                            }

                            var recordaclids = _aclRepository.GetAll().Where(e => e.EntityID == (Guid)recordmatter.RecordId).Select(n => n.Id).ToList();
                            foreach (var aclid in recordaclids)
                            {
                                _aclRepository.Delete(aclid);
                            }

                            _recordsRepository.Delete((Guid)recordmatter.RecordId);

                        }


                    }
                    unitOfWork.Complete();
                }


            }
        }
        private string substituteBrackets(string input, XElement Data)
        {

            Regex Pattern = new Regex("{.*?}", RegexOptions.IgnoreCase); //TODO: Revert to {{}}
            var matches = Pattern.Matches(input);
            matches.ToList().ForEach(i1 =>
            {
                input = XMLUtility.ReplaceBracketValuesXML(i1, Data, input);
            });

            return input;
        }

        private List<Attachment> FileUploadAttachements(CreateOrEditAppJobRecordMatterDto recordmatter)
        {

            var result = new List<Attachment>();

            List<GetFileForViewDto> FilesForView = new List<GetFileForViewDto>();
            MemoryStream memoryStream = new MemoryStream();

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnection.Value.ConnectionString);
            string cloudStorageAccountTable = _storageConnection.Value.BlobStorageContainer;

            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(cloudStorageAccountTable);
            IEnumerable<IListBlobItem> blobs = container.ListBlobs("file-uploads" + "/" + recordmatter.RecordId + "/" + recordmatter.RecordMatterId + "/" + recordmatter.RecordMatterItemGroupId + "/", false);

            List<string> blobNames = blobs.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();

            blobNames.ForEach(i =>
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(i.ToString());
                if (blockBlob.Exists())
                {
                    MemoryStream memStream = new MemoryStream();
                    blockBlob.DownloadToStream(memStream);
                    byte[] FileByte = memStream.ToArray();
                    var fname = Path.GetFileName(i);
                    Attachment attachment = new Attachment(new MemoryStream(FileByte), fname);
                    result.Add(attachment);
                }

            });

            return result;

        }

        Boolean IsValidUri(string uri)
        {
            try
            {
                new Uri(uri);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}