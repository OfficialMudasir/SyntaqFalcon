using Aspose.Words;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

namespace Syntaq.Falcon.Assembly
{
    public static class Assembly
    {

        static TraceWriter _log = null;
        static string _sqlConnection = "Server=localhost; Database=FalconDb; Trusted_Connection=True;";
        //static string _sqlConnection = "Server=syntaq-au-e.database.windows.net,1433;Database=SyntaqFalcon_AU_88;Persist Security Info=False;User ID=syntaq_admin;Password=WV*t4ijBPF#%R04Jw@kz@YBn0;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //static string _sqlConnection = "Server=syntaq-au-e.database.windows.net,1433;Database=SyntaqFalcon_AU_Test;Persist Security Info=False;User ID=syntaq_admin;Password=WV*t4ijBPF#%R04Jw@kz@YBn0;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //static string _sqlConnection = "Server=brm-builder-ae-sql-pre.database.windows.net,1433;Database=brm-builder-ae-sdb-pre;Persist Security Info=False;User ID=syntaq_admin;Password=H25ybbo8fLRS*F2U#3fV;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //static string _sqlConnection = "Server=brm-rfx-builder-ae-sql-prd.database.windows.net,1433;Database=brm-rfx-builder-ae-sdb-prd;Persist Security Info=False;User ID=syntaq_admin;Password=&V4q^x!0pqDzCTo*!JSeGjCO@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"; // ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        //static string _sqlConnection = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        /// <summary>
        /// Document Assembly "Blackbox" Azure Function
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("Assemble")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req, TraceWriter log)
        {
            _log = log;

            //_sqlConnection = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            _sqlConnection = "Server=localhost; Database=FalconDb; Trusted_Connection=True;";
            //_sqlConnection = "Server=syntaq-au-e.database.windows.net,1433;Database=SyntaqFalcon_AU_88;Persist Security Info=False;User ID=syntaq_admin;Password=WV*t4ijBPF#%R04Jw@kz@YBn0;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //_sqlConnection = "Server=brm-builder-ae-sql-pre.database.windows.net,1433;Database=brm-builder-ae-sdb-pre;Persist Security Info=False;User ID=syntaq_admin;Password=H25ybbo8fLRS*F2U#3fV;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //_sqlConnection = "Server=brm-rfx-builder-ae-sql-prd.database.windows.net,1433;Database=brm-rfx-builder-ae-sdb-prd;Persist Security Info=False;User ID=syntaq_admin;Password=&V4q^x!0pqDzCTo*!JSeGjCO@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


            log.Info("C# HTTP trigger function processed an assembly request.");

            License license = new License();
            license.SetLicense("Aspose.Words.lic");

            Document doc = null;
            MemoryStream outStream = null;

            var RequestContent = await req.Content.ReadAsMultipartAsync();

            XElement formdata = await RequestContent.Contents[0].ReadAsAsync<XElement>(); // XML DATA
            var Key = RequestContent.Contents[1].ReadAsStringAsync().Result;

            // Get the token from here too
            // WEB Url or direct to DB
            Byte[] bytedoc = getDocumentTemplate(formdata);

            _log.Info("Loaded Document");

            if (bytedoc == null)
            {
                doc = new Document();
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.Bold = true;
                builder.Font.Color = Color.Red;
                builder.Font.Size = 18;
                builder.Font.Name = "Verdana";
                builder.Writeln("There has been a problem assembling your document template.");
                builder.Writeln("");
                builder.Bold = false;
                builder.Font.Size = 12;
                builder.Writeln("This Document template does not exist or you do not have access permission.");
                //builder.Writeln(url);
                builder.Writeln("");

                //builder.Font.Size = 10;
                //builder.Font.Name = "Verdana";
                //builder.Writeln("");
                //builder.Writeln("Support site");
                //builder.Font.Color = Color.Blue;
                //builder.Font.Underline = Underline.Single;
                //builder.Writeln("https://syntaq.zendesk.com/hc/en-us");

                outStream = new MemoryStream();
                doc.Save(outStream, SaveFormat.Docx);
                bytedoc = outStream.ToArray();
            }

            doc = new Document(new MemoryStream(bytedoc));

            var docassembly = new WordFusion.Assembly.Assembler
            {
                IsLocalAssembly = false,
                EvaluateIF = true
            };

            try
            {
                var xuserdataid = formdata.Element("MyUserDataId");
                if (xuserdataid != null)
                {
                    var userId = string.IsNullOrEmpty(xuserdataid.Value) ? 0 : Convert.ToInt16(xuserdataid.Value);
                    docassembly.UserLogo = getUserLogoFromSQL(Convert.ToInt32(userId));
                }

            }
            catch (Exception ex)
            {// Continue
                _log.Info(ex.Message);
            }

            // Used for the Insert Function to Build URL for Document Template
            if (RequestContent.Contents.Count >= 4)
            {
                var RequestAuthority = RequestContent.Contents[3].ReadAsStringAsync().Result;
                docassembly.RequestAuthority = RequestAuthority;
            }

            // Check for valid MText XML
            if (RequestContent.Contents.Count > 2)
            {

                try
                {

                    XElement xe = await RequestContent.Contents[2].ReadAsAsync<XElement>(); // XML DATA
                    string mText = xe.ToString(); // XMl MERGETEXT
                    if (TryParseXml(mText))
                    {
                        System.Data.DataSet dsMText = new System.Data.DataSet();
                        System.IO.StringReader xmlSR = new System.IO.StringReader(mText);
                        dsMText.ReadXml(xmlSR, XmlReadMode.IgnoreSchema);
                        docassembly.DataMergeText = dsMText;
                    }
                }
                catch (Exception ex)
                {
                    log.Info(ex.Message);
                }
            }

            docassembly.InsertSchedules = true;
            docassembly.UpdateFields = false;


            _log.Info("Start Document Assembly");

            string sformdata = formdata.ToString();

            // ErrorAdd bookmarks for splitting document in post processing

            doc = docassembly.Assemble(doc, sformdata);

            outStream = new MemoryStream();
            doc.Save(outStream, SaveFormat.Docx);

            DocumentAssemblyResponse DocumentAssemblyResponse = new DocumentAssemblyResponse { Key = Key };
            //DocumentAssemblyResponse.Error = new string[];
            List<string> stringErrors = new List<string>();
            //var i = 0;
            foreach (Exception Error in docassembly.Errors)
            {
                stringErrors.Add(Error.Message.ToString());
                //.SetValue(Error.Message.ToString(), i);
                //i++;
            }
            DocumentAssemblyResponse.Error = stringErrors.ToArray();
            //DocumentAssemblyResponse.Error = docassembly.Errors.;
            DocumentAssemblyResponse.Document = Convert.ToBase64String(outStream.ToArray());

            string output = JsonConvert.SerializeObject(DocumentAssemblyResponse, Formatting.Indented);

            HttpResponseMessage ResponseContent = new HttpResponseMessage
            {
                Content = new StringContent(output)
            };


            _log.Info("Returning Document");
            return ResponseContent;
        }

