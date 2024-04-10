using DMTServices.Models.AEPS;
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
    public class nsdlEncController : ApiController
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
        #region AepsSearch
        [AcceptVerbs("POST")]
        [HttpPost()]
        [ActionName("AepsSearch")]
        public HttpResponseMessage nsdlEnc(HttpRequestMessage request)
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
            return nsdlEnc(requestString);
        }


        // public HttpResponseMessage GetHotels([FromBody]Hotel_RQ request)
        [AcceptVerbs("POST")]
        [HttpPost()]
        public HttpResponseMessage nsdlEnc(string request)
        {
            string response = string.Empty;
            try
            {
                // objError.IsWritelog = true;
                AddLogs("---Create Token---");
                if ((apiKey!=null || apiKey!="") && requestIP!="")
                {
                    Log.Error.LogLogin(Convert.ToString(CompanyId), Convert.ToString(requestString), "DMTAEPSSearch");
                    AddLogs(string.Format("Common RQ in {0}:", requestContentType));
                    AddLogs(requestString);
                    clsAEPSRequestProcessor objProcessor = new clsAEPSRequestProcessor();
                   
                    if (string.Compare(requestContentType, "application/json") == 0)
                        objProcessor.IsJson = true;


                    BalanceInquiryRequest objRQ = new BalanceInquiryRequest();
                    BalanceInquiryResponse obj = new BalanceInquiryResponse();
                    if (objProcessor.IsJson)
                    {
                        objRQ = JsonConvert.DeserializeObject<BalanceInquiryRequest>(requestString);
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
                        if (objRQ.APIMode == "AEPS2")
                            obj = objProcessor.GetbalancerBalanceSMSdaak(objRQ);
                        else
                            obj = objProcessor.GetbalancerBalance(objRQ);

                    }
                    else
                    {
                       
                    }
                    if (objProcessor.IsJson)
                    {
                        response = JsonConvert.SerializeObject(obj);
                        string json = Convert.ToString(response);
                        Log.Error.LogLogin(Convert.ToString(objRQ.CompanyID), Convert.ToString(json), "KYCStatusResponse");
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

        public static List<string> GetMiniStatement(string field120)
        {
            //remove first 28 characters - next 3 digits is length - divide the total length by 10 i.e. 350/10 = 35  
            //split the string with every 35 characters.

            string str1 = field120.Substring(28);
            string miniStatement = field120.Substring(31);
            Int32 strLength = Convert.ToInt32(str1.Substring(0, 3));
            Int32 lengthToDivide = (strLength / 10);

            List<string> chunks = SplitString(miniStatement, lengthToDivide);
            //foreach (string chunk in chunks)
            //{
            //    Console.WriteLine(chunk);
            //}

            return chunks;
        }

        public static List<string> SplitString(string input, Int32 chunkSize)
        {
            List<string> chunks = new List<string>();

            for (int i = 0; i < input.Length; i += chunkSize)
            {
                int length = Math.Min(chunkSize, input.Length - i);
                string chunk = input.Substring(i, length);
                chunks.Add(chunk);
            }

            return chunks;
        }
        public static string GetNSDLBalanceAmount(string field54)
        {
            /*
                Input value : 000000112200
                Return value : 1122.00
            */
            string intPart;
            string finalValue="";


            //if (Int32.TryParse(field54, out Int32 intValue))
            //{
            //    intPart = intValue.ToString();
            //    string remainingString = intPart.Substring(0, intPart.Length - 2);
            //    string lastTwoChars = intPart.Substring(intPart.Length - 2);
            //    finalValue = remainingString + "." + lastTwoChars;
            //}
            //else
            //{
            //    finalValue = field54;
            //}

            //Console.WriteLine("Balance Amount : " + finalValue);
            return finalValue;
        }
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
