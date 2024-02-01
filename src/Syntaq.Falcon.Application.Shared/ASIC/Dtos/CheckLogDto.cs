using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class ZCO
    {
        public string company_name { get; set; }
        public string acn { get; set; }
        public string company_type { get; set; }
        public string company_class { get; set; }
        public string certificate_print_option { get; set; }
        public string jurisdiction_of_registration { get; set; }
        public string date_of_registration { get; set; }
        public string company_subclass { get; set; }
    }

    public class RegisteredAgentAddress
    {
        public string care_of { get; set; }
        public string line2 { get; set; }
        public string street { get; set; }
        public string locality { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    }

    public class ZIL
    {
        public string account_number { get; set; }
        public string supplier_name { get; set; }
        public string supplier_abn { get; set; }
        public string registered_agent_name { get; set; }
        public RegisteredAgentAddress registered_agent_address { get; set; }
    }

    public class ZIA
    {
        public string invoice_description { get; set; }
        public string invoice_amount { get; set; }
        public string document_number { get; set; }
        public string form_code { get; set; }
        public string tax_invoice_text { get; set; }
        public string tax_code { get; set; }
        public string tax_amount { get; set; }
        public string reference_number { get; set; }
        public string invoice_issue_date { get; set; }
        public string treasurers_determination { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class DocumentsAccepted
    {
        public string sequence { get; set; }
        public string form { get; set; }
        public string document_number { get; set; }
        public string company_acn { get; set; }
        public string company_name { get; set; }
        public string trace_number { get; set; }
    }

    public class Header
    {
        public List<string> page { get; set; }
        public string trans_report_number { get; set; }
    }

    public class Values
    {
        public ZCO? ZCO { get; set; }
        public ZIL? ZIL { get; set; }
        public ZIA? ZIA { get; set; }
        public object date { get; set; }
        public string validation_report_number { get; set; }
        public object agent { get; set; }
        public string agent_number { get; set; }
        public string agent_address { get; set; }
        public string transmission_number { get; set; }
        public string date_time { get; set; }
        public string sent { get; set; }
        public string received { get; set; }
        public string accepted { get; set; }
        public string warning_1 { get; set; }
        public string warning_2 { get; set; }
        public List<object> documents_rejected { get; set; }
        public List<DocumentsAccepted> documents_accepted { get; set; }
        public bool? call_asic { get; set; }
        public Header header { get; set; }
        public string trans_number { get; set; }
        public string complete { get; set; }
        public bool? error { get; set; }
        public string error_message { get; set; }
        public List<string> files { get; set; }
        public List<object> deleted_files { get; set; }
        public bool? failure { get; set; }
    }

    public class Communication
    {
        public string form { get; set; }
        public string version { get; set; }
        public string reference { get; set; }
        public Values values { get; set; }
        public int bout { get; set; }
    }
    //partial class Response
    //{
    //    public int request_id { get; set; }
    //    public Communication communication { get; set; }
    //    public string created_at { get; set; }
    //}


    public class CheckLogDto
    {

        public List<Response> response { get; set; }
        public List<object> error { get; set; }
    }



}
