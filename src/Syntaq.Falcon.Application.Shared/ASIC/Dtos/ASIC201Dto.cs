using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.ASIC.Dtos
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class CompanyId
    {
        public string company_type { get; set; }
        public string company_class { get; set; }
        public string company_subclass { get; set; }
        public string acn_yesno { get; set; }
        public string acn_legal { get; set; }
        public string reserved_410 { get; set; }
        public string name_identical { get; set; }
        public string jurisdiction { get; set; }
        public string residential_officeholder { get; set; }
        public string company_name { get; set; }
        public string abn { get; set; }
    }

    public class Address
    {
        public string care_of { get; set; }

        public string line2 { get; set; }
        public string street { get; set; }
        public string locality { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
    }

    public class RegisteredOffice
    {
        public Address address { get; set; }
        public string address_overridden { get; set; }
        public string occupy { get; set; }
        public string occupier_name { get; set; }
        public string occupant_consent { get; set; }
    }

    public class PrincipalPlace
    {
        public Address address { get; set; }
        public string address_overridden { get; set; }
    }

    public class Name
    {
        public string family_name { get; set; }
        public string given_name1 { get; set; }
        public string given_name2 { get; set; }
    }

    public class BirthDetails
    {
        public string date { get; set; }
        public string locality { get; set; }
        public string locality_qualifier { get; set; }
    }

    public class Officer
    {
        public Address address { get; set; }
        public string address_overridden { get; set; }
        public Name name { get; set; }
        public List<string> offices { get; set; }
        public BirthDetails birth_details { get; set; }
    }

    public class HoldingOwner
    {
        public Name member_name_person { get; set; }
        public Address member_address { get; set; }
        public string address_overridden { get; set; }

        public string member_has_acn { get; set; }
        public string member_name_organisation { get; set; }

        public string member_acn_organisation { get; set; }

    }

    public class Member
    {
        public string share_class { get; set; }
        public double number { get; set; }
        public string shares_fully_paid { get; set; }
        public string beneficial_owner { get; set; }
        public double total_paid { get; set; }
        public double total_unpaid { get; set; }
        public double amount_paid_per_share { get; set; }
        public double amount_due_per_share { get; set; }
        public List<HoldingOwner> holding_owners { get; set; }
    }

    //public class NamePerson
    //{
    //    public string family_name { get; set; }
    //    public string given_name1 { get; set; }
    //    public string given_name2 { get; set; }
    //}

    //public class SignatoryName
    //{
    //    public string family_name { get; set; }
    //    public string given_name1 { get; set; }
    //    public string given_name2 { get; set; }
    //}

    public class Applicant
    {
        public Name name_person { get; set; }
        public string name_organisation { get; set; }

        public string acn_organisation { get; set; }
        public Name signatory_name { get; set; }
        public Address address { get; set; }

        public string signatory_role {get;set;}
        public string date_signed { get; set; }
        public string confirm_assented_to { get; set; }
    }

   

    public class Admin
    {
        public string request_manual_review { get; set; }
        public string certificate_delivery_option { get; set; }
        public string has_asic_consent_for_name { get; set; }
        public string text_manual_review { get; set; }
    }

    public class ShareClass
    {
        public string code { get; set; }
        public string title { get; set; }
        public double total_number_issued { get; set; }
        public double total_amount_paid { get; set; }
        public double total_amount_unpaid { get; set; }
    }

    public class Reservation
    { 
        public Name applicant_name_person { get; set; }
        public string applicant_name_organisation { get; set; }

        public string document_number { get; set; }

    }

    public class Business
    { 
        public string place_registration { get; set; }
        public string registration_number { get; set; }
    }

    public class UltimateHolding
    {
        public string name { get; set; }
        public string acn { get; set; }
        public string place_incorporation { get; set; }
        public string abn { get; set; }
    }


    public class ASIC201Dto
    {
        public UltimateHolding ultimate_holding { get; set; }
        public string identifier { get; set; }
        public CompanyId company_id { get; set; }
        public Reservation reservation { get; set; }
        public List<Business> business { get; set; }
        public string members_amount { get; set; }
        public string token { get; set; }
        public string test_transmission { get; set; }

        public bool self_signed { get; set; }
        public string company_full_name { get; set; }
        public RegisteredOffice registered_office { get; set; }
        public PrincipalPlace principal_place { get; set; }
        public List<Officer> officers { get; set; } 
        public List<Member> members { get; set; }
        public Applicant applicant { get; set; }
        public Admin admin { get; set; }
        public List<ShareClass> share_class { get; set; }
        public List<HoldingOwner> nonshare_members { get; set; }
      
    }


}
