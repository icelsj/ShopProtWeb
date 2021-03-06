﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace ShopProtWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "PostmanApi",
                routeTemplate: "api/Postman/{id}",
                defaults: new { controller = "Postman", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "MembershipsApi",
                routeTemplate: "api/GroupLists/{id}/Memberships",
                defaults: new { id = RouteParameter.Optional, controller = "Memberships" }
            );

            config.Routes.MapHttpRoute(
                name: "ItemListApi",
                routeTemplate: "api/GroupLists/{id}/ItemLists/{iid}",
                defaults: new { id = RouteParameter.Optional, controller = "ItemLists", iid = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ShowAllApi",
                routeTemplate: "api/ShowAll",
                defaults: new { controller = "ShowAll" }
            );
        }
    }
}
