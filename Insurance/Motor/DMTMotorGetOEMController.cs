﻿using DMTServices.Models.Common;
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
    public class DMTMotorGetOEMController : ApiController
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
        [ActionName("GetOEM")]
        public HttpResponseMessage GetOEM(HttpRequestMessage request)
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
            return GetOEM(requestString);
        }


        // public HttpResponseMessage GetOEM([FromBody]Hotel_RQ request)
        [AcceptVerbs("POST")]
        [HttpPost()]
        public HttpResponseMessage GetOEM(string request)
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


                    OEMRequest objRQ = new OEMRequest();
                    OEMResponse obj = new OEMResponse();
                    if (objProcessor.IsJson)
                    {
                        objRQ = JsonConvert.DeserializeObject<OEMRequest>(requestString);
                        Log.Error.LogLogin(Convert.ToString(objRQ.CompanyID), Convert.ToString(requestString), "DMTHealthCareRequest");
                    }
                    else
                    {
                        objRQ = XmlSerializers.DeserializeFromXml<OEMRequest>(requestString);
                    }

                    //objRQ.CompanyID = CompanyId;
                    bool validate = true;// ValidateApiuser(objRQ.TokenId, requestIP, objRQ.CompanyID);
                    if (validate)
                    {
                        // objRQ.ApiPassword
                        obj = objProcessor.GetOEMData(objRQ);
                        if (obj != null)
                        {
                            response = JsonConvert.SerializeObject(obj);
                            string json = Convert.ToString(response);
                            Log.Error.LogLogin(Convert.ToString(objRQ.CompanyID), Convert.ToString(json), "DMTHealthCareResponse");
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
        public class OEMRequest
        {
            public string CompanyID { get; set; }
            public string TokenId { get; set; }
        }
        public class OEMResponse
        {
            public List<InsOEM> InsOEM { get; set; }
            public string errorMsg { get; set; }
            public string errorCode { get; set; }
            public string CompanyID { get; set; }

        }
        public class InsOEM
        {
            public string MAKE_NAME { get; set; }
            public string MAKE_ID { get; set; }
        }
    }
}
