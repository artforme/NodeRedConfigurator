using Infrastructure.JsonProcessing;
using Infrastructure.Logging;
using Infrastructure.Managers;
using Models.Domain.Entities;
using Newtonsoft.Json.Linq;

public class JsonManager
{
    private readonly TemplateManager _templateManager;
    private readonly CoordinateSetter _coordinateSetter;
    private readonly IdNodesSetter _idNodesSetter;
    private readonly PropertiesSetter _propertiesSetter;
    private readonly GlobalSettings _globalSettings;
    private readonly ILogger _logger;

    public JsonManager(
        TemplateManager templateManager,
        CoordinateSetter coordinateSetter,
        IdNodesSetter idNodesSetter,
        PropertiesSetter propertiesSetter,
        GlobalSettings globalSettings,
        ILogger logger)
    {
        _templateManager = templateManager;
        _coordinateSetter = coordinateSetter;
        _idNodesSetter = idNodesSetter;
        _propertiesSetter = propertiesSetter;
        _globalSettings = globalSettings;
        _logger = logger;
    }

    public JArray GenerateJson(IEnumerable<Chain> chains, string platform)
    {
        var finalResult = new JArray();
        var flowCounters = new Dictionary<string, int>(); // Один словарь для всех цепочек
        foreach (var chain in chains)
        {
            var nodes = ProcessTemplate(chain.Type.Value, platform, chain, flowCounters);
            foreach (var node in nodes)
            {
                finalResult.Add(node);
            }
            _logger.Info($"Added {nodes.Count} nodes for {chain.Type.Value} ({chain.Id}), total: {finalResult.Count}");
        }
        return finalResult;
    }
    
    public JArray GenerateConnectionsJson()
    {
        var flowCounters = new Dictionary<string, int>();
        return ProcessTemplate("Connection", "Non platform", null, flowCounters);
    }

    private JArray ProcessTemplate(string chainType, string platform, Chain chain, Dictionary<string, int> flowCounters)
    {
        _logger.Info($"Processing template: {chainType} for platform: {platform}{(chain != null ? $" with chain: {chain.Id}" : "")}");
        var template = _templateManager.LoadTemplate(chainType, platform);
        JObject chainJson;

        if (template is JObject obj)
        {
            chainJson = (JObject)obj.DeepClone();
        }
        else if (template is JArray arr && arr.Count > 0 && arr[0] is JObject sourceObj)
        {
            chainJson = (JObject)sourceObj.DeepClone();
            if (chainJson["template"] == null)
            {
                chainJson["template"] = new JArray(sourceObj["template"]?.DeepClone());
            }
        }
        else
        {
            throw new InvalidOperationException($"Invalid template format for {chainType} ({platform}).");
        }

        _idNodesSetter.SearchAndSetIdNodes(chainJson);
        if (chain != null)
        {
            var chainProperties = chain.GetProperties();
            foreach (var (search, replace) in chainProperties)
            {
                _propertiesSetter.SearchAndSetProperties(chainJson, search, replace.ToString());
            }
        }
        
        _propertiesSetter.SearchAndSetProperties(chainJson, "%%IDTabApple%%", _globalSettings.AppleFlowId.Value);
        _propertiesSetter.SearchAndSetProperties(chainJson, "%%IDTabAlice%%", _globalSettings.AliceFlowId.Value);
        _propertiesSetter.SearchAndSetProperties(chainJson, "%%MQTTserver%%", _globalSettings.MqttServer.Value);
        _propertiesSetter.SearchAndSetProperties(chainJson, "%%model%%", _globalSettings.Model.Value);
        _propertiesSetter.SearchAndSetProperties(chainJson, "%%Serial%%", _globalSettings.SerialNumber.Value);
        _propertiesSetter.SearchAndSetProperties(chainJson, "%%Bridge%%", _globalSettings.BridgeId.Value);
        _propertiesSetter.SearchAndSetProperties(chainJson, "%%Cloud%%", _globalSettings.CloudId.Value);

        var tempArray = new JArray { chainJson };
        _coordinateSetter.UpdateCoordinates(tempArray, flowCounters, platform);
        _logger.Info($"Updated coordinates for {chainType} ({platform}), flowCounters: {string.Join(", ", flowCounters.Select(kv => $"{kv.Key}: {kv.Value}"))}");

        if (chainJson["template"] is JArray templateArray)
        {
            return templateArray;
        }
        
        throw new InvalidOperationException($"Template for {chainType} ({platform}) must contain a 'template' array.");
    }
}