        private static byte[] getDocumentTemplate(XElement formData)
        {

            string url = formData.Element("System").Element("Document").Element("DocumentTemplateURL").Value.ToString();
            var tenantId = string.IsNullOrEmpty(formData.Element("System").Element("Document").Element("TenantId").Value) ? 0 : Convert.ToInt16(formData.Element("System").Element("Document").Element("TenantId").Value);
            string authtoken = string.Empty;

            if (formData.Element("AccessToken") != null)
                authtoken = Convert.ToString(formData.Element("AccessToken").Value);



            Byte[] bytedoc = null;
            try{

                string pattern = @"([a-z0-9]{8}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{4}[-][a-z0-9]{12})";

                MatchCollection mc;
                mc = Regex.Matches(url, pattern);
                if (mc.Count == 1)
                {
                    string paramversion = "live";
                    var elements = url.Split('&');
                    if (elements.Length > 0)
                    {
                        if (elements.Length > 1)
                        {
                            var versionelements = elements[1].Split('=');
                            if (versionelements.Length > 0)
                            {
                                paramversion = versionelements[1];
                            }
                        }
                    }

                    var projectId = formData.Element("ProjectId")?.Value;

                    if (string.IsNullOrEmpty(projectId))
                    {
                        bytedoc = getDocumentFromSQL(tenantId, mc[0].Value, paramversion, authtoken);
                    }
                    else
                    {
                        bytedoc = getDocumentFromRelease(projectId, mc[0].Value, paramversion, authtoken);
                    }

                }

                if (bytedoc == null)
                {
                    bytedoc = getDocumentFromWeb(url);
                }

            }
            catch (Exception ex){
                _log.Info(ex.Message);
            }

            return bytedoc;

        }

