using DMTServices.Models.Common;
using DMTServices.Models.Insurance;
using Newtonsoft.Json;
using Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

using TMC.Controllers;


namespace DMTServices.Controllers
{
    public class DMTMotorCreatePolicyController : ApiController
    {
        // GET api/values

        string CID = string.Empty;
        string requestString = string.Empty;
        string requestContentType = string.Empty;
        StringBuilder sLogs = new StringBuilder();
        string _companyId = string.Empty;
        string apiKey = String.Empty;
        string DMTAPIuserid = string.Empty;
        string DMTAPIpassword = string.Empty;
        string requestIP = "";
        string CompanyId = "";
        string Password = "";

        // ClsCommon_DAL_Hotel _objCmnmDH = null;
        void AddLogs(string sLogString)
        {
            sLogs.Append(Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + sLogString + Environment.NewLine);
        }
        //Error objError = new Error();
        //string LogDir = "Common";
        #region genrateToken
        [AcceptVerbs("POST")]
        [HttpPost()]
        [ActionName("CreateProposal")]
        public HttpResponseMessage DMTMotorCreatePolicy(HttpRequestMessage request)
        {
            var req = this.Request.Content;
            requestContentType = req.Headers.ContentType.ToString();
            requestString = req.ReadAsStringAsync().Result;
            apiKey = Convert.ToString(request.Headers.GetValues("apikey").First());
            //CompanyId = Convert.ToString(request.Headers.GetValues("CompanyID").First());
            // Password = Convert.ToString(request.Headers.GetValues("ApiPassword").First());
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var ctx = request.Properties["MS_HttpContext"] as HttpContextWrapper;
                if (ctx != null)
                {
                    requestIP = ctx.Request.UserHostAddress;
                    //do stuff with IP
                }
            }
            return DMTMotorCreatePolicys(requestString);
         }


