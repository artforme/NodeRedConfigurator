using Newtonsoft.Json.Linq;

namespace Infrastructure.JsonProcessing;

public class PropertiesSetter
{
    public void SearchAndSetProperties(JToken token, string search, string replace)
    {
        if (token is JObject obj)
        {
            foreach (var property in obj.Properties())
            {
                if (property.Value.Type == JTokenType.String && property.Value.ToString() == search)
                {
                    property.Value = replace;
                }
                else
                {
                    SearchAndSetProperties(property.Value, search, replace);
                }
            }
        }
        else if (token is JArray array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Type == JTokenType.String && array[i].ToString() == search)
                {
                    array[i] = replace;
                }
                else
                {
                    SearchAndSetProperties(array[i], search, replace);
                }
            }
        }
    }
}