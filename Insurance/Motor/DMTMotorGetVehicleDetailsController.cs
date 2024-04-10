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
    public class DMTMotorGetVehicleDetailsController : ApiController
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
        [ActionName("GetVehicleDetails")]
        public HttpResponseMessage GetVehicle(HttpRequestMessage request)
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
            return GetVehicle(requestString);
        }


        // public HttpResponseMessage GetHotels([FromBody]Hotel_RQ request)
        [AcceptVerbs("POST")]
        [HttpPost()]
        public HttpResponseMessage GetVehicle(string request)
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


                    clsGetVehicleReq objRQ = new clsGetVehicleReq();
                    clsGetVehicleRes obj = new clsGetVehicleRes();
                        if (objProcessor.IsJson)
                        {
                            objRQ = JsonConvert.DeserializeObject<clsGetVehicleReq>(requestString);
                            Log.Error.LogLogin(Convert.ToString(objRQ.vehicleNumber), Convert.ToString(requestString), "DMTGetVehicleRequest");
                        }
                        else
                        {
                             objRQ = XmlSerializers.DeserializeFromXml<clsGetVehicleReq>(requestString);
                        }

                    //objRQ.CompanyID = CompanyId;
                    bool validate = true;// ValidateApiuser(objRQ.GetQuoteReq.AuthenticationData.TokenId, requestIP, objRQ.GetQuoteReq.AuthenticationData.ClientID);
                    if (validate)
                    {

                        if(objRQ.vehicleNumber != null)
                        {
                            
                            
                            obj = objProcessor.GetVehicleReg(objRQ);

                            
                        }
                        
                        // objRQ.ApiPassword
                        //obj = objProcessor.GetQuote(objRQ);
                        if (obj != null)
                        {
                            response = JsonConvert.SerializeObject(obj);
                            string json = Convert.ToString(response);
                            //string response1 = JsonConvert.SerializeObject(objRQ);
                            //string json1 = Convert.ToString(response1);
                            //objProcessor.SaveData(json, json1);
                            Log.Error.LogLogin(Convert.ToString(objRQ.vehicleNumber), Convert.ToString(json), "DMtGetVehicleResponse");
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
   

  

  

    public class clsGetVehicleReq
    {
        public string vehicleNumber { get; set; }
    }

    public class clsGetVehicleRes
    {
        public string errorCode { get; set; }
        public string errorMsg { get; set; }

        public string Data { get; set; }
    }
}
