using System.Text.RegularExpressions;
using Infrastructure.Generators;
using Newtonsoft.Json.Linq;

namespace Infrastructure.JsonProcessing;

public class IdNodesSetter
{
    private readonly IdGenerator _idGenerator;
    
    private readonly Dictionary<string, string> _idCache;

    public IdNodesSetter(IdGenerator idGenerator)
    {
        _idGenerator = idGenerator;
        _idCache = new Dictionary<string, string>();
    }
    
    public string SearchAndSetIdNodes(string json)
    {
        JToken jsonToken = JToken.Parse(json);
        
        string pattern = @"%%IDNode_(\d+)%%";
        
        ReplaceIdNodes(jsonToken, pattern);
        
        return jsonToken.ToString();
    }
    
    private void ReplaceIdNodes(JToken token, string pattern)
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
                        
                        if (!_idCache.TryGetValue(idNode, out string newValue))
                        {
                            newValue = _idGenerator.GenerateSecureIdNodes();
                            _idCache[idNode] = newValue;
                        }
                        
                        property.Value = newValue;
                    }
                }
                else
                {
                    ReplaceIdNodes(property.Value, pattern);
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
                        
                        if (!_idCache.TryGetValue(idNode, out string newValue))
                        {
                            newValue = _idGenerator.GenerateSecureIdNodes();
                            _idCache[idNode] = newValue;
                        }
                        
                        array[i] = newValue;
                    }
                }
                else
                {
                    ReplaceIdNodes(array[i], pattern);
                }
            }
        }
    }
}