        // public HttpResponseMessage GetHotels([FromBody]Hotel_RQ request)
        [AcceptVerbs("POST")]
        [HttpPost()]
        public HttpResponseMessage DMTMotorCreatePolicys(string request)
        {
            string response = string.Empty;
            try
            {
                // objError.IsWritelog = true;
                AddLogs("---Create Token---");
                if ((apiKey != null || apiKey != "") && requestIP != "")
                {

                    AddLogs(string.Format("Common RQ in {0}:", requestContentType));
                    AddLogs(requestString);
                    clsInsuranceProcessor objProcessor = new clsInsuranceProcessor();
                    //clsTataAigReq clsTataAigReq = new clsTataAigReq();
                    //clsTataAigService tataAigService = new clsTataAigService();
                    if (string.Compare(requestContentType, "application/json") == 0)
                        objProcessor.IsJson = true;


                    ClsGetCreatePolicyReq objRQ = new ClsGetCreatePolicyReq();
                    clsCommonCreatePolicyResp obj = new clsCommonCreatePolicyResp();
                    if (objProcessor.IsJson)
                    {
                        objRQ = JsonConvert.DeserializeObject<ClsGetCreatePolicyReq>(requestString);
                        Log.Error.LogLogin(Convert.ToString(objRQ.GetCreatePolicyReq.AuthenticationData1.ClientID), Convert.ToString(requestString), "DMTCreatePolicyRequest");
                    }
                    else
                    {
                        objRQ = XmlSerializers.DeserializeFromXml<ClsGetCreatePolicyReq>(requestString);
                    }

                    //objRQ.CompanyID = CompanyId;
                    bool validate = true;// ValidateApiuser(objRQ.GetQuoteReq.AuthenticationData.TokenId, requestIP, objRQ.GetQuoteReq.AuthenticationData.ClientID);
                    if (validate)
                    {
                        XmlDocument cDocReq = new XmlDocument();


                        // objRQ.ApiPassword
                        if(objRQ.GetCreatePolicyReq.AuthenticationData1.enquiryId != "")
                        {

                            response = objProcessor.CreatePolicy(objRQ);

                        }
                         
                        if (response != null)
                        {
                            
                            dynamic response1 = JsonConvert.DeserializeObject<dynamic>(response);
                            
                           
                            Log.Error.LogLogin(Convert.ToString(objRQ.GetCreatePolicyReq.AuthenticationData1.ClientID), Convert.ToString(response1), "DMtGetQuoteResponse");
                            return new HttpResponseMessage { Content = new StringContent("{\"Response\": { \"Status\": \"Success\",\"Message\": \"successfully\" },\"Data\":" + response1 + "}", System.Text.Encoding.UTF8, "application/json") };

                        }
                        else
                        {
                            return new HttpResponseMessage { Content = new StringContent("{\"Response\": { \"Status\": \"Fail\",\"Message\": \"T-PIN Not genrated\" }}", System.Text.Encoding.UTF8, "application/json") };

                        }
                    }
                    else
                    {
                        return new HttpResponseMessage { Content = new StringContent("{\"Response\": { \"Status\": \"Fail\",\"Message\": \"Ooops!! Login again.\" }}", System.Text.Encoding.UTF8, "application/json") };

                    }

                    AddLogs("Auth Response:" + response);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                AddLogs(string.Format("In catch block(GETTOKEN):{0}{1}:{2} ", ex.Message, Environment.NewLine, ex.StackTrace));
                return null;
            }
            

            //   return new HttpResponseMessage { Content = new StringContent("{\"GetStatus\": { \"Status\": \"Success\",\"Message\": \"successfully\" },\"Data\":" + json + "}", System.Text.Encoding.UTF8, "application/json") };
           
        }
        #endregion
        #region ValidateApiuser
        public bool ValidateApiuser(string tokenId, string IPAddress, string CompanyId)
        {
            bool validate = false;

            clsRequestProcessor objProcessor = new clsRequestProcessor();
            validate = objProcessor.ValidateToken(tokenId, IPAddress, CompanyId);

            return validate;


        }
        #endregion    

    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class GetCreatePolicyReq
    {
        public AuthenticationData1 AuthenticationData1 { get; set; }
        public PolicyRequest1 Request1 { get; set; }
        
    }
    public class ClsGetCreatePolicyReq
    {
        public GetCreatePolicyReq GetCreatePolicyReq { get; set; }
    }
    public class AuthenticationData1
    {
        public string ClientID { get; set; }
        public string ClientType { get; set; }
        public string Service { get; set; }
        public string enquiryId { get; set; }
        public string scode { get; set; }
        public string token { get; set; }
    }


    public class PolicyRequest1
    {
        public string proposal_id { get; set; }
        public string PolicyID { get; set; }
        public string proposer_salutation { get; set; }
        public string proposer_fname { get; set; }
        public string proposer_mname { get; set; }
        public string proposer_lname { get; set; }
        public string proposer_dob { get; set; }
        public string proposer_gender { get; set; }
        public string proposer_marital { get; set; }
        public string proposer_occupation { get; set; }
        public string proposer_add1 { get; set; }
        public string proposer_add2 { get; set; }
        public string proposer_add3 { get; set; }
        public string proposer_state { get; set; }
        public string proposer_pan { get; set; }
        public string proposer_email { get; set; }
        public string proposer_mobile { get; set; }
        public string financier_type { get; set; }
        public string financier_name { get; set; }
        public string vehicle_chassis { get; set; }
        public string vehicle_engine { get; set; }
        public string vehicle_puc_declaration { get; set; }
        public string vehicle_puc { get; set; }
        public string vehicle_puc_expiry { get; set; }
        public string nominee_name { get; set; }
        public string nominee_relation { get; set; }
         public string nominee_age { get; set; }
        public string bund_od_pol_number { get; set; }
        public string bund_od_add { get; set; }
        public string bund_od_insurer_name { get; set; }
        public string bund_tp_pol_number { get; set; }
        public string bund_tp_add { get; set; }
        public string bund_tp_insurer_name { get; set; }
        public string pre_insurer_name { get; set; }
        public string pre_insurer_no { get; set; }
        public string pre_od_insurer_code { get; set; }
        public string pre_od_insurer_name { get; set; }
        public string pre_od_policy_no { get; set; }
        public string pre_tp_insurer_code { get; set; }
        public string pre_tp_insurer_name { get; set; }
        public string pre_tp_pol_no { get; set; }
        public string carriedOutBy { get; set; }
        public bool proposalInspectionOverride { get; set; }
        public string P_Title { get; set; }
        public string P_Name { get; set; }
        public string P_DOB { get; set; }
        public string P_Gender { get; set; }
        public string P_FName { get; set; }
        public string P_LName { get; set; }
        public string P_ADD { get; set; }
         public string NfirstName { get; set; }
         public string NdateOfBirth { get; set; }
         public string NlastName { get; set; }
         public string NmiddleName { get; set; }
         public string Nrelation { get; set; }
         public string previousPolicyNumber { get; set; }

        public string P_FatehrName { get; set; }
        public string P_Martial { get; set; }
        public string P_mother { get; set; }
        public string P_occupation { get; set; }
        public string P_education { get; set; }
        public string P_annualincome { get; set; }
        public string P_Mobile { get; set; }
        public string P_Email { get; set; }
        public string P_PAN { get; set; }
        public string P_address { get; set; }
        public string BasicOD { get; set; }
        public string ThirdPartyPremium { get; set; }
        public string RTIPremium { get; set; }
        public string ZDPremium { get; set; }
        public string NCBDiscount { get; set; }
        public string OtherDiscount { get; set; }
        public string TotalGST { get; set; }
        public string SGST { get; set; }
        public string SGSTPer { get; set; }
        public string CGST { get; set; }
        public string CGSTPer { get; set; }
        public string IGST { get; set; }
        public string IGSTPer { get; set; }
        public string NetPremium { get; set; }
        public string tds { get; set; }
        public string GrossPremium { get; set; }
        public string insuranceProductCode { get; set; }
        public string communicationId { get; set; }
        public string personType { get; set; }
        public string NpersonType { get; set; }
        public string licensePlateNumber { get; set; }
        public string vehicleMaincode { get; set; }

        public string enquiryId { get; set; }
        
        public object pincode { get; set; }
        public object documents { get; set; }
        public string ckycReferenceDocId { get; set; }
        public string kycdateOfBirth { get; set; }
        public string ckycReferenceNumber { get; set; }
        public string photo { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string registrationDate { get; set; }
 
       
    }

   

    public class TataCreatePReq
    {
        public string proposal_id { get; set; }
        public string proposer_salutation { get; set; }
        public string proposer_fname { get; set; }
        public string proposer_mname { get; set; }
        public string proposer_lname { get; set; }
        public string proposer_dob { get; set; }
        public string proposer_gender { get; set; }
        public string proposer_marital { get; set; }
        public string proposer_occupation { get; set; }
        public string proposer_add1 { get; set; }
        public string proposer_add2 { get; set; }
        public string proposer_add3 { get; set; }
        public string proposer_state { get; set; }
        public string proposer_pan { get; set; }
        public string proposer_email { get; set; }
        public string proposer_mobile { get; set; }
        public string financier_type { get; set; }
        public string financier_name { get; set; }
        public string vehicle_chassis { get; set; }
        public string vehicle_engine { get; set; }
        public string vehicle_puc_declaration { get; set; }
        public string vehicle_puc { get; set; }
        public string vehicle_puc_expiry { get; set; }
        public string nominee_name { get; set; }
        public string nominee_relation { get; set; }
        public int nominee_age { get; set; }
        public string bund_od_pol_number { get; set; }
        public string bund_od_add { get; set; }
        public string bund_od_insurer_name { get; set; }
        public string bund_tp_pol_number { get; set; }
        public string bund_tp_add { get; set; }
        public string bund_tp_insurer_name { get; set; }
        public string pre_insurer_name { get; set; }
        public string pre_insurer_no { get; set; }
        public string pre_od_insurer_code { get; set; }
        public string pre_od_insurer_name { get; set; }
        public string pre_od_policy_no { get; set; }
        public string pre_tp_insurer_code { get; set; }
        public string pre_tp_insurer_name { get; set; }
        public string pre_tp_pol_no { get; set; }
        public string carriedOutBy { get; set; }
        public string __finalize { get; set; }
        public bool proposalInspectionOverride { get; set; }
    }

    
    


}
