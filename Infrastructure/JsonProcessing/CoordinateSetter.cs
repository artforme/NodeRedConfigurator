using Newtonsoft.Json.Linq;

namespace Infrastructure.JsonProcessing;

public class CoordinateSetter
{
    public void UpdateCoordinates(JArray data, Dictionary<string, int> flowCounters)
    {
        foreach (JObject item in data)
        {
            string flow = item["tab"].ToString();
            int height = item["height"].Value<int>();
            
            if (!flowCounters.ContainsKey(flow))
            {
                flowCounters[flow] = height;
            }
            else
            {
                flowCounters[flow] += height;
            }
            
            if (item["template"] is JArray template)
            {
                foreach (JObject templateItem in template)
                {
                    if (templateItem.ContainsKey("y"))
                    {
                        int currentY = templateItem["y"].Value<int>();
                        templateItem["y"] = currentY + flowCounters[flow];
                    }
                }
            }
        }
    }
}