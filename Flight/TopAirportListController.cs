﻿using DMTServices.Models.Flight;
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
using DMTServices.Models.Common;

namespace DMTServices.Controllers
{
    public class TopAirportListController : ApiController
    {

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
        
        #region StateList
        [AcceptVerbs("POST")]
        [HttpPost()]
        [ActionName("CityList")]
        public HttpResponseMessage TopAirportList(HttpRequestMessage request)
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
            return TopAirportList(requestString);
        }


        // public HttpResponseMessage GetHotels([FromBody]Hotel_RQ request)
        [AcceptVerbs("POST")]
        [HttpPost()]
        public HttpResponseMessage TopAirportList(string request)
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
                    clsFlightRequestProcessor objProcessor = new clsFlightRequestProcessor();

                    if (string.Compare(requestContentType, "application/json") == 0)
                        objProcessor.IsJson = true;


                    AirportListRequest objRQ = new AirportListRequest();
                    AirportListReponse obj = new AirportListReponse();
                    if (objProcessor.IsJson)
                    {
                        objRQ = JsonConvert.DeserializeObject<AirportListRequest>(requestString);
                    }
                    else
                    {
                        // objRQ = XmlSerializers.DeserializeFromXml<TokenRequest>(requestString);
                    }


                    // objRQ.ApiPassword
                    CompanyId = objRQ.CompanyID;
                    bool validate = ValidateApiuser(objRQ.TokenID, requestIP);
                    if (validate)
                    {
                        obj = objProcessor.TOPAirportList(objRQ);
                    }
                    else
                    {

                    }
                    if (objProcessor.IsJson)
                    {
                        response = JsonConvert.SerializeObject(obj);
                        string json = Convert.ToString(response);
                        return new HttpResponseMessage { Content = new StringContent("{\"Response\": { \"Status\": \"Success\",\"Message\": \"successfully\" },\"Data\":" + json + "}", System.Text.Encoding.UTF8, "application/json") };

                    }

                    AddLogs("Auth Response:" + response);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                AddLogs(string.Format("In catch block(GetHotels):{0}{1}:{2} ", ex.Message, Environment.NewLine, ex.StackTrace));
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
        public bool ValidateApiuser(string tokenId, string IPAddress)
        {
            bool validate = false;

            clsRequestProcessor objProcessor = new clsRequestProcessor();
            validate = objProcessor.ValidateToken(tokenId, IPAddress, CompanyId);

            return validate;


        }
        #endregion  
    }
}