        private static Byte[] getDocumentFromWeb(string url)
        {

            Byte[] bytedoc = null;
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result)
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        // Download data.
                        bytedoc = new WebClient().DownloadData(url);
                    }
                    catch (Exception ex)
                    {
                        _log.Info("Null Document");
                        bytedoc = null;
                    }
                }
            }

            return bytedoc;

        }

        public static Byte[] getDocumentFromSQL(int? tenantId, string originalid, string version, string authtoken)
        {

            Byte[] result = null;
            var locktotenant = false;
            int? doctenantId = null;
            using (SqlConnection myConnection = new SqlConnection(_sqlConnection))
            {

                string cmdtext = "Select top 1 * from sfaDocumentTemplates where OriginalId = @OriginalId ";
                int n; bool isNumeric = int.TryParse(version, out n);
                if (version.ToLower() == "live" || string.IsNullOrEmpty(version) || !isNumeric){
                    cmdtext += " and [Version] = CurrentVersion";
                }
                else{
                    cmdtext += " and [Version] = " + version;
                }

                SqlCommand oCmd = new SqlCommand(cmdtext, myConnection);
                oCmd.Parameters.AddWithValue("@OriginalId", originalid);
                myConnection.Open();

                using (SqlDataReader oReader = oCmd.ExecuteReader()){
                    while (oReader.Read()){
                        locktotenant = (bool)oReader["LockToTenant"];
                        doctenantId = oReader["TenantId"] as int? ?? null;
                        tenantId = tenantId == 0 ? null : tenantId;
                        result = (byte[])oReader["Document"];
                    }
                }
                
                // Check Contributor AuthTokenAccess
                SqlCommand oCmdRMC = new SqlCommand("Select count(*) from sfaRecordMatterContributors where AccessToken = @AccessToken ", myConnection);
                oCmdRMC.Parameters.AddWithValue("@AccessToken", authtoken);
                Int32 cntRMC = (Int32)oCmdRMC.ExecuteScalar();

                if (locktotenant && doctenantId != tenantId && cntRMC == 0)
                { 
                    result = null; 
                }

                myConnection.Close();

            }

            return result;
        }

        public static Byte[] getDocumentFromRelease(string projectid, string documentId, string version, string authtoken)
        {
            Byte[] result = null;

            using (SqlConnection myConnection = new SqlConnection(_sqlConnection))
            {

                // Get Project
                string cmdproject = "Select top 1 * from sfaProjects where Id = @ProjectId";
                SqlCommand oCmdProject = new SqlCommand(cmdproject, myConnection);
                oCmdProject.Parameters.AddWithValue("@ProjectId", projectid);
                myConnection.Open();

                Guid releaseId = Guid.Empty;

                using (SqlDataReader oReader = oCmdProject.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        releaseId = (Guid)oReader["ReleaseId"];
                    }
                }

                // Get ProjectRelease
                string cmdtext = "Select top 1 * from sfaProjectReleases where Id = @ReleaseidId ";
                SqlCommand oCmd = new SqlCommand(cmdtext, myConnection);
                oCmd.Parameters.AddWithValue("@ReleaseidId", releaseId);                
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {

                        Byte[] artifact = (byte[])oReader["Artifact"];

                        using (var stream = new MemoryStream(artifact)) 
                        using (var archive = new System.IO.Compression.ZipArchive(stream))
                        {
                            var zipentry = archive.GetEntry("Project.json");
                            using (StreamReader reader = new StreamReader(zipentry.Open()))
                            {
                                var projectjson = reader.ReadToEnd();

                                ImportProjectInput input = new ImportProjectInput()
                                {
                                    Project = projectjson
                                };
                                var pt = JsonConvert.DeserializeObject<ProjectExport>(input.Project);

                                if (pt.Templates != null)
                                {                               
                                    var t = pt.Templates.First(f => f.Id == Guid.Parse(documentId));
                                    result = t.Document;
                                }

                            }
                        }

                    }
                }

                myConnection.Close();

            }

            return result;
        }

        public static Image getUserLogoFromSQL(int userid)
        {

            Image result = null;
            using (SqlConnection myConnection = new SqlConnection(_sqlConnection))
            {
                myConnection.Open();

                string cmdtext = "Select LogoPictureId From AbpUsers where id=@userid";
                SqlCommand oCmd = new SqlCommand(cmdtext, myConnection);
                oCmd.Parameters.AddWithValue("@userid", userid);
                var LogoPictureId = (Guid)oCmd.ExecuteScalar();


                Byte[] imgbyte = null;

                cmdtext = "Select Bytes From AppBinaryObjects where id=@id";
                oCmd = new SqlCommand(cmdtext, myConnection);
                oCmd.Parameters.AddWithValue("@id", LogoPictureId);
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        imgbyte = (byte[])oReader["Bytes"];
                    }
                }
                myConnection.Close();

                if (imgbyte != null)
                {
                    using (var ms = new MemoryStream(imgbyte))
                    {
                        result = Image.FromStream(ms);
                    }

                }
            }

            return result;
        }

        private static bool TryParseXml(string xml)
        {
            try
            {
                XElement.Parse(xml);
                return true;
            }
            catch
            {

                return false;
            }
        }

        public class DocumentAssemblyResponse
        {
            public string Key { get; set; }
            public string[] Error { get; set; }
            public string Document { get; set; }
        }
    }

    public class ImportProjectInput
    {
        public string Project { get; set; }
    }

    class ProjectExport
    {
        public List<Template> Templates { get; set; } = new List<Template>();
    }

    public class Template
    {
        public virtual Guid Id { get; set; }
        public int? TenantId { get; set; }
        public virtual Guid OriginalId { get; set; }
        public virtual string Name { get; set; }
        public virtual string DocumentName { get; set; }
        public virtual byte[] Document { get; set; }
        public virtual int Version { get; set; }
        public virtual int CurrentVersion { get; set; }
        public virtual string Comments { get; set; }
        public virtual Guid FolderId { get; set; }
        public bool LockToTenant { get; set; }
    }
}

