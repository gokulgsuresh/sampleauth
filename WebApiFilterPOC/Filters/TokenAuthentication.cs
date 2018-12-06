using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Linq;

namespace WebApiFilterPOC.Filters
{
    public class TokenAuthentication : AuthorizationFilterAttribute
    {
       //private variabes
        private string _decryptUserAgent;
        private string _decryptIpAddress;
        private string _decryptCurrentTime;
        private string _decryptUserId;

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            IEnumerable<string> userAgent;
            IEnumerable<string> ipAddress;
            IEnumerable<string> currentTimeStamp;
            DateTime curentTime = DateTime.MinValue;

            try
            {
                //check if token exists on the request.
                if (actionContext.Request.Headers.Authorization == null)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                else
                {
                    string token = actionContext.Request.Headers.Authorization.Parameter;
                    actionContext.Request.Headers.TryGetValues("UserAgent", out userAgent);
                    actionContext.Request.Headers.TryGetValues("IpAddress", out ipAddress);
                    actionContext.Request.Headers.TryGetValues("CurrentTime", out currentTimeStamp);
                    DateTime.TryParse(currentTimeStamp.First(), out curentTime);
                    HttpRequest request = HttpContext.Current.Request;

                    //Method To decrypt token
                    string decryptToken = GetDecryptToken(token);

                    if (!string.IsNullOrEmpty(decryptToken))
                    {
                        //fetch values from token
                        GetEncryptedValues(decryptToken);

                        // Database call to check user valid or not.
                        if (!ValidateUser(_decryptUserId))
                        {
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        }
                        //validate useragent and ipaddress.
                        else if ((userAgent == null || ipAddress == null) && _decryptUserAgent != userAgent.First() && _decryptIpAddress != ipAddress.First())
                        {
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        }
                        //validate time stamp.
                        else if (Convert.ToDateTime(_decryptCurrentTime).AddMinutes(30) < curentTime)
                        {
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        }
                    }
                    else
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
            }
            catch (Exception ex)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            base.OnAuthorization(actionContext);
        }

        /// <summary>
        /// Decryption Logic
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetDecryptToken(string token)
        {
            return "user_id=AW322BJH4G54JHGJK*user_agent=Chrome*ip_address=192.168.0.42*time_stamp=12/8/2015 15:15";
        }

        /// <summary>
        /// Split the text with the delimeter provided.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        public string[] SplitString(string value, char delimeter)
        {
            string[] splitValue = value.Split(delimeter);
            return splitValue;
        }

        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ValidateUser(string userId)
        {
            //Code to validate user
            return true;
        }


        /// <summary>
        /// Get encrypted values.
        /// </summary>
        /// <param name="decryptToken"></param>
        private void GetEncryptedValues(string decryptToken)
        {
            string[] items = null;
            //First level of split
            string[] values = SplitString(decryptToken, '*');
            if (values != null && values.Length > 0)
            {
                foreach (var item in values)
                {
                    items = null;
                    //Second level of split.
                    items = SplitString(item, '=');
                    if (items != null && items.Length > 0)
                    {
                        if (items[0] != null && items[0].ToString() == "user_id")
                        {
                            _decryptUserId = items[1] != null ? items[1].ToString() : string.Empty;
                            continue;
                        }
                        if (items[0] != null && items[0].ToString() == "user_agent")
                        {
                            _decryptUserAgent = items[1] != null ? items[1].ToString() : string.Empty;
                            continue;
                        }
                        if (items[0] != null && items[0].ToString() == "ip_address")
                        {
                            _decryptIpAddress = items[1] != null ? items[1].ToString() : string.Empty;
                            continue;
                        }
                        if (items[0] != null && items[0].ToString() == "time_stamp")
                        {
                            _decryptCurrentTime = items[1] != null ? items[1].ToString() : string.Empty;
                            continue;
                        }
                    }
                }
            }
        }
    }
}

