using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

/**
 * @author Kerem TÜRKER
 * @gitHub https://github.com/Keremturker
 * Class oneSignal
 * Date: 09.03.2019
 */



namespace OneSignal.Controllers
{
    public class NotificationsController : ApiController
    {


        #region  Push Notifications
        [HttpPost]
        public JObject Push_Notification(String _header, String _contents, String _url, String _big_picture, String _large_icon)
        {
      
            var request = WebRequest.Create(WebApiConfig.URL_Notification) as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", "Basic " + WebApiConfig.API_KEY);

            var serializer = new JavaScriptSerializer();
            var obj = new
            {
                app_id = WebApiConfig.APP_ID,
                contents = new { en = _contents },
                headings = new { en = _header },
                url = _url,
                big_picture = _big_picture,
                large_icon = _large_icon,
                included_segments = new string[] { "All" }
            };
            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);
            
            JObject objResult = new JObject();

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {

                        JObject Jobject = JObject.Parse(reader.ReadToEnd());

                        objResult.Add("status", NUMARATOR.SUCCESS.ToString());
                        objResult.Add("id", Jobject.GetValue("id"));
                        objResult.Add("recipients", Jobject.GetValue("recipients"));

                    }
                }

                return objResult;

            }
            catch (WebException e)
            {

                JObject objError = new JObject();

                objError.Add("status", NUMARATOR.ERROR.ToString());
                objError.Add("messages", e.ToString());
                return objError;

            }

        }
        #endregion

        #region View All Notifications

        [HttpGet]
        public JArray View_Notifications()
        {


            JArray arrResult = new JArray();

            try
            {
                string str = WebApiConfig.URL_Notification + "?app_id=" + WebApiConfig.APP_ID;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(str);
                request.ContentType = "application/json; charset=utf-8";
                request.Headers["Authorization"] = "Basic " + WebApiConfig.API_KEY;
                request.PreAuthenticate = true;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;



                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);


                    JObject Jobject = JObject.Parse(reader.ReadToEnd());
                    JArray array = JArray.Parse(Jobject.GetValue("notifications").ToString());


                    for (int i = 0; i < array.Count; i++)
                    {

                        // successful = Number of devices successfully transmitted
                        // converted = Number of users who have clicked / tapped on your notification.


                        int successful = Convert.ToInt32(array[i]["successful"]);
                        int converted = Convert.ToInt32(array[i]["converted"]);
                        int rate_of_click = (100 / successful * converted);

                        JObject obj = new JObject();
                        obj.Add("status", NUMARATOR.SUCCESS.ToString());
                        obj.Add("app_id", array[i]["app_id"]);
                        obj.Add("id", array[i]["id"]);
                        obj.Add("successful", successful);
                        obj.Add("converted", converted);
                        obj.Add("rate_of_click", "%" + rate_of_click);
                        obj.Add("canceled", array[i]["canceled"]);
                        obj.Add("headings", array[i]["headings"]["en"]);
                        obj.Add("contents", array[i]["contents"]["en"]);
                        obj.Add("url", array[i]["url"]);
                        obj.Add("big_picture", array[i]["big_picture"]);
                        obj.Add("large_icon", array[i]["large_icon"]);


                        arrResult.Add(obj);


                    }

                }
                return arrResult;
            }
            catch (Exception e)
            {
                JArray arrError = new JArray();

                JObject objError = new JObject();
                objError.Add("status", NUMARATOR.ERROR.ToString());
                objError.Add("messages", e.ToString());
                arrError.Add(objError);


                return arrError;

            }

        }
        #endregion

        #region View Notification by ID

        [HttpGet]
        public JObject View_Notification(String ID)
        {

            JObject objResult = new JObject();

            try
            {
                string str = WebApiConfig.URL_Notification + "/" + ID + "?app_id=" + WebApiConfig.APP_ID;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(str);
                request.ContentType = "application/json; charset=utf-8";
                request.Headers["Authorization"] = "Basic " + WebApiConfig.API_KEY;
                request.PreAuthenticate = true;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;


                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    JObject Jobject = JObject.Parse(reader.ReadToEnd());

                    int successful = Convert.ToInt32(Jobject.GetValue("successful"));
                    int converted = Convert.ToInt32(Jobject.GetValue("converted"));
                    int rate_of_click = (100 / successful * converted);


                    objResult.Add("status", NUMARATOR.SUCCESS.ToString());
                    objResult.Add("app_id", Jobject.GetValue("app_id"));
                    objResult.Add("id", Jobject.GetValue("id"));
                    objResult.Add("successful", successful);
                    objResult.Add("converted", converted);
                    objResult.Add("rate_of_click", "%" + rate_of_click);
                    objResult.Add("canceled", Jobject.GetValue("canceled"));
                    objResult.Add("headings", Jobject.GetValue("headings")["en"]);
                    objResult.Add("contents", Jobject.GetValue("contents")["en"]);
                    objResult.Add("url", Jobject.GetValue("url"));
                    objResult.Add("big_picture", Jobject.GetValue("big_picture"));
                    objResult.Add("large_icon", Jobject.GetValue("large_icon"));


                }
                return objResult;
            }
            catch (Exception e)
            {
                JObject objError = new JObject();
                objError.Add("status", NUMARATOR.ERROR.ToString());
                objError.Add("messages", e.ToString());


                return objError;

            }

        }
        #endregion

        #region WARNING MESSAGES

        enum NUMARATOR
        {
            SUCCESS,
            ERROR
        }

        #endregion
    }





}
