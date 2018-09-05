using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace BlogFull.Web.Business.Helpers
{
	public static class ReCaptchaHelper
	{
		public static bool Validate(string result)
		{
			//build url
			var secret = ConfigurationManager.AppSettings["recaptcha-secret-key"];
			var request = (HttpWebRequest) WebRequest.Create($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={result}");

			//get the response
			var response = request.GetResponse();
			using (var readStream = new StreamReader(response.GetResponseStream()))
			{
				var jsonResponse = readStream.ReadToEnd();

				var js = new JavaScriptSerializer();
				dynamic data = js.DeserializeObject(jsonResponse);

				if (Convert.ToBoolean(data["success"]))
				{
					//good user
					return true;
				}
			}

			//bad robot
			return false;
		}
	}
}