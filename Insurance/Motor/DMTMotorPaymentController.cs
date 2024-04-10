using DMTServices.Models.Common;
using DMTServices.Models.Insurance;
using Newtonsoft.Json;
using Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using TMC.Controllers;

namespace DMTServices.Controllers
{
    public class DMTMotorPaymentController : ApiController
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
        [ActionName("DMTMotorPayment")]
        public HttpResponseMessage GetPayment(HttpRequestMessage request)
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
            return GetPayments(requestString);
        }


        // public HttpResponseMessage GetHotels([FromBody]Hotel_RQ request)
        [AcceptVerbs("POST")]
        [HttpPost()]
        public HttpResponseMessage GetPayments(string request)
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


                    clsGetPaymentReq objRQ = new clsGetPaymentReq();
                    clsGetPaymentRes obj = new clsGetPaymentRes();
                        if (objProcessor.IsJson)
                        {
                            objRQ = JsonConvert.DeserializeObject<clsGetPaymentReq>(requestString);
                            Log.Error.LogLogin(Convert.ToString(objRQ.GetPaymentReq2.AuthenticationData2.ClientID), Convert.ToString(requestString), "DMTGetPaymentRequest");
                        }
                        else
                        {
                             objRQ = XmlSerializers.DeserializeFromXml<clsGetPaymentReq>(requestString);
                        }

                    //objRQ.CompanyID = CompanyId;
                    bool validate = true;// ValidateApiuser(objRQ.GetQuoteReq.AuthenticationData.TokenId, requestIP, objRQ.GetQuoteReq.AuthenticationData.ClientID);
                    if (validate)
                    {
                        // objRQ.ApiPassword
                        //obj = objProcessor.GetPayment(objRQ);
                        if (obj != null)
                        {
                            
                            response = JsonConvert.SerializeObject(obj);
                            string json = Convert.ToString(response);
                            Log.Error.LogLogin(Convert.ToString(objRQ.GetPaymentReq2.AuthenticationData2.ClientID), Convert.ToString(json), "DMtGetPaymentResponse");
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
    public class AuthenticationData2
    {
        public string ClientID { get; set; }
        public string ClientType { get; set; }
        public string Service { get; set; }
        public string enquiryId { get; set; }
        public string TokenId { get; set; }


    }

    public class GetPaymentReq2
    {
        public string errorCode { get; set; }
        public string errorMsg { get; set; }
        public AuthenticationData2 AuthenticationData2 { get; set; }
        public Request2 Request2 { get; set; }
    }

    public class Request2
    {
        
        //public string payment_mode { get; set; }
        //public string online_payment_mode { get; set; }
        //public string payer_type { get; set; }
        //public string product_code { get; set; }
        //public string payer_id { get; set; }
        //public string payer_pan_no { get; set; }
        //public string payer_name { get; set; }
        //public string payer_relationship { get; set; }
        //public string email { get; set; }
        //public string office_location_code { get; set; }
        //public string deposit_in { get; set; }

        //public string Type { get; set; }
        //public string pan_no { get; set; }
        //public List<string> payment_id { get; set; }
        //public string returnurl { get; set; }
        public string payment_id { get; set; }
        public string payer_type { get; set; }
        public string amount { get; set; }
        public string usercode { get; set; }
        public string cdacno { get; set; }

    }

    public class clsGetPaymentReq
    {
        public GetPaymentReq2 GetPaymentReq2 { get; set; }
    }

    public class tataPaymentReq
    {
        public string payment_id { get; set; }
        public string payer_type { get; set; }
        public string amount { get; set; }
        public string usercode { get; set; }
        public string cdacno { get; set; }
    }

    public class tataPayVerify
    {
        public string payment_id { get; set; }
    }
    public class clsGetPaymentRes
    {

        public string errorCode { get; set; }
        public string errorMsg { get; set; }

        public string encrypted_policy_id { get; set; }
   
        public string encrypted_policy_no { get; set; }
        //public string product_id { get; set; }

        
        public string net_premium { get; set; }
        //public string verify_stage { get; set; }
        //public string document_stage { get; set; }
        //public string payment_stage { get; set; }

    }

   
}
