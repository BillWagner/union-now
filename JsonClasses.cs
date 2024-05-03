using System.Text;
using System.Text.Json.Serialization;

namespace UnionsNow
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class SensorDetails
    {
        public double temperature { get; set; }
        public string units { get; set; }
        public string status { get; set; }
        public bool smoke { get; set; }
        public string entry { get; set; }
        public bool motion { get; set; }
    }

    public class Sensor
    {
        public int id { get; set; }
        public string model { get; set; }
        public string status { get; set; }
        public string location { get; set; }
        public List<SensorDetails> data { get; set; }

        public List<string> open { get; set; }
        public List<string> closed { get; set; }

        public override string ToString() => $"location: {location}\tstatus: {status}";
    }

    public class Root
    {
        [JsonPropertyName("overall-status")]
        public string OverallStatus { get; set; }
        public List<Sensor> sensors { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder($"Overall status: {OverallStatus}");
            stringBuilder.AppendLine("Individual Sensor:");
            foreach (Sensor sensor in sensors)
            {
                stringBuilder.AppendLine($"  {sensor.ToString()}");
            }
            return stringBuilder.ToString();
        }
    }
}
