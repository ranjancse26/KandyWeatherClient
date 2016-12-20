using NHyperCat;

namespace WeatherSample
{
    public class HypercatWeatherDataHelper
    {
        public static CatalogueMetaDataCollection GetCatalogueMetaDataCollection()
        {
            return new CatalogueMetaDataCollection
            {
                new CatalogueMetaData
                {
                    val = "application/vnd.hypercat.catalogue+json",
                    rel = "urn:X-hypercat:rels:isContentType"
                },
                new CatalogueMetaData
                {
                    val =  "Hypercat Weather Data",
                    rel =  "urn:X-hypercat:rels:hasDescription:en"
                }
            };
        }

        public static ItemMetaDataCollection GetItemMetaDataCollection(string cityName, WeatherEntity weatherEntity)
        {
            return new ItemMetaDataCollection
            {
                new ItemMetaData {
                    val = cityName,
                    rel = "urn:X-hypercat:rels:hasDescription:en"
                },
                new ItemMetaData {
                    val = weatherEntity.TempInCentigrade,
                    rel = "TemperatureInCentigrade"
                },
                new ItemMetaData {
                    val = weatherEntity.TempInFahrenheit,
                    rel = "TemperatureInFahrenheit"
                },
                new ItemMetaData {
                    val = weatherEntity.Humidity,
                    rel = "Humidity"
                },
                new ItemMetaData {
                    val = weatherEntity.Wind,
                    rel = "Wind"
                },
                new ItemMetaData {
                    val = weatherEntity.Condition,
                    rel = "Condition"
                },
                 new ItemMetaData {
                    val = weatherEntity.High,
                    rel = "High"
                },
                new ItemMetaData {
                    val = weatherEntity.Low,
                    rel = "Low"
                }
            };
        }
    }
}
