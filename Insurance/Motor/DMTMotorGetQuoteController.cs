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
using TMC.Controllers;

namespace DMTServices.Controllers
{
    public class DMTMotorGetQuoteController : ApiController
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
        [ActionName("GetQuote")]
        public HttpResponseMessage GetQuote(HttpRequestMessage request)
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
            return GetQuotes(requestString);
        }


        // public HttpResponseMessage GetHotels([FromBody]Hotel_RQ request)
        [AcceptVerbs("POST")]
        [HttpPost()]
        public HttpResponseMessage GetQuotes(string request)
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
                        if (string.Compare(requestContentType, "application/json") == 0)
                            objProcessor.IsJson = true;


                        clsGetQuoteReq objRQ = new clsGetQuoteReq();
                        clsCommonGetQuoteResp obj = new clsCommonGetQuoteResp();
                        if (objProcessor.IsJson)
                        {
                            objRQ = JsonConvert.DeserializeObject<clsGetQuoteReq>(requestString);
                            Log.Error.LogLogin(Convert.ToString(objRQ.GetQuoteReq.AuthenticationData.ClientID), Convert.ToString(requestString), "DMTGetQuoteRequest");
                        }
                        else
                        {
                             objRQ = XmlSerializers.DeserializeFromXml<clsGetQuoteReq>(requestString);
                        }

                    //objRQ.CompanyID = CompanyId;
                    bool validate = true;// ValidateApiuser(objRQ.GetQuoteReq.AuthenticationData.TokenId, requestIP, objRQ.GetQuoteReq.AuthenticationData.ClientID);
                    if (validate)
                    {

                        if(objRQ.GetQuoteReq.AuthenticationData.enquiryId != null)
                        {
                            
                            
                            obj = objProcessor.GetQuote(objRQ);

                            
                        }
                        
                        // objRQ.ApiPassword
                        //obj = objProcessor.GetQuote(objRQ);
                        if (obj != null)
                        {
                            response = JsonConvert.SerializeObject(obj);
                            dynamic msg= JsonConvert.DeserializeObject<dynamic>(response);
                            string json = Convert.ToString(response);
                            string response1 = JsonConvert.SerializeObject(objRQ);
                            string json1 = Convert.ToString(response1);
                            if(msg.ResponseCode != "400")
                            {

                            objProcessor.SaveData(json, json1);
                            }
                            Log.Error.LogLogin(Convert.ToString(objRQ.GetQuoteReq.AuthenticationData.ClientID), Convert.ToString(json), "DMtGetQuoteResponse");
                            return new HttpResponseMessage { Content = new StringContent("{\"Response\": { \"Status\": \"Success\",\"Message\": \"successfully\" },\"Data\":" + json + "}", System.Text.Encoding.UTF8, "application/json") };

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
            finally
            {
                // objError.LogError(_companyId, "", sLogs.ToString(), HotelMethods.GetHotels.ToString());
            }

            //   return new HttpResponseMessage { Content = new StringContent("{\"GetStatus\": { \"Status\": \"Success\",\"Message\": \"successfully\" },\"Data\":" + json + "}", System.Text.Encoding.UTF8, "application/json") };
            return null;
        }
        #endregion
        #region ValidateApiuser
        public bool ValidateApiuser(string tokenId, string IPAddress ,string CompanyId)
        {
            bool validate = false;

            clsRequestProcessor objProcessor = new clsRequestProcessor();
            validate = objProcessor.ValidateToken(tokenId, IPAddress, CompanyId);

            return validate;


        }
        #endregion    

    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AuthenticationData
    {
        public string ClientID { get; set; }
        public string ClientType { get; set; }
        public string Service { get; set; }
        public string enquiryId { get; set; }
        public string TokenId { get; set; }


    }

    public class GetQuoteReq
    {
        public AuthenticationData AuthenticationData { get; set; }
        public Request Request { get; set; }
    }

    public class Request
    {
        public string RegNo { get; set; }
        public string P_Email { get; set; }
        public string pol_tenure_ui { get; set; }
        
        public string PolicyType { get; set; }
        public string PolicyTypeCode { get; set; }
        public string CustType { get; set; }
        public string OEMId { get; set; }
        public string OEM { get; set; }
        public string ModelId { get; set; }
        public string ModelName { get; set; }
        public string FuelTypeId { get; set; }
        public string FuelType { get; set; }
        public string Varient { get; set; }
        public string VarientId { get; set; }
        public string ManufectureYear { get; set; }
        public string IsPolicyExpired { get; set; }
        public string ClaimTaken { get; set; }
        public string StateId { get; set; }
        public string StateName { get; set; }
        public string CityId { get; set; }
        public string CityName { get; set; }
        public string RTOId { get; set; }
        public string RTOName { get; set; }
        public string Pincode { get; set; }
        public string NCB { get; set; }
        public string PolicyStartDate { get; set; }
        public string Type { get; set; }
        public string chassisNumber { get; set; }
        public string engineNumber { get; set; }
        public string registrationDate { get; set; }
        public string MobileNo { get; set; }
        public string RTI { get; set; }
        public string ND { get; set; }
        public string currentThirdPartyPolicy { get; set; }
        public string isPreviousInsurerKnown { get; set; }
        public string originalPreviousPolicyType { get; set; }
        public string previousNoClaimBonus { get; set; }
        public string previousPolicyExpiryDate { get; set; }
        public string previousPolicyNumber { get; set; }
        public string previousPolicyType { get; set; }
        public string InsuranceCompID { get; set; }
        public string previousNoClaim { get; set; }
        public string IsKyc { get; set; }
        public string ckycReferenceDocId { get; set; }
        public string ckycReferenceNumber { get; set; }
        public string dateOfBirth { get; set; }
        public string photo { get; set; }

        //for tata aig @rajesh

        
      
        public string vehicle_idv { get; set; }
        public string q_producer_code { get; set; }
        public string q_office_location { get; set; }
        public string vehicle_variant { get; set; }
        public string vehicle_make { get; set; }

        public string vehicle_model { get; set; }

        public string no_past_pol { get; set; }

        public string make_code { get; set; }

        public string model_code { get; set; }
        public string variant_code { get; set; }


        public string pol_tenure { get; set; }
        public string business_type { get; set; }
        public string business_type_no { get; set; }
        public string proposer_type { get; set; }
        public string plan_type { get; set; }
        public string fleet_policy { get; set; }
        public string fleet_code { get; set; }
        public string fleet_name { get; set; }
       
        public string pa_owner { get; set; }
        public string pa_owner_tenure { get; set; }
        public string pa_owner_declaration { get; set; }
        public string cpa_start_date { get; set; }
        public string cpa_end_date { get; set; }
        public string driver_age { get; set; }
        public string driver_gender { get; set; }
        public string driver_occupation { get; set; }
        
        public string claim_last_count { get; set; }
        public string claim_last_amount { get; set; }
        
      
        public string prev_pol_start_date { get; set; }
        
        public string pol_start_date { get; set; }
         
        public string claim_last { get; set; }
        public string pre_pol_ncb { get; set; }

        public string prev_pol_type { get; set; }
        public string rtn_invoice { get; set; }

        public string cng_lpg { get; set; }
        public string dep_reimb { get; set; }
        public string ble_tp_start { get; set; }
        public string ble_tp_end { get; set; }
        public string ble_od_start { get; set; }
        public string ble_od_end { get; set; }
        public string special_regno { get; set; }
        public string regno_1 { get; set; }
        public string regno_2 { get; set; }
        public string regno_3 { get; set; }
        public string regno_4 { get; set; }
        public string place_reg { get; set; }
        public string place_reg_no { get; set; }
        
        public string proposer_pincode { get; set; }

        public string dor { get; set; }

        public string man_year { get; set; }

        public string manu_month { get; set; }
       
       public string veh_plying_city { get; set; }

        public string return_invoice { get; set; }

        public string side_car { get; set; }
        public string side_car_idv { get; set; }
        public string non_electrical_acc { get; set; }
        public string non_electrical_si { get; set; }
        public string non_electrical_des { get; set; }
        public string electrical_acc { get; set; }
        public string electrical_si { get; set; }
        public string electrical_des { get; set; }
        public string cng_lpg_cover { get; set; }
        public string cng_lpg_si { get; set; }
        public string automobile_association_cover { get; set; }
        public string automobile_association_mem_exp_date { get; set; }
        public string automobile_association_mem_no { get; set; }
        public string antitheft_cover { get; set; }
        public string vehicle_blind { get; set; }
        public string own_premises { get; set; }
        public string driving_tution { get; set; }
        public string tppd_discount { get; set; }
        public string voluntary_deductibles { get; set; }
        public string voluntary_amount { get; set; }
        public string voluntary_deductibles_amt { get; set; }
        public string uw_discount { get; set; }
        public string uw_loading { get; set; }
        public string pa_paid { get; set; }
        public string pa_paid_no { get; set; }
        public string pa_paid_si { get; set; }
        public string pa_unnamed { get; set; }
        public string pa_unnamed_no { get; set; }
        public string pa_unnamed_si { get; set; }
        public string dep_reimburse { get; set; }
        public string dep_reimburse_claims { get; set; }
        public string dep_reimburse_deductible { get; set; }
      
        public string consumbale_expense { get; set; }
        public string add_towing { get; set; }
        public string add_towing_amount { get; set; }
        public string rsa { get; set; }
        public string emg_med_exp { get; set; }
        public string emg_med_exp_si { get; set; }
        public string ll_paid { get; set; }
        public string ll_paid_no { get; set; }
        public string ll_emp { get; set; }
        public string ll_emp_no { get; set; }
        public string add_tppd { get; set; }
        public string add_tppd_si { get; set; }
        public string add_pa_owner { get; set; }
        public string add_pa_owner_si { get; set; }
        public string add_pa_unnamed { get; set; }
        public string add_pa_unnamed_si { get; set; }
        public string vehicle_trails_racing { get; set; }
        public string event_name { get; set; }
        public string promoter_name { get; set; }
        public string event_from_date { get; set; }
        public string event_to_date { get; set; }
        public string ext_racing { get; set; }
        public string imported_veh_without_cus_duty { get; set; }
        public string fibre_fuel_tank { get; set; }
        public string loss_accessories { get; set; }
        public string loss_accessories_idv { get; set; }
        public string geography_extension { get; set; }
        public string geography_extension_bang { get; set; }
        public string geography_extension_bhutan { get; set; }
        public string geography_extension_lanka { get; set; }
        public string geography_extension_maldives { get; set; }
        public string geography_extension_nepal { get; set; }
        public string geography_extension_pak { get; set; }
        public string finalise_flag_ui { get; set; }

        //create proposal

        public string quote_id { get; set; }
        public string quote_no { get; set; }
        public string proposal_no { get; set; }
        public string proposal_id { get; set; }
        public string policy_id { get; set; }
        public string product_id { get; set; }
        public string discount_id { get; set; }
        public string nstp_id { get; set; }
        public string payment_id { get; set; }
        public string refferal { get; set; }
        public string sol_id { get; set; }
        public string tagic_emp_code { get; set; }
        public string mobile_no { get; set; }
        public string email_id { get; set; }
        public string premium_value { get; set; }
        public string document_id { get; set; }
        public string self_inspection_link { get; set; }
        public string inspectionFlag { get; set; }
        public string stage { get; set; }
        //public string pol_start_date { get; set; }
        public string payment_stage { get; set; }
        public string policy_stage { get; set; }
        public string proposal_stage { get; set; }
        public string quote_stage { get; set; }




    }

    public class clsGetQuoteReq
    {
        public GetQuoteReq GetQuoteReq { get; set; }
    }

    public class clsGetQuoteRes
    {
        public GetQuoteReq GetQuoteReq { get; set; }
        public string errorCode { get; set; }
        public string errorMsg { get; set; }
    }
}
