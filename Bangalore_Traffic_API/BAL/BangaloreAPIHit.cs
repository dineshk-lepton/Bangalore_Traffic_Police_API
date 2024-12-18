using Azure.Core;
using Bangalore_Traffic_API.Model;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Bangalore_Traffic_API.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Org.BouncyCastle.Crypto;
using System.Data;
using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;
using Mysqlx.Session;
using MySqlX.XDevAPI;

namespace Bangalore_Traffic_API.BAL
{
    public class BangaloreAPIHit:IBangaloreAPIHit
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly IDBAccess _dl;
        private readonly ILogger<BangaloreAPIHit> _logger;
        int apiDataCount = 0, imgCount = 0;
        public BangaloreAPIHit(IConfiguration config, IDBAccess dl,HttpClient httpClient, ILogger<BangaloreAPIHit> logger)
        {
            _config = config;
            _httpClient = httpClient;
            _logger= logger;
            _dl = dl;
        }


       public async Task<int> bangaloreTrafficLoginAPIConsume()
        {
            int finalData;
            string accessToken="";
            string apiLogin = _config["loginAPIDetails:loginAPI"];
            string apiUName = _config["loginAPIDetails:username"];
            string apiPassword = _config["loginAPIDetails:password"];
            try
            {
                _logger.LogInformation("Checking for Token Availability for Today. \n ------------------------------------------------------------------------------------");
                accessToken = _dl.getAccessToken(DateTime.Now.ToString("dd-MM-yyyy"));
                if (accessToken.IsNullOrEmpty())
                {
                    // Prepare the HTTP request
                    var request = new HttpRequestMessage(HttpMethod.Post, apiLogin);
                    var content = new StringContent("{\"username\":\"" + apiUName + "\",\"password\":\"" + apiPassword + "\"}", null, "application/json");
                    request.Content = content;

                    // Send the HTTP request and get the response
                    var response = await _httpClient.SendAsync(request);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        JObject responseJson = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        // Extract the value of the 'access' property
                        accessToken = (string)responseJson["access"];
                        int i = _dl.saveAccessToken(accessToken);
                        if (i > 0)
                        {
                            _logger.LogInformation("New Token Saved Successfully in DataBase. \n ------------------------------------------------------------------------------------");
                            finalData = await bangaloreTrafficGetDataAPIConsume(accessToken);
                            // Read and return the response content
                            if (finalData > 0)
                            {
                                _logger.LogInformation("New Token Saved Successfully in Database. \n ------------------------------------------------------------------------------------");
                                return 1;

                            }
                            else
                            {
                                _logger.LogError("New Token not saved in Database. \n ------------------------------------------------------------------------------------");
                                return 0;
                            }

                        }
                        else
                        {
                            _logger.LogError("New Token Not Generated. \n ------------------------------------------------------------------------------------");
                            return 0;
                        }
                    }
                    else
                    {
                        _logger.LogError("Response for Token Generation is :" + response.RequestMessage + ". \n ------------------------------------------------------------------------------------");
                        return 0;
                    }
                }
                else
                {
                    _logger.LogInformation("Token Already Availability for Today and Start Getting New Feed. \n ------------------------------------------------------------------------------------");
                    finalData = await bangaloreTrafficGetDataAPIConsume(accessToken);
                    // Read and return the response content
                    return finalData;
                }
            }catch (Exception ex) 
            {
                _logger.LogError("Exception Occur in bangaloreTrafficLoginAPIConsume Method: " + ex.Message.ToString() + " \n ------------------------------------------------------------------------------------");
                return 0;
            }
        }
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ0b2tlbl90eXBlIjoiYWNjZXNzIiwiZXhwIjoxNzExODIzNjI4LCJpYXQiOjE3MTE3MzcyMjgsImp0aSI6IjdiMjVkNTg5M2EzNjQwODc4ZmE2MGMzZDMzMmQzOTkxIiwidXNlcl9pZCI6MTQ0NH0.7fzzm-VKevCC7mNsXjODlTXb8zFOGihjYcN7YU46pEY
        public async Task<int> bangaloreTrafficGetDataAPIConsume(string accessToken)
        {
            string getDataAPI = _config["FinalDataAPIDetails:GetDataAPI"];

            string startDate =  _config["FinalDataAPIDetails:start_date"];
            string endDate =  _config["FinalDataAPIDetails:end_date"];
            //string startDate = DateTime.Now.ToString("yyyy-MM-dd") + _config["FinalDataAPIDetails:start_date"];
            //string endDate = DateTime.Now.ToString("yyyy-MM-dd")+ _config["FinalDataAPIDetails:end_date"];
            //string statusChoice = _config["FinalDataAPIDetails:status_choice"];
           string authenticated = _config["FinalDataAPIDetails:authenticated"];
           // int client = int.Parse(_config["FinalDataAPIDetails:client"]);
            try
            {

                // Prepare the HTTP request            
                var request = new HttpRequestMessage(HttpMethod.Post, getDataAPI);
                request.Headers.Add("Authorization", "Bearer " + accessToken);

                /* if (statusChoice.IsNullOrEmpty() && authenticated.IsNullOrEmpty())
                 {
                      var content = new StringContent("{\"start_date\":\"" + startDate + "\"," +
                        "\"end_date\":\"" + endDate + "\"}", null, "application/json");
                     request.Content = content;
                     //var content = new StringContent("{\"start_date\":\"2024-03-27T01:00:04Z\",\r\n\"end_date\": \"2024-03-27T23:00:00Z\"\r\n}", null, "application/json");
                 }
                 else 
                 {
                      var content = new StringContent("{\"start_date\":\"" + startDate + "\"," +
                     "\"end_date\":\"" + endDate + "\"," +
                     "\"status_choice\":\"" + statusChoice + "\"," +
                     "\"authenticated\":\"" + authenticated + "\" }", null, "application/json");
                     request.Content = content;
                 }*/

                /*var content = new StringContent("{\"start_date\":\"" + startDate + "\"," +
                  "\"end_date\":\"" + endDate + "\"," +
                  "\"status_choice\":" + statusChoice + "," +
                  "\"authenticated\":\"" + authenticated + "\","+
                  "\"client\":" + client + "}", null, "application/json");*/

                var content = new StringContent( "{ \"start_date\": \"" + startDate + "\"," +
     "\"end_date\": \"" + endDate + "\"," +
     "\"authenticated\": \"" + authenticated + "\" }", null,"application/json");

                request.Content = content;

                // Send the HTTP request and get the response
                var response = await _httpClient.SendAsync(request);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {

                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (responseContent.Length <= 2)
                    {
                        _logger.LogInformation(" There is no data in response Feed.\n ------------------------------------------------------------------------------------");
                        _logger.LogInformation("Response Feed is: " + responseContent.ToString() + " \n ------------------------------------------------------------------------------------");
                        return 0;
                       
                    }
                    else
                    {
                        // Deserialize the response into a list of BangaloreAPI objects
                        List<BangaloreAPI> bangaloreAPIResult = JsonConvert.DeserializeObject<List<BangaloreAPI>>(responseContent);

                        if (bangaloreAPIResult.Count >= 1)
                        {
                            List<EventMapImages> listEventMapImages = bangaloreAPIResult
                            .SelectMany(api => api.event_map_images, (api, img) => new EventMapImages
                            {
                                api_data_id = api.id,
                                image_id = img.image_id,
                                //image = "https://airavt.ai" + img.image,
                                image = img.image,
                                imgevent = img.imgevent
                            })
                            .ToList();
                            apiDataCount = _dl.saveAPIData(bangaloreAPIResult);
                            imgCount = _dl.saveAPIImageData(listEventMapImages);
                            if (apiDataCount > 0 || imgCount > 0)
                            {
                                _logger.LogInformation(apiDataCount + " New Traffic Feed Inserted Successfully into tblbta_bangtraffic Table\n ------------------------------------------------------------------------------------");
                                _logger.LogInformation(imgCount + " New Image Feed Inserted Successfully into tblbta_event_map_images Table \n ------------------------------------------------------------------------------------");
                            }
                            else
                            {
                                _logger.LogInformation(" New Feed not Available.\n ------------------------------------------------------------------------------------");
                            }
                        }
                        return apiDataCount;
                    }

                }
                else
                {    // Log or handle the error appropriately
                     // For now, let's just return an empty string
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Occur into bangaloreTrafficGetDataAPIConsume Method: " + ex.Message.ToString() + " \n ------------------------------------------------------------------------------------");
                return 0;
               
            }

}
       
    }
}
