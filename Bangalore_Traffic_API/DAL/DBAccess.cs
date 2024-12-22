using Bangalore_Traffic_API.BAL;
using Bangalore_Traffic_API.Model;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Bangalore_Traffic_API.DAL
{
    public class DBAccess : IDBAccess
    {
        private readonly string _connectionString;
        private readonly ILogger<DBAccess> _logger;
        public DBAccess(IConfiguration configuration,ILogger<DBAccess> logger)
        {
            _connectionString = configuration["connString"].ToString();
            _logger = logger;
        }


        public string getAccessToken(string todayDate)
        {
            string result = "";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT access_token FROM tblbta_access_token where token_date='" + todayDate + "'";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = reader.GetString(0); // Assuming access_token is a string column
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Occur in getAccessToken Method: " + ex.Message.ToString() + " \n ------------------------------------------------------------------------------------");
                return result;
            }
        }

        public int saveAccessToken(string accessToken)
        {
            try { 
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO tblbta_access_token (access_token, token_date) VALUES (@accessToken, @dateAdded)";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@accessToken", accessToken);
                command.Parameters.AddWithValue("@dateAdded", DateTime.Now.ToString("dd-MM-yyyy"));
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected;
            }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Occur in saveAccessToken Method: " + ex.Message.ToString() + " \n ------------------------------------------------------------------------------------");
                return 0;
            }
        }

        public int saveAPIData(List<BangaloreAPI> list)
        {
            try
            {
                string query = "INSERT IGNORE INTO tblbta_bangtraffic (api_data_id, last_modified_by, event_type,latitude,longitude,endlatitude,endlongitude,location,event_cause,requires_road_closure,start_datetime,end_datetime,status,authenticated,modified_datetime,map_file,direction,description,CityName,executed_time) VALUES";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    foreach (var item in list)
                    {
                       
                        string start_datetime = "";
                        int dotIndexStart = item.start_datetime.IndexOf('.');
                        if (dotIndexStart >= 0)
                        {
                            string formattedDateTime = item.start_datetime.Substring(0, dotIndexStart) + "Z";
                            DateTime startTime = DateTime.ParseExact(formattedDateTime, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
                            start_datetime = startTime.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            DateTime startTime = DateTime.ParseExact(item.start_datetime, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
                            start_datetime = startTime.ToString("yyyy-MM-dd HH:mm:ss");

                        }
                        string end_datetime = "";
                        if (!item.end_datetime.IsNullOrEmpty())
                        {                            
                            int dotIndexEnd = item.end_datetime.IndexOf('.');
                            if (dotIndexEnd >= 0)
                            {
                                string formattedDateTime = item.end_datetime.Substring(0, dotIndexEnd) + "Z";
                                DateTime endTime = DateTime.ParseExact(formattedDateTime, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
                                end_datetime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                DateTime endTime = DateTime.ParseExact(item.end_datetime, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
                                end_datetime = endTime.ToString("yyyy-MM-dd HH:mm:ss");

                            }
                        }
                        



                        string modified_datetime = "";
                        int dotIndexModify = item.modified_datetime.IndexOf('.');
                        if (dotIndexModify >= 0)
                        {
                            string formattedDateTime = item.modified_datetime.Substring(0, dotIndexModify) + "Z";
                            DateTime modifyTime = DateTime.ParseExact(formattedDateTime, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
                             modified_datetime = modifyTime.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else {
                            DateTime modifyTime = DateTime.ParseExact(item.modified_datetime, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
                             modified_datetime = modifyTime.ToString("yyyy-MM-dd HH:mm:ss");

                        }

                        if (string.IsNullOrEmpty(item.endlatitude) || item.endlatitude == "0.0")
                        {
                            item.endlatitude = "";
                        }
                        if (string.IsNullOrEmpty(item.endlongitude) || item.endlongitude == "0.0")
                        {
                            item.endlongitude = "";
                        }

                        query = query + "(" + item.id + ",\'" + item.last_modified_by + "\',\'" + item.event_type + "\',\'" + item.latitude + "\',\'" + item.longitude + "\',\'" + item.endlatitude + "\',\'" + item.endlongitude + "\',\"" + item.address + "\",\'" + item.event_cause + "\'," + item.requires_road_closure + ",\'" + start_datetime + "\',\'" + end_datetime + "\',\'" + item.status + "\',\'" + item.authenticated + "\',\'" + modified_datetime + "\',\'" + item.map_file + "\',\'" + item.direction + "\',\"" + item.description.Replace("\"", "'") + "\",'3',\"" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\"),";

                        //query = query + "(" + item.id + ",\'" + item.last_modified_by + "\',\'" + item.event_type + "\',\'" + item.latitude + "\',\'" + item.longitude + "\',\'" + item.endlatitude + "\',\'" + item.endlongitude + "\',\"" + item.location + "\",\'" + item.event_cause + "\'," + item.requires_road_closure + ",\'" + start_datetime + "\',\'" + end_datetime + "\',\'" + item.status + "\',\'" + item.authenticated + "\',\'" + modified_datetime + "\',\'" + item.map_file + "\',\'" + item.direction + "\',\"" + item.description.Replace("\"","'") + "\",'3',\""+ DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")+ "\"),";
                        //query =query+ " VALUES (" + item.id + ",\'" + item.last_modified_by + "\',\'" + item.event_type + "\',\'" + item.latitude + "\',\'" + item.longitude + "\',\'" + item.endlatitude + "\',\'" + item.endlongitude + "\',\'" + item.location + "\',\'" + item.event_cause + "\'," + item.requires_road_closure + ",DATE_FORMAT(STR_TO_DATE(\'" + item.start_datetime+ "\', '%m/%d/%Y %h:%i:%s %p'), '%Y-%m-%d %h:%i:%s %p'),DATE_FORMAT(STR_TO_DATE(\'" + item.end_datetime+ "\', '%m/%d/%Y %h:%i:%s %p'), '%Y-%m-%d %h:%i:%s %p'),\'" + item.status + "\',\'" + item.authenticated + "\',DATE_FORMAT(STR_TO_DATE(\'"+item.modified_datetime+"\', '%m/%d/%Y %h:%i:%s %p'), '%Y-%m-%d %h:%i:%s %p'),\'" + item.map_file + "\',\'" + item.direction + "\',\'" + item.description + "\'),";
                        // query = query + " VALUES (" + item.id + ",\"" + item.last_modified_by + "\",\"" + item.event_type + "\",\"" + item.latitude + "\",\"" + item.longitude + "\",\"" + item.endlatitude + "\",\"" + item.endlongitude + "\",\"" + item.location + "\",\"" + item.event_cause + "\"," + item.requires_road_closure + "," + item.start_datetime + "," + item.end_datetime + ",\"" + item.status + "\",\"" + item.authenticated + "\"," + item.modified_datetime + ",\"" + item.map_file + "\",\"" + item.direction + "\",\"" + item.description + "\"),";
                    }
                    query = query.TrimEnd(',') + ";";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    int apiDataInserted = command.ExecuteNonQuery();
                    return apiDataInserted;
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Occur in saveAPIData Method: " + ex.Message.ToString() + " \n ------------------------------------------------------------------------------------");
                return 0;
            }

        }

        public int saveAPIImageData(List<EventMapImages> list)
        {
            try
            {
                string query = "INSERT IGNORE INTO tblbta_event_map_images (api_data_id, image_id, image,imgevent,create_datetime) VALUES";

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {

                    connection.Open();

                    foreach (var item in list)
                    {
                        query = query + "(" + item.api_data_id + ",\'" + item.image_id + "\',\'" + item.image + "\',\'" + item.imgevent + "\',\'" + System.DateTime.Now.ToString() + "\'),";

                    }
                    query = query.TrimEnd(',') + ";";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    int apiImageInserted = command.ExecuteNonQuery();
                    return apiImageInserted;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Occur in saveAPIImageData Method: " + ex.Message.ToString() + " \n ------------------------------------------------------------------------------------");
                return 0;
            }
        }

    }
}
