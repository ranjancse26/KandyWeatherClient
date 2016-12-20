using System;
using System.Xml;
using System.Configuration;

using KandyCSharp.Kandy;
using KandyCSharp.Entities.Message;

using Newtonsoft.Json;
using NHyperCat;

namespace WeatherSample
{
	/// <summary>
	/// Reused and modified code from - https://github.com/raharrison/GoogleAPI/tree/master/GoogleWeather
	/// Personal Weather Service - http://api.previmeteo.com/API_KEY/ig/api?weather=CITY_NAME
	/// </summary>
	class MainClass
	{
		public static void Main (string[] args)
		{
            // Register yourself with the Weather Service to get the API KEY
		    string weatherApiKey = ConfigurationManager.AppSettings["WeatherAPIKey"];
		    string kandyDeviceId = ConfigurationManager.AppSettings["DeviceId"];
	
			if (weatherApiKey.Equals ("<APIKEY>")) {
				Console.WriteLine ("Please key in the API KEY to the fetch weather information");
				weatherApiKey = Console.ReadLine ();
			}

			Console.WriteLine ("Enter city name: ");
			var city = Console.ReadLine ();
		    WeatherEntity weather = null;

            try
            {
				weather = GetWeatherCondition(weatherApiKey, city);
				if(weather != null){
					Console.WriteLine ("************************************************** ");
					Console.WriteLine ("TempC: " + weather.TempInCentigrade);
					Console.WriteLine ("TempF: " + weather.TempInFahrenheit);
					Console.WriteLine ("Condition: "+ weather.Condition);
					Console.WriteLine (weather.Humidity);
					Console.WriteLine (weather.Wind);
					Console.WriteLine ("************************************************** ");
        		}
				else{
					Console.WriteLine ("Problem in fetching in the weather information.");
				}
			}
			catch(Exception ex){
				Console.WriteLine (ex.ToString ());
			}

            // Build Hypercat Catalouge
		    var hypercatCatalouge = "";

            if (weather != null)
		    {
                hypercatCatalouge = BuildHypercatCatalouge(city, weather);
                Console.WriteLine(hypercatCatalouge);
            }

            // Send IM using KandyCSharp Library
            SendMessageToDevice(hypercatCatalouge, kandyDeviceId);

            Console.WriteLine ("Press any key to exit!");
			Console.ReadLine ();
		}

	    private static void SendMessageToDevice(string hypercatCatalouge, string deviceUniqueId)
	    {
            var userAccessTokenService = new UserAccessTokenService();
            var userAccessToken = userAccessTokenService.GetUserAccessToken("user2");

            var deviceService = new DeviceService();
            var message = new MessageToSendRoot
            {
                MessageToSend = new MessageToSend
                {
                    Message = new Message
                    {
                        MimeType = "application/json",
                        Text = hypercatCatalouge
                    },
                    UUID = deviceUniqueId,
                    ContentType = "text",
                    Destination = ConfigurationManager.AppSettings["DestinationUser"]
                }
            };

            // Send a message to device
	        var messageToSend = JsonConvert.SerializeObject(message);
            Console.WriteLine("Sending: {0}", messageToSend);
            Console.WriteLine("User Access Token: {0}", userAccessToken);
            Console.WriteLine("Device ID: {0}", deviceUniqueId);

            var status = deviceService.SendIm(userAccessToken,
                deviceUniqueId, JsonConvert.SerializeObject(message));
            Console.WriteLine("Status: {0}", status);
        }

        private static string BuildHypercatCatalouge(string cityName, WeatherEntity weatherEntity)
	    {
            var items = new ItemCollection
            {
                new Item
                {
                    ItemMetadata = HypercatWeatherDataHelper.GetItemMetaDataCollection(cityName, weatherEntity),
                    Href = "/"+cityName
                }
            };

            var catelouge = new Catalogue
            {
                CatalogueMetaData = HypercatWeatherDataHelper.GetCatalogueMetaDataCollection(),
                Items = items
            };

            var hyperCatBuilder = new HyperCatBuilder();
            hyperCatBuilder.AddCatalogueMetaData(catelouge.CatalogueMetaData);
            hyperCatBuilder.AddCatalogueItem(items);
            return hyperCatBuilder.ToString();
        }

        public static WeatherEntity GetWeatherCondition(string apiKey, string city)
		{
			var weatherEntity = new WeatherEntity();
			using(var weatherXmlReader = new XmlTextReader($"http://api.previmeteo.com/{apiKey}/ig/api?weather={city}"))
			{
				var doc = new XmlDocument();
				doc.Load(weatherXmlReader);

				if (doc.SelectSingleNode("xml_api_reply/weather/problem_cause") != null)
				{
					weatherEntity = null;
				}
				else
				{
					weatherEntity.City = doc.SelectSingleNode("/xml_api_reply/weather/forecast_information/city").Attributes["data"].InnerText;
					weatherEntity.Condition = doc.SelectSingleNode("/xml_api_reply/weather/current_conditions/condition").Attributes["data"].InnerText;
					weatherEntity.TempInCentigrade = doc.SelectSingleNode("/xml_api_reply/weather/current_conditions/temp_c").Attributes["data"].InnerText;
					weatherEntity.TempInFahrenheit = doc.SelectSingleNode("/xml_api_reply/weather/current_conditions/temp_f").Attributes["data"].InnerText;
					weatherEntity.Humidity = doc.SelectSingleNode("/xml_api_reply/weather/current_conditions/humidity").Attributes["data"].InnerText;
					weatherEntity.Wind = doc.SelectSingleNode("/xml_api_reply/weather/current_conditions/wind_condition").Attributes["data"].InnerText;
				}
			}
			return weatherEntity;
		}
	}
}
