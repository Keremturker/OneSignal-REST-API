using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;


/**
 * @author Kerem TÜRKER
 * @gitHub https://github.com/Keremturker
 * Class oneSignal
 * Date: 09.03.2019
 */

namespace OneSignal
{
    public static class WebApiConfig
    {

        public const string URL_Notification = "https://onesignal.com/api/v1/notifications"; // DO NOT TOUCH THIS 
        public const string APP_ID = "YOUR-ONE_SIGNAL-APP-ID";
        public const string API_KEY = "YOUR-ONE_SIGNAL-REST-API-KEY";


        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
