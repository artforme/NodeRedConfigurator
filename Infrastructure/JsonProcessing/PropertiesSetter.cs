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
                if (property.Value.Type == JTokenType.String)
                {
                    string currentValue = property.Value.ToString();
                    if (currentValue.Contains(search))
                    {
                        property.Value = currentValue.Replace(search, replace);
                    }
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
                if (array[i].Type == JTokenType.String)
                {
                    string currentValue = array[i].ToString();
                    if (currentValue.Contains(search))
                    {
                        array[i] = currentValue.Replace(search, replace);
                    }
                }
                else
                {
                    SearchAndSetProperties(array[i], search, replace);
                }
            }
        }
    }
}