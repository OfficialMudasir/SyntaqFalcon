using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Newtonsoft.Json;

namespace Syntaq.Falcon.Forms.Dtos
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class GetNZBNEntitiesForView
    {
        public int? pageSize { get; set; }
        public int? page { get; set; }
        public int? totalItems { get; set; }
        public object sortBy { get; set; }
        public object sortOrder { get; set; }
        public List<Item> items { get; set; }
        public List<Link2> links { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
        public List<string> methods { get; set; }
    }

    public class Item
    {
        public string entityStatusCode { get; set; }
        public string entityName { get; set; }
        public string nzbn { get; set; }
        public string entityTypeCode { get; set; }
        public string entityTypeDescription { get; set; }
        public string entityStatusDescription { get; set; }
        public List<object> tradingNames { get; set; }
        public List<object> classifications { get; set; }
        public List<string> previousEntityNames { get; set; }
        public DateTime? registrationDate { get; set; }
        public string sourceRegisterUniqueId { get; set; }
        public List<Link> links { get; set; }
    }

    public class Link2
    {
        public string rel { get; set; }
        public string href { get; set; }
        public List<string> methods { get; set; }
    }



    // ENTITY

    public class EmailAddress
    {
        public string uniqueIdentifier { get; set; }
        public string emailAddress { get; set; }
        public string emailPurpose { get; set; }
        public string emailPurposeDescription { get; set; }
        public DateTime? startDate { get; set; }
    }

    public class AddressList
    {
        public string uniqueIdentifier { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string careOf { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string postCode { get; set; }
        public string countryCode { get; set; }
        public string addressType { get; set; }
        public string pafId { get; set; }
    }

    public class Addresses
    {
        public List<Link> links { get; set; }
        public List<AddressList> addressList { get; set; }
    }

    public class IndustryClassification
    {
        public string uniqueIdentifier { get; set; }
        public string classificationCode { get; set; }
        public string classificationDescription { get; set; }
    }

    public class PhoneNumber
    {
        public string uniqueIdentifier { get; set; }
        public string phonePurpose { get; set; }
        public string phonePurposeDescription { get; set; }
        public string phoneCountryCode { get; set; }
        public string phoneAreaCode { get; set; }
        public string phoneNumber { get; set; }
        public DateTime? startDate { get; set; }
    }

    public class Website
    {
        public string uniqueIdentifier { get; set; }
        public string url { get; set; }
        public string websitePurpose { get; set; }
        public DateTime? startDate { get; set; }
    }

    public class TradingName
    {
        public string uniqueIdentifier { get; set; }
        public string name { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class PrivacySettings
    {
        public bool? nzbnPrivateInformation { get; set; }
        public bool? namePrivateInformation { get; set; }
        public bool? tradingNamePrivateInformation { get; set; }
        public bool? businessClassificationPrivateInformation { get; set; }
        public bool? phonePrivateInformation { get; set; }
        public bool? emailPrivateInformation { get; set; }
        public bool? partnersPrivateInformation { get; set; }
        public bool? trusteesPrivateInformation { get; set; }
        public bool? publicSuggestChangesYn { get; set; }
        public bool? abnPrivateInformation { get; set; }
        public bool? gstNumberPrivateInformation { get; set; }
        public bool? paymentBankAccountNumbersPrivateInformation { get; set; }
        public bool? registeredAddressPrivateInformation { get; set; }
        public bool? postalAddressPrivateInformation { get; set; }
        public bool? serviceAddressPrivateInformation { get; set; }
        public bool? invoiceAddressPrivateInformation { get; set; }
        public bool? deliveryAddressPrivateInformation { get; set; }
        public bool? officeAddressPrivateInformation { get; set; }
    }

    public class IndividualShareholder
    {
        public string firstName { get; set; }
        public string middleNames { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
    }

    public class OtherShareholder
    {
        public string currentEntityName { get; set; }
        public string nzbn { get; set; }
        public string companyNumber { get; set; }
        public string entityType { get; set; }
    }

    public class ShareholderAddress
    {
        public string uniqueIdentifier { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string careOf { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string postCode { get; set; }
        public string countryCode { get; set; }
        public string addressType { get; set; }
        public string pafId { get; set; }
    }

    public class Shareholder
    {
        public string type { get; set; }
        public DateTime? appointmentDate { get; set; }
        public DateTime? vacationDate { get; set; }
        public IndividualShareholder individualShareholder { get; set; }
        public OtherShareholder otherShareholder { get; set; }
        public ShareholderAddress shareholderAddress { get; set; }
    }

    public class ShareAllocation
    {
        public int? allocation { get; set; }
        public List<Shareholder> shareholder { get; set; }
    }

    public class HistoricShareholderAddress
    {
        public string uniqueIdentifier { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string careOf { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string postCode { get; set; }
        public string countryCode { get; set; }
        public string addressType { get; set; }
        public string pafId { get; set; }
    }

    public class HistoricIndividualShareholder
    {
        public string firstName { get; set; }
        public string middleNames { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
    }

    public class HistoricOtherShareholder
    {
        public string currentEntityName { get; set; }
        public string nzbn { get; set; }
        public string companyNumber { get; set; }
        public string entityType { get; set; }
    }

    public class HistoricShareholder
    {
        public string type { get; set; }
        public DateTime? appointmentDate { get; set; }
        public DateTime? vacationDate { get; set; }
        public HistoricShareholderAddress historicShareholderAddress { get; set; }
        public HistoricIndividualShareholder historicIndividualShareholder { get; set; }
        public HistoricOtherShareholder historicOtherShareholder { get; set; }
    }

    public class Shareholding
    {
        public int? numberOfShares { get; set; }
        public List<ShareAllocation> shareAllocation { get; set; }
        public List<HistoricShareholder> historicShareholder { get; set; }
    }

    public class UltimateHoldingCompanyAddress
    {
        public string uniqueIdentifier { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string careOf { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string postCode { get; set; }
        public string countryCode { get; set; }
        public string addressType { get; set; }
        public string pafId { get; set; }
    }

    public class UltimateHoldingCompany
    {
        public bool? yn { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string number { get; set; }
        public string nzbn { get; set; }
        public string country { get; set; }
        public DateTime? effectiveDate { get; set; }
        public List<UltimateHoldingCompanyAddress> ultimateHoldingCompanyAddress { get; set; }
    }

    public class InsolvencyReport
    {
        public string name { get; set; }
        public bool? filed { get; set; }
        public DateTime? date { get; set; }
    }

    public class InsolvencyAppointeePhoneNumber
    {
        public string uniqueIdentifier { get; set; }
        public string phonePurpose { get; set; }
        public string phonePurposeDescription { get; set; }
        public string phoneCountryCode { get; set; }
        public string phoneAreaCode { get; set; }
        public string phoneNumber { get; set; }
        public DateTime? startDate { get; set; }
    }

    public class InsolvencyAppointeeAddress
    {
        public string uniqueIdentifier { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string careOf { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string postCode { get; set; }
        public string countryCode { get; set; }
        public string addressType { get; set; }
        public string pafId { get; set; }
    }

    public class InsolvencyAppointee
    {
        public string firstName { get; set; }
        public string middleNames { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string organisationName { get; set; }
        public DateTime? appointmentDate { get; set; }
        public DateTime? vacationDate { get; set; }
        public string email { get; set; }
        public List<InsolvencyAppointeePhoneNumber> insolvencyAppointeePhoneNumber { get; set; }
        public InsolvencyAppointeeAddress insolvencyAppointeeAddress { get; set; }
    }

    public class Insolvency
    {
        public DateTime? commenced { get; set; }
        public string insolvencyType { get; set; }
        public List<InsolvencyReport> insolvencyReport { get; set; }
        public List<InsolvencyAppointee> insolvencyAppointee { get; set; }
    }

    public class CompanyDetails
    {
        public int? annualReturnFilingMonth { get; set; }
        public int? financialReportFilingMonth { get; set; }
        public string nzsxCode { get; set; }
        public DateTime? annualReturnLastFiled { get; set; }
        public bool? hasConstitutionFiled { get; set; }
        public string countryOfOrigin { get; set; }
        public string overseasCompany { get; set; }
        public bool? extensiveShareholding { get; set; }
        public bool? stockExchangeListed { get; set; }
        public Shareholding shareholding { get; set; }
        public UltimateHoldingCompany ultimateHoldingCompany { get; set; }
        public string australianCompanyNumber { get; set; }
        public List<Insolvency> insolvencies { get; set; }
        public bool? removalCommenced { get; set; }
    }

    public class NonCompanyDetails
    {
        public int? annualReturnFilingMonth { get; set; }
        public string countryOfOrigin { get; set; }
        public string registeredUnionStatus { get; set; }
        public string charitiesNumber { get; set; }
        public DateTime? balanceDate { get; set; }
    }

    public class RoleEntity
    {
        public string nzbn { get; set; }
        public string entityName { get; set; }
    }

    public class RolePerson
    {
        public string title { get; set; }
        public string firstName { get; set; }
        public string middleNames { get; set; }
        public string lastName { get; set; }
    }

    public class RoleAddress
    {
        public string uniqueIdentifier { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string careOf { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string postCode { get; set; }
        public string countryCode { get; set; }
        public string addressType { get; set; }
        public string pafId { get; set; }
    }

    public class RoleAsicAddress
    {
        public string uniqueIdentifier { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string careOf { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string address4 { get; set; }
        public string postCode { get; set; }
        public string countryCode { get; set; }
        public string addressType { get; set; }
        public string pafId { get; set; }
    }

    public class Role
    {
        public string roleType { get; set; }
        public string roleStatus { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public bool? asicDirectorshipYn { get; set; }
        public string asicName { get; set; }
        public string acn { get; set; }
        public RoleEntity roleEntity { get; set; }
        public RolePerson rolePerson { get; set; }
        public List<RoleAddress> roleAddress { get; set; }
        public RoleAsicAddress roleAsicAddress { get; set; }
        public string uniqueIdentifier { get; set; }
    }

    public class TradingArea
    {
        public string uniqueIdentifier { get; set; }
        public string tradingArea { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class GstNumber
    {
        public string uniqueIdentifier { get; set; }
        public string gstNumber { get; set; }
        public string purpose { get; set; }
        public DateTime? startDate { get; set; }
    }

    public class Url
    {
        public string rel { get; set; }
        public string href { get; set; }
        public List<string> methods { get; set; }
    }

    public class Document2
    {
        public string rel { get; set; }
        public string href { get; set; }
        public List<string> methods { get; set; }
    }

    public class Document
    {
        public string documentId { get; set; }
        public string documentType { get; set; }
        public Url url { get; set; }
        public Document document { get; set; }
    }

    public class ProofOfIdentity
    {
        public string metadataId { get; set; }
        public string metadataType { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Number { get; set; }
        public string Version { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class SupportingInformation
    {
        public List<Document> documents { get; set; }
        public List<ProofOfIdentity> proofOfIdentity { get; set; }
        public string authorityType { get; set; }
        public string organisationId { get; set; }
    }

    public class NZBNEntity
    {
        public string entityStatusCode { get; set; }
        public string entityName { get; set; }
        public string nzbn { get; set; }
        public string entityTypeCode { get; set; }
        public string entityTypeDescription { get; set; }
        public string entityStatusDescription { get; set; }
        public DateTime? registrationDate { get; set; }
        public string sourceRegister { get; set; }
        public string sourceRegisterUniqueIdentifier { get; set; }
        public DateTime? lastUpdatedDate { get; set; }
        public string australianBusinessNumber { get; set; }
        public List<EmailAddress> emailAddresses { get; set; }
        public Addresses addresses { get; set; }
        public List<IndustryClassification> industryClassifications { get; set; }
        public List<PhoneNumber> phoneNumbers { get; set; }
        public List<Website> websites { get; set; }
        public List<TradingName> tradingNames { get; set; }
        public PrivacySettings privacySettings { get; set; }

        [JsonProperty("company-details")]
        public CompanyDetails CompanyDetails { get; set; }

        [JsonProperty("non-company-details")]
        public NonCompanyDetails NonCompanyDetails { get; set; }
        public List<Role> roles { get; set; }
        public List<TradingArea> tradingAreas { get; set; }
        public List<GstNumber> gstNumbers { get; set; }

        [JsonProperty("supporting-information")]
        public SupportingInformation SupportingInformation { get; set; }
        public List<object> organisationParts { get; set; }
        public string hibernationStatusCode { get; set; }
        public string hibernationStatusDescription { get; set; }
    }


}