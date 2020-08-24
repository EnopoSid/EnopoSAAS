using JWT;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Services.Localization;
using System;
using System.IO;
using System.Net;
using System.Text;
using XcellenceIt.Plugin.Misc.WebApiClient.DataClass;
using XcellenceIt.Plugin.Misc.WebApiClient.Filters;
using Nop.Services.Logging;
using System.Reflection;

[assembly: Obfuscation(Feature = "Apply to type *: renaming", Exclude = true, ApplyToMembers = true)]
namespace XcellenceIt.Plugin.Misc.WebApiClient.Controllers
{
    [Route("api/client/[action]")]
    [ApiException]
    public class SecureApiController :Controller
    {
        #region Fields
        private readonly NopRestApiClientSettings _nopRestApiClientSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        #endregion

        #region ctor
        public SecureApiController(NopRestApiClientSettings nopRestApiClientSettings, ILogger logger, ILocalizationService localizationService)
        {
            this._nopRestApiClientSettings = nopRestApiClientSettings;
            this._logger = logger;
            this._localizationService = localizationService;
        }
        #endregion

        #region Method

        [HttpPost]
        public string SecureApi([FromBody]SecureApiRequest secureApiRequest)
        {
            try
            {
                //decrypt jwt token
                var appSecret = _nopRestApiClientSettings.SecretKey != null ? _nopRestApiClientSettings.SecretKey.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries) : null;
                if (appSecret != null)
                {
                    string applicationID = appSecret[0];
                    string secretKey = appSecret[1];
                    var bytes = Encoding.UTF8.GetBytes(secretKey);
                    var secret = Convert.ToBase64String(bytes);
                    var jwtDecryption = (object)null;
                    jwtDecryption = JsonWebToken.DecodeToObject(secureApiRequest.Token, secret, true, true);
                    var jsonObj = JObject.FromObject(jwtDecryption);
                    string appId = jsonObj["appId"].Value<string>();
                    if (appId.Equals(applicationID) && !string.IsNullOrEmpty(appId))
                    {
                        var uri = Request.IsHttps ? "https://" : "http://";
                        uri += Request.Host + "/Api/Client/" + Convert.ToString(jsonObj["method"]);

                        jsonObj.Remove("appId");
                        jsonObj.Remove("iat");
                        jsonObj.Remove("exp");
                        jsonObj.Remove("method");

                        string data = "{";
                        int i = 0;
                        foreach (var item in jsonObj)
                        {
                            if (i > 0)
                                data += ",";

                            if (int.TryParse(Convert.ToString(item.Value), out int itemValue))
                                data += $"\"{item.Key}\":{item.Value}";
                            else if (item.Value.ToString().Contains("{") || item.Value.ToString().Contains("}") || item.Value.ToString().Contains("[") || item.Value.ToString().Contains("]"))
                                data += $"\"{item.Key}\":{item.Value}";
                            else
                                data += $"\"{item.Key}\":\"{item.Value}\"";

                            i++;
                        }
                        data += "}";
                        WebClient client = new WebClient();
                        client.Headers.Add("Content-type", "application/json");
                        client.Headers.Add("Cache-Control", "no-cache");
                        client.Headers.Add("IsSecure", "IsSecure");
                        var myresponse = client.UploadString(new Uri(uri), "POST", data);
                        return myresponse;
                    }
                    else
                    {
                        ErrorObject obj = new ErrorObject
                        {
                            Status = 400,
                            ErrorMessage = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SecureApi.NotValidAppId")
                        };
                        var errResp = JsonConvert.SerializeObject(obj);
                        return errResp;
                    }
                }
                else
                {
                    ErrorObject obj = new ErrorObject
                    {
                        Status = 400,
                        ErrorMessage = _localizationService.GetResource("Plugins.XcellenceIT.WebApiClient.Message.SecureApi.AppSecretNotAvailable")
                    };
                    var errResp = JsonConvert.SerializeObject(obj);
                    return errResp;
                }
            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpErrorResponse = (HttpWebResponse)we.Response as HttpWebResponse;
                    StreamReader reader = new StreamReader(httpErrorResponse.GetResponseStream(), Encoding.UTF8);
                    string responseBody = reader.ReadToEnd();
                    _logger.Error(responseBody);
                    return responseBody;
                }
                _logger.Error(we.Message);
                return we.Message;
            }
        }



        #endregion API
    }
}
