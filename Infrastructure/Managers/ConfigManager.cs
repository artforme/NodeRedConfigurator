using Models.Domain.ValueObjects;
using Newtonsoft.Json.Linq;

public class ConfigManager
{
    public Info ConfigFilePath { get; }
    public Info TemplatesFolderPath { get; }

    public ConfigManager(string testDirectory = null)
    {
        string basePath = testDirectory ?? Directory.GetCurrentDirectory();
        ConfigFilePath = new Info(Path.Combine(basePath, "templates.json"));
        TemplatesFolderPath = new Info(Path.Combine(basePath, "templates"));

        if (!Directory.Exists(TemplatesFolderPath.Value))
        {
            Directory.CreateDirectory(TemplatesFolderPath.Value);
        }

        if (!File.Exists(ConfigFilePath.Value))
        {
            File.WriteAllText(ConfigFilePath.Value, @"{
                      ""Templates"": {
                        ""Connection"": {
                          ""Non platform"": ""Подключения + вкладка.json""
                        },
                        ""RgbStrip"": {
                          ""Alice"": ""RGBStripAlice.json"",
                          ""Apple"": ""RGBStripHomeKit.json""
                        },
                        ""SimpleLightOnOff"": {
                          ""Alice"": ""Лампа простая.json"",
                          ""Apple"": ""Лампа простая HomeKit.json""
                        },
                        ""WhiteStrip"": {
                          ""Alice"": ""White лента.json"",
                          ""Apple"": ""White лента HomeKit.json""
                        },
                        ""Dehumidifier"": {
                          ""Alice"": ""Осушитель.json"",
                          ""Apple"": ""Осушитель HomeKit.json""
                        },
                        ""Socket"": {
                          ""Alice"": ""Розетки.json"",
                          ""Apple"": ""Розетки HomeKit.json""
                        },
                        ""WarmFloor"": {
                          ""Alice"": ""Теплый пол.json"",
                          ""Apple"": ""Теплый пол HomeKit.json""
                        }
                      }
                    }");
        }
    }

    public string GetTemplatePath(string chainType, string platform)
    {
        var config = JObject.Parse(File.ReadAllText(ConfigFilePath.Value));
        string relativePath = config["Templates"]?[chainType]?.Value<string>(platform);
        return string.IsNullOrEmpty(relativePath) ? null : Path.Combine(TemplatesFolderPath.Value, relativePath);
    }

    public void SetTemplatePath(string chainType, string platform, string fullPath)
    {
        if (!fullPath.StartsWith(TemplatesFolderPath.Value))
        {
            throw new ArgumentException(
                $"Template file must be located in the 'templates' folder: {TemplatesFolderPath}");
        }

        var config = JObject.Parse(File.ReadAllText(ConfigFilePath.Value));
        if (config["Templates"] == null)
        {
            config["Templates"] = new JObject();
        }

        if (config["Templates"][chainType] == null)
        {
            config["Templates"][chainType] = new JObject();
        }

        string relativePath = Path.GetFileName(fullPath);
        config["Templates"][chainType][platform] = relativePath;
        File.WriteAllText(ConfigFilePath.Value, config.ToString());
    }

    public IEnumerable<string> GetAllChainTypes()
    {
        var config = JObject.Parse(File.ReadAllText(ConfigFilePath.Value));
        var templates = config["Templates"] as JObject;
        return templates?.Properties().Select(p => p.Name) ?? Enumerable.Empty<string>();
    }

    // Новый метод: проверка, является ли тип цепочки платформозависимым
    public bool IsPlatformDependent(string chainType)
    {
        var config = JObject.Parse(File.ReadAllText(ConfigFilePath.Value));
        var typeConfig = config["Templates"]?[chainType] as JObject;
        return typeConfig != null && typeConfig["Alice"] != null && typeConfig["Apple"] != null;
    }

    public string GetTemplatesFolderPath() => TemplatesFolderPath.Value;
}