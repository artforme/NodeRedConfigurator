using System.Text.RegularExpressions;
using Infrastructure.Generators;
using Newtonsoft.Json.Linq;

namespace Infrastructure.JsonProcessing;

public class IdNodesSetter
{
    private readonly IdGenerator _idGenerator;

    public IdNodesSetter(IdGenerator idGenerator)
    {
        _idGenerator = idGenerator;
    }
    
    public string SearchAndSetIdNodes(JToken jsonToken)
    {
        string pattern = @"%%IDNode_(\d+)%%|%%IDNode%%";
        var idCache = new Dictionary<string, string>(); // Новый кэш для каждой цепочки
        
        ReplaceIdNodes(jsonToken, pattern, idCache);
        
        return jsonToken.ToString();
    }
    
    private void ReplaceIdNodes(JToken token, string pattern, Dictionary<string, string> idCache)
    {
        if (token is JObject obj)
        {
            foreach (var property in obj.Properties())
            {
                if (property.Value.Type == JTokenType.String)
                {
                    string value = property.Value.ToString();
                    Match match = Regex.Match(value, pattern);
                    if (match.Success)
                    {
                        string idNode = match.Value;
                        
                        if (!idCache.TryGetValue(idNode, out string newValue))
                        {
                            newValue = _idGenerator.GenerateSecureIdNodes();
                            idCache[idNode] = newValue;
                        }
                        
                        property.Value = newValue;
                    }
                }
                else
                {
                    ReplaceIdNodes(property.Value, pattern, idCache);
                }
            }
        }
        else if (token is JArray array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Type == JTokenType.String)
                {
                    string value = array[i].ToString();
                    Match match = Regex.Match(value, pattern);
                    if (match.Success)
                    {
                        string idNode = match.Value;
                        
                        if (!idCache.TryGetValue(idNode, out string newValue))
                        {
                            newValue = _idGenerator.GenerateSecureIdNodes();
                            idCache[idNode] = newValue;
                        }
                        
                        array[i] = newValue;
                    }
                }
                else
                {
                    ReplaceIdNodes(array[i], pattern, idCache);
                }
            }
        }
    }
}
