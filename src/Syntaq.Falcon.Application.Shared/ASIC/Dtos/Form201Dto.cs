using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.ASIC.Dtos
{
    // Root myDeserializedClass = JsonConvert.Deserializestring<Root>(myJsonResponse); 
    public class MatterInvoiceCho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }
    }

    //public class Matter
    //{
    //    public string formatted_address { get; set; }
    //    public string Addr_Co_txt { get; set; }
    //    public string Addr_Level_txt { get; set; }
    //    public string Addr_1_txt { get; set; }
    //    public string Addr_Suburb_txt { get; set; }
    //    public string Addr_State_txt { get; set; }
    //    public string Addr_PC_txt { get; set; }
    //    public string Addr_Country_cho { get; set; }
    //    public string Addr_txt { get; set; }
    //}

  
    //public class Send
    //{
    //    public string formatted_address { get; set; }
    //    public string Addr_Co_txt { get; set; }
    //    public string Addr_Level_txt { get; set; }
    //    public string Addr_1_txt { get; set; }
    //    public string Addr_Suburb_txt { get; set; }
    //    public string Addr_State_txt { get; set; }
    //    public string Addr_PC_txt { get; set; }
    //    public string Addr_Country_cho { get; set; }
    //    public string Addr_txt { get; set; }
    //}

    public class ClientTypeYn
    {
        public bool Individual { get; set; }
        public string Client_Type_yn { get; set; }
        public string Mtext { get; set; }
        public bool Company { get; set; }
    }

    public class ClientName
    {
        public string Sal_cho { get; set; }
        public string Name_First_txt { get; set; }
        public string Name_Middle_txt { get; set; }
        public string Name_Last_txt { get; set; }
        public string Name_Full_scr { get; set; }
    }

    //public class Client
    //{
    //    public string formatted_address { get; set; }
    //    public string Addr_Co_txt { get; set; }
    //    public string Addr_Level_txt { get; set; }
    //    public string Addr_1_txt { get; set; }
    //    public string Addr_Suburb_txt { get; set; }
    //    public string Addr_State_txt { get; set; }
    //    public string Addr_PC_txt { get; set; }
    //    public string Addr_Country_cho { get; set; }
    //    public string Addr_txt { get; set; }
    //}


    public class FormAddress
    {
        public string formatted_address { get; set; }
        public string Addr_Co_txt { get; set; }
        public string Addr_Level_txt { get; set; }
        public string Addr_1_txt { get; set; }
        public string Addr_Suburb_txt { get; set; }
        public string Addr_State_txt { get; set; }
        public string Addr_PC_txt { get; set; }
        public string Addr_Country_cho { get; set; }
        public string Addr_txt { get; set; }
    }

    public class CoManualReviewYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Co_Manual_Review_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class CoLegalElementCho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }
    }

    public class CoNameConsentYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Co_Name_Consent_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class CoPlaceIncorpCho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }
    }

    public class CoNameAsACNYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Co_Name_As_ACN_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class ASICABNYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string ASIC_ABN_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class CoRBNYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Co_RBN_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class CO410Yn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string CO_410_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class CO410ApplicantTypeYn
    {
        public bool @true { get; set; }
        public string CO_410_Applicant_Type_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class CoUltHoldYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Co_Ult_Hold_yn { get; set; }
        public string Mtext { get; set; }
    }

    //public class Co
    //{
    //    public string formatted_address { get; set; }
    //    public string Addr_Co_txt { get; set; }
    //    public string Addr_Level_txt { get; set; }
    //    public string Addr_1_txt { get; set; }
    //    public string Addr_Suburb_txt { get; set; }
    //    public string Addr_State_txt { get; set; }
    //    public string Addr_PC_txt { get; set; }
    //    public string Addr_Country_cho { get; set; }
    //    public string Addr_txt { get; set; }
    //}

    public class CoRegdAddrSameYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Co_Regd_Addr_Same_yn { get; set; }
        public string Mtext { get; set; }
    }

    //public class CoRegd
    //{
    //    public string formatted_address { get; set; }
    //    public string Addr_Co_txt { get; set; }
    //    public string Addr_Level_txt { get; set; }
    //    public string Addr_1_txt { get; set; }
    //    public string Addr_Suburb_txt { get; set; }
    //    public string Addr_State_txt { get; set; }
    //    public string Addr_PC_txt { get; set; }
    //    public string Addr_Country_cho { get; set; }
    //    public string Addr_txt { get; set; }
    //}

    public class CoAddrRegdOccYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Co_Addr_Regd_Occ_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class CoSuperYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Co_Super_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class CoAddrRegdOccPerYn
    {
        public bool @true { get; set; }
        public string Co_Addr_Regd_Occ_Per_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class SalCho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }
    }

    //public class Dir
    //{
    //    public string formatted_address { get; set; }
    //    public string Addr_Co_txt { get; set; }
    //    public string Addr_Level_txt { get; set; }
    //    public string Addr_1_txt { get; set; }
    //    public string Addr_Suburb_txt { get; set; }
    //    public string Addr_State_txt { get; set; }
    //    public string Addr_PC_txt { get; set; }
    //    public string Addr_Country_cho { get; set; }
    //    public string Addr_txt { get; set; }
    //}

    public class BirthCountryTxt
    {
        public string value { get; set; }
        public string label { get; set; }
        public string mtext { get; set; }
    }

    public class BirthStateCho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }
    }

    public class RoleSecretaryYn
    {
        public bool @true { get; set; }
        public string Role_Secretary_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class RolePubOffYn
    {
        public bool @true { get; set; }
        public string Role_PubOff_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class RoleMemberYn
    {
        public bool @true { get; set; }
        public string Role_Member_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class ClassCho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string Mtext { get; set; }
    }

    public class BenefYn
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }
    }

    public class DirectorShareRpt
    {
        public bool repeat { get; set; }
        public ClassCho Class_cho { get; set; }
        public double Payable_num { get; set; }
        public double Paid_num { get; set; }
        public BenefYn Benef_yn { get; set; }
        public double Unpaid_num { get; set; }
        public double Nbr_num { get; set; }
    }

    public class DirectorRpt
    {
        public bool repeat { get; set; }
       // public SalCho Sal_cho { get; set; }
        public string Name_First_txt { get; set; }
        public string Name_Middle_txt { get; set; }
        public string Name_Last_txt { get; set; }
      //  public string Dir_Gender_txt { get; set; }
        public FormAddress Dir { get; set; }
       // public string Country_Clone_scr { get; set; }
        public string Birth_City_txt { get; set; }
        public string DOB_dt { get; set; }
        public BirthCountryTxt Birth_Country_txt { get; set; }
        public BirthStateCho Birth_State_cho { get; set; }
        public RoleSecretaryYn Role_Secretary_yn { get; set; }
       // public RolePubOffYn Role_PubOff_yn { get; set; }
        public RoleMemberYn Role_Member_yn { get; set; }
        public List<DirectorShareRpt> Director_Share_rpt { get; set; }      
    }

    public class ShareholdersYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Shareholders_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class TypeIndYn
    {
        public bool @true { get; set; }
        public string Type_Ind_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    //public class SHInd
    //{
    //    public string formatted_address { get; set; }
    //    public string Addr_Co_txt { get; set; }
    //    public string Addr_Level_txt { get; set; }
    //    public string Addr_1_txt { get; set; }
    //    public string Addr_Suburb_txt { get; set; }
    //    public string Addr_State_txt { get; set; }
    //    public string Addr_PC_txt { get; set; }
    //    public string Addr_Country_cho { get; set; }
    //    public string Addr_txt { get; set; }
    //}

    public class SHNameRpt
    {
        public bool repeat { get; set; }
       // public string SH_Index_scr { get; set; }
       // public Cho SH_Sal_cho { get; set; }
        public string SH_Name_First_txt { get; set; }
        public string SH_Name_Middle_txt { get; set; }
        public string SH_Name_Last_txt { get; set; }
       // public string SH_Gender_txt { get; set; }
        public FormAddress SH_Ind { get; set; }
    }

    public class SHCoACNYn
    {
        public bool @true { get; set; }
        public string SH_Co_ACN_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class ShareholdersOfficersRpt
    {
        public bool repeat { get; set; }
        public string Co_Dir_Name_First_txt { get; set; }
        public string Co_Dir_Name_Last_txt { get; set; }
    }

    //public class SH
    //{
    //    public string formatted_address { get; set; }
    //    public string Addr_Co_txt { get; set; }
    //    public string Addr_Level_txt { get; set; }
    //    public string Addr_1_txt { get; set; }
    //    public string Addr_Suburb_txt { get; set; }
    //    public string Addr_State_txt { get; set; }
    //    public string Addr_PC_txt { get; set; }
    //    public string Addr_Country_cho { get; set; }
    //    public string Addr_txt { get; set; }
    //}

    public class SHRoleSecretaryYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string SH_Role_Secretary_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class SHRolePubOffYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string SH_Role_PubOff_yn { get; set; }
        public string Mtext { get; set; }
    }

    
    public class ShareholderRpt
    {
        public bool repeat { get; set; }
        public TypeIndYn Type_Ind_yn { get; set; }
        public List<SHNameRpt> SH_Name_rpt { get; set; }
       // public string SH_Names_Count { get; set; }
        public string SH_Co_Name_txt { get; set; }
        public SHCoACNYn SH_Co_ACN_yn { get; set; }
        public string SH_Co_ACN_msk { get; set; }
        //public List<ShareholdersOfficersRpt> Shareholders_Officers_rpt { get; set; }
        public FormAddress SH { get; set; }
        public string SH_Birth_City_txt { get; set; }
        public Cho SH_Birth_Country_txt { get; set; }
        public Cho SH_Birth_State_cho { get; set; }
        public string SH_DOB_dt { get; set; }
        //public string SH_Country_Clone_scr { get; set; }
        public SHRoleSecretaryYn SH_Role_Secretary_yn { get; set; }
        //public SHRolePubOffYn SH_Role_PubOff_yn { get; set; }
        public List<DirectorShareRpt> Shareholder_Share_rpt { get; set; }
        public string Role_Member_yn { get; set; }
     
    }

    public class IndividualsYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Individuals_yn { get; set; }
        public string Mtext { get; set; }
    }

    //public class Ind
    //{
    //    public string formatted_address { get; set; }
    //    public string Addr_Co_txt { get; set; }
    //    public string Addr_Level_txt { get; set; }
    //    public string Addr_1_txt { get; set; }
    //    public string Addr_Suburb_txt { get; set; }
    //    public string Addr_State_txt { get; set; }
    //    public string Addr_PC_txt { get; set; }
    //    public string Addr_Country_cho { get; set; }
    //    public string Addr_txt { get; set; }
    //}
    public class ListComponent
    {
        public string value { get; set; }
        public string label { get; set; }
        public string mtext { get; set; }

    }

    public class IndBirthCountryTxt
    {
        public string value { get; set; }
        public string label { get; set; }
        public string mtext { get; set; }

    }

    public class IndRoleSecretaryYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Ind_Role_Secretary_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class IndRolePubOffYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Ind_Role_PubOff_yn { get; set; }
        public string Mtext { get; set; }
    }

    public class IndividualRpt
    {
        public bool repeat { get; set; }
       // public Cho Ind_Sal_cho { get; set; }
        public string Ind_Name_First_txt { get; set; }
        public string Ind_Name_Middle_txt { get; set; }
        public string Ind_Name_Last_txt { get; set; }
       // public string Ind_Gender_txt { get; set; }
        public FormAddress Ind { get; set; }
      //  public string Ind_Country_Clone_scr { get; set; }
        public string Ind_Birth_City_txt { get; set; }
        public string Ind_DOB_dt { get; set; }

        public IndBirthCountryTxt Ind_Birth_Country_txt { get; set; }
        public Cho Ind_Birth_State_cho {get;set;}
        public IndRoleSecretaryYn Ind_Role_Secretary_yn { get; set; }
       
    }

    public class IssuePreemYn
    {
        public bool @true { get; set; }
        public string Issue_Preem_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class TfrPreemYn
    {
        public bool @true { get; set; }
        public string Tfr_Preem_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class TfrComeAlongYn
    {
        public bool @true { get; set; }
        public string Tfr_ComeAlong_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class TfrTagAlongYn
    {
        public bool @true { get; set; }
        public string Tfr_TagAlong_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class TfrDeemedYn
    {
        public bool @true { get; set; }
        public string Tfr_Deemed_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class GovDirDecisionsCho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }
    }
    public class Cho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }
    }

public class CoUltHoldingPlaceIncorpCho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }

    }

    public class GovDirChairYn
    {
        public bool @true { get; set; }
        public string Gov_Dir_Chair_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class GovMembersDecisionsCho
    {
        public string label { get; set; }
        public string value { get; set; }
        public string mtext { get; set; }
    }

    public class GovMemberChairYn
    {
        public bool @true { get; set; }
        public string Gov_Member_Chair_yn { get; set; }
        public string Mtext { get; set; }
        public bool @false { get; set; }
    }

    public class SubmitYn
    {
        public bool @true { get; set; }
        public bool @false { get; set; }
        public string Submit_yn { get; set; }
        public string Mtext { get; set; }
    }

    //public class Form201PayLoad 
    //{
    //    public string Id { get; set; } //appId
    //    public string data { get; set; }
    //}

    public class Form201Dto
    {
        public Form201Dto()
        {
            //Set Required Classes Here
        }
        public bool is_test_transmission { get; set; } = false;
        public Guid FormId { get; set; }
        public Guid RecordId { get; set; }
        public Guid RecordMatterId { get; set; }
        public Guid RecordMatterItemId { get; set; }
        public Guid SubmissionId { get; set; }

        // public string AppId { get; set; } //appId only submitting to a form
        public string AccessToken { get; set; } 
        public string Send_eml { get; set; }
        public string Error_eml { get; set; }

        // public DateTime dateTime { get; set; }
        //public ClientTypeYn Client_Type_yn { get; set; }
        public ClientName Client_Name { get; set; }
        public FormAddress Client { get; set; }
        public bool Co_Dec_Sig_yn { get; set; }
        public CoManualReviewYn Co_Manual_Review_yn { get; set; }
        public string Co_Manual_Review_Reason_txt { get; set; }
      
        public string Co_Name_txt { get; set; }
        public CoLegalElementCho Co_Legal_Element_cho { get; set; }
        public CoNameConsentYn Co_Name_Consent_yn { get; set; }
        public CoPlaceIncorpCho Co_Place_Incorp_cho { get; set; }
        public CoNameAsACNYn Co_Name_As_ACN_yn { get; set; }
        public string Co_Est_dt { get; set; }
       // public string Co_Type_cho_MText { get; set; }
        public string Co_Type_cho { get; set; }
        public string Co_ACN_msk { get; set; }
        public ASICABNYn ASIC_ABN_yn { get; set; }
        public CoRBNYn Co_RBN_yn { get; set; }
        public string Co_RBN_SA_msk { get; set; }
        public string Co_RBN_ACT_msk { get; set; }
        public string Co_RBN_NSW_msk { get; set; }
        public string Co_RBN_TAS_msk { get; set; }
        public string Co_RBN_NT_msk { get; set; }
        public string Co_RBN_VIC_msk { get; set; }
        public string Co_RBN_QLD_msk { get; set; }
        public string Co_RBN_WA_msk { get; set; }
        public CO410Yn CO_410_yn { get; set; }
        public CO410ApplicantTypeYn CO_410_Applicant_Type_yn { get; set; }
        public string CO_410_Name_First_txt { get; set; }
        public string CO_410_Name_Last_txt { get; set; }
        public string CO_410_Client_Name_txt { get; set; }
        public string CO_410_Number_txt { get; set; }
        public CoUltHoldYn Co_Ult_Hold_yn { get; set; }
        public string Co_Ult_Holding_ABN_msk { get; set; }
        public FormAddress Co { get; set; }
        public CoRegdAddrSameYn Co_Regd_Addr_Same_yn { get; set; }
        public FormAddress Co_Regd { get; set; }
        public CoAddrRegdOccYn Co_Addr_Regd_Occ_yn { get; set; }
        public string Co_Addr_Regd_Occ_txt { get; set; }
        //public CoSuperYn Co_Super_yn { get; set; }
       // public string Super_Add_Text_scr { get; set; }
        public string Co_Class_Sub_cho_MText { get; set; }
        public string Co_Class_cho { get; set; }
       // public string Co_Super_Name_txt { get; set; }
      //  public bool Co_Super_Trust_yn { get; set; }
        public CoAddrRegdOccPerYn Co_Addr_Regd_Occ_Per_yn { get; set; }
        public List<DirectorRpt> Director_rpt { get; set; }
        //public string Co_Dir_Count_scr { get; set; }
      //  public string HasPO { get; set; }
        public ShareholdersYn Shareholders_yn { get; set; }
        public List<ShareholderRpt> Shareholder_rpt { get; set; }
      //  public string SH_HasPO { get; set; }
        public IndividualsYn Individuals_yn { get; set; }
        public List<IndividualRpt> Individual_rpt { get; set; }
        public SubmitYn Submit_yn { get; set; }
        public string ASIC201Submit_yn { get; set; }     
        public string ASIC_ABN_msk { get; set; }
        public string Co_Ult_Holding_Name_txt { get; set; }
        public string Co_Ult_Holding_ACN_msk { get; set; }
        public CoUltHoldingPlaceIncorpCho Co_Ult_Holding_Place_Incorp_cho {get;set;}


    }
    
}
