using Newtonsoft.Json.Linq;

namespace Infrastructure.Managers;

public class TemplateManager
{
    private readonly string _templatesFolderPath;
    private readonly ConfigManager _configManager;

    public TemplateManager(string templatesFolderPath, ConfigManager configManager)
    {
        _templatesFolderPath = templatesFolderPath;
        _configManager = configManager;
    }

    public JToken LoadTemplate(string chainType, string platform)
    {
        var templateFileName = _configManager.GetTemplatePath(chainType, platform);
        if (string.IsNullOrEmpty(templateFileName))
        {
            throw new FileNotFoundException($"Template for {chainType} ({platform}) not found in configuration.");
        }

        var templatePath = Path.Combine(_templatesFolderPath, templateFileName);
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file '{templateFileName}' not found in folder '{_templatesFolderPath}'.");
        }

        var json = File.ReadAllText(templatePath);
        return JToken.Parse(json); // Возвращаем JToken, который может быть JObject или JArray
    }
}