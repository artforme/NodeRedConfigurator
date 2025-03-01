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

    // Метод для цепочек
    public JArray GenerateJson(IEnumerable<Chain> chains, string platform)
    {
        var finalResult = new JArray();
        foreach (var chain in chains)
        {
            var nodes = ProcessTemplate(chain.Type.Value, platform, chain);
            foreach (var node in nodes)
            {
                finalResult.Add(node);
            }
        }
        return finalResult;
    }
    
    public JArray GenerateConnectionsJson()
    {
        return ProcessTemplate("Connection");
    }

    // Универсальный метод обработки шаблона
    private JArray ProcessTemplate(string chainType, string platform = "Non platform", Chain chain = null)
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

        // Заменяем ID узлов
        _idNodesSetter.SearchAndSetIdNodes(chainJson);

        // Заменяем свойства из chain.GetProperties(), если цепочка есть
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

        // Обновляем координаты
        var tempArray = new JArray { chainJson };
        var flowCounters = new Dictionary<string, int>();
        _coordinateSetter.UpdateCoordinates(tempArray, flowCounters);

        // Извлекаем template
        if (chainJson["template"] is JArray templateArray)
        {
            return templateArray;
        }
        
        throw new InvalidOperationException($"Template for {chainType} ({platform}) must contain a 'template' array.");

    }
}