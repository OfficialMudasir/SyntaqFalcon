using Newtonsoft.Json;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Documents.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Syntaq.Falcon.API
{
	public class APIManager : FalconDomainServiceBase
	{
		private dynamic _Workflow;
		private CreateOrEditAppJobDto _jobClass;
		private List<PostProcessingItem> _DocCollection;

		public async Task<dynamic> TriggerAPI(dynamic Workflow, CreateOrEditAppJobDto jobClass, List<PostProcessingItem> DocCollection)
		{
			_Workflow = Workflow;
			_jobClass = jobClass;
			_DocCollection = DocCollection;
			dynamic Output = null;
			//var x = Workflow.Rest.ToString();
			//var y = Workflow.Rest.Value;

			string type = Workflow.Rest ?? string.Empty;

			switch (type.ToLower() )
			{
				case "post":
					Debug.WriteLine("Post");
					Output = await PostAPI();
					break;
				case "get":
					Debug.WriteLine("Get");
					break;
				case "put":
					Debug.WriteLine("Put");
					break;
				case "patch":
					Debug.WriteLine("Patch");
					break;
				case "delete":
					Debug.WriteLine("Delete");
					break;
			}

			return Output;
		}

		private dynamic ConvertContent()
		{
			dynamic Content = null;
 
			switch (_Workflow.BodyType.ToString())
			{
				case "text/plain":
					Debug.WriteLine("text/plain");
					break;
				case "text/xml":
					Debug.WriteLine("text/xml");
					if (_Workflow.BodyContent.Contains("%xData%"))
					{
						Content = _jobClass.XData[0].ToString();
						Content = Content.Trim();
					}
					break;
				case "text/html":
					Debug.WriteLine("text/html");
					break;
				case "application/json":
					Debug.WriteLine("application/json");
					// Content = _Workflow.BodyContent == "%AssemblyMessage%" ? JsonConvert.SerializeObject(_jobClass) : JsonConvert.SerializeObject(_Workflow.BodyContent);
					Content = ConvertContentJson();
					break;
				case "application/javascript":
					Debug.WriteLine("application/javascript");
					break;
				case "multipart/form-data":
					Debug.WriteLine("multipart/form-data");
					var multipartContent = new MultipartFormDataContent();
					//StringContent Data = new StringContent(_Workflow.BodyContent == "%AssemblyMessage%" ? JsonConvert.SerializeObject(_jobClass).ToString() : JsonConvert.SerializeObject(_Workflow.BodyContent).ToString(), Encoding.UTF8, "application/json");
					//_Workflow.BodyContent == "%AssemblyMessage%" ? JsonConvert.SerializeObject(_jobClass) : JsonConvert.SerializeObject(_Workflow.BodyContent);


					Content = ConvertContentJson();
					StringContent Data = new StringContent(Content, Encoding.UTF8, "application/json");
					multipartContent.Add(Data, "Data");

					_DocCollection.ForEach(i => 
					{

						if (i.AllowWord)
						{
							ByteArrayContent docContent = new ByteArrayContent(i.Document);
							docContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

							multipartContent.Add(docContent, i.Name);
						}

						if (i.AllowPdf)
						{
							ByteArrayContent pdfContent = new ByteArrayContent(i.PdfDocument);
							pdfContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
							multipartContent.Add(pdfContent, i.Name);
						}

					});
					Content = multipartContent;
					break;
			}
			return Content;
		}

		/// <summary>
		/// Build the message body to the Rest API call based on the %Job% %Data% flags int the App Job
		/// Todo. Check for robustness, dependent of message being well formed
		/// </summary>
		/// <returns></returns>
		private string ConvertContentJson()
		{
			string jobjson = string.Empty;
			string filejson = string.Empty;

			string bodycontent = _Workflow.BodyContent;
			string result = bodycontent;
			if (bodycontent.ToLower().Contains("%job%") || bodycontent.ToLower().Contains("%data%") || bodycontent.ToLower().Contains("%xdata%"))
			{
				//result = "{";


				//if (bodycontent.ToLower().Contains("%job%"))
				//{
				//	jobjson = " \"Job\" :" + JsonConvert.SerializeObject(_jobClass)  + " ";
				//}

				//if (bodycontent.ToLower().Contains("%data%"))
				//{
				//	filejson = _jobClass.Data;
				//	filejson = filejson.Trim();
				//	filejson = filejson.TrimStart('{'); ;
				//	filejson = filejson.TrimEnd('}'); ;
				//}

				//result += string.IsNullOrEmpty(filejson) ? jobjson : string.IsNullOrEmpty(jobjson)? "" : jobjson + ", ";
				//result += filejson;

				//result += "}";


				//if (bodycontent.ToLower().Contains("%job%"))
				//{
				//	jobjson = " \"Job\" :" + JsonConvert.SerializeObject(_jobClass) + " ";
				//}

				//if (bodycontent.ToLower().Contains("%data%"))
				//{
					result = _jobClass.Data;
					result = result.Trim();
					//filejson = filejson.TrimStart('{'); ;
					//filejson = filejson.TrimEnd('}'); ;
				//}

				//result += string.IsNullOrEmpty(filejson) ? jobjson : string.IsNullOrEmpty(jobjson) ? "" : jobjson + ", ";
				//result += filejson;

				//result += "}";

			}

			//if (bodycontent.Contains("%Job%") || bodycontent.Contains("%PdfDoc%"))
			//{

			//}

				return result;

		}

		//private async Task<string> AsyncPostAPI()
		//{
		//    dynamic ConvertedContent = ConvertContent();  //jsonStr = _Workflow.BodyContent == "%AssemblyMessage%" ? JsonConvert.SerializeObject(_jobClass) : JsonConvert.SerializeObject(_Workflow.BodyContent);
		//    using (var client = new HttpClient())
		//    {
		//        dynamic content = null;
		//        if (_Workflow.BodyType != "multipart/form-data")
		//        {
		//            content = new StringContent(ConvertedContent.ToString(), Encoding.UTF8, _Workflow.BodyType.ToString());
		//        }
		//        else
		//        {
		//            content = ConvertedContent;
		//        }
		//        var response = await client.PostAsync(_Workflow.URL, content);
		//        return response;
		//    }
		//}

		private async Task<dynamic> PostAPI()
		{
			if (IsValidUri(_Workflow.URL.ToString()))
			{
				dynamic ConvertedContent = ConvertContent();
				using (var client = new HttpClient())
				{

					 var bodytype= string.IsNullOrEmpty( _Workflow.BodyType) ? "text/plain" : _Workflow.BodyType;

					HttpResponseMessage response = null;
					if (bodytype != "multipart/form-data")
					{
				
							StringContent content = new StringContent(ConvertedContent.ToString(), Encoding.UTF8, bodytype);
						//response = await client.PostAsync(_Workflow.URL.ToString(), content);

						// response.head

						try
						{

							if (!string.IsNullOrEmpty(_Workflow.Header1Key)) client.DefaultRequestHeaders.Add(_Workflow.Header1Key, _Workflow.Header1Value);


							client.DefaultRequestHeaders
								  .Accept
								  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

							//if (!string.IsNullOrEmpty(_Workflow.Header2Key)) client.DefaultRequestHeaders.Add(_Workflow.Header2Key, _Workflow.Header2Value);   // OK// 
							//if (!string.IsNullOrEmpty(_Workflow.Header3Key)) client.DefaultRequestHeaders.Add(_Workflow.Header3Key, _Workflow.Header3Value);   // OK// 
						}
						catch(Exception ex)
						{
							var z = ex;
						}



						response = client.PostAsync(_Workflow.URL.ToString(), content).Result;  // Blocking call!  

						var responseresult = response.Content.ReadAsStringAsync();

						Console.WriteLine(responseresult);
						return _Workflow.Async == true ? responseresult : null;

					}
					else
					{
						MultipartFormDataContent content = ConvertedContent;
						response = await client.PostAsync(_Workflow.URL.ToString(), content);
					}
					//var response = await client.PostAsync(_Workflow.URL.Value, content);
					return _Workflow.Async == true ? response : null;
				}
			}
			return null ;
		}

		Boolean IsValidUri(String uri)
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
