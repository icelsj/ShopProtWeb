using Newtonsoft.Json.Linq;
using ShopProtWeb.Models;
using System;
using System.IO;
using System.Net;

namespace ShopProtWeb
{
    public static class UniTool
    {
        public static bool VerifyFacebook(string fbId, string accessToken, out UserResponseModel model)
        {
            try
            {
                string graphUrl = string.Format("https://graph.facebook.com/me?access_token={0}", accessToken);
                WebRequest request = WebRequest.Create(graphUrl);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseData = reader.ReadToEnd();
                reader.Close();
                response.Close();

                JObject graphData = JObject.Parse(responseData);
                string graphId = graphData.GetValue("id").ToString();

                model = new UserResponseModel();
                model.facebook_id = graphId;
                model.gender = graphData.GetValue("gender").ToString();
                model.email = graphData.GetValue("email").ToString();
                model.name = graphData.GetValue("name").ToString();
                model.first_name = graphData.GetValue("first_name").ToString();
                model.last_name = graphData.GetValue("last_name").ToString();

                if (graphId.Equals(fbId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                model = new UserResponseModel();
                return false;
            }
        }
    }
}