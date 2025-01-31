using Newtonsoft.Json.Linq;

namespace Infrastructure.Managers;

public class ConfigManager
{
    private readonly string _configFilePath;

    public ConfigManager(string configFilePath)
    {
        _configFilePath = configFilePath;
        if (!File.Exists(_configFilePath))
        {
            File.WriteAllText(_configFilePath, "{}");
        }
    }

    public string GetTemplatePath(string chainType, string platform)
    {
        var config = JObject.Parse(File.ReadAllText(_configFilePath));
        return config["Templates"]?[chainType]?.Value<string>(platform);
    }

    public void SetTemplatePath(string chainType, string platform, string templateFileName)
    {
        var config = JObject.Parse(File.ReadAllText(_configFilePath));
        if (config["Templates"] == null)
        {
            config["Templates"] = new JObject();
        }

        if (config["Templates"][chainType] == null)
        {
            config["Templates"][chainType] = new JObject();
        }

        config["Templates"][chainType][platform] = templateFileName;
        File.WriteAllText(_configFilePath, config.ToString());
    }
}