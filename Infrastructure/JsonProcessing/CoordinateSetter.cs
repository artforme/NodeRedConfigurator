using Newtonsoft.Json.Linq;

namespace Infrastructure.JsonProcessing;

public class CoordinateSetter
{
    private const int DefaultPadding = 100;

    public void UpdateCoordinates(JArray data, Dictionary<string, int> flowCounters, string platform)
    {
        foreach (JObject item in data)
        {
            if (!item.ContainsKey("height") || item["height"] == null)
            {
                return;
            }

            int height = item["height"].Value<int>();
            
            if (!flowCounters.ContainsKey(platform))
            {
                flowCounters[platform] = 0;
            }
            
            if (item["template"] is JArray template)
            {
                foreach (JObject templateItem in template)
                {
                    if (templateItem.ContainsKey("y"))
                    {
                        int currentY = templateItem["y"].Value<int>();
                        templateItem["y"] = currentY + flowCounters[platform];
                    }
                }
            }
            
            flowCounters[platform] += height + DefaultPadding;
        }
    }
}