using Xunit;
using Infrastructure.Managers;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Tests;

public class ConfigManagerTests : IDisposable
{
    private readonly string _testDir;

    public ConfigManagerTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "ConfigManagerTest");
        Directory.CreateDirectory(_testDir);
        Directory.SetCurrentDirectory(_testDir);
    }

    [Fact]
    public void Constructor_CreatesTemplatesFolderAndConfigFile()
    {
        var configManager = new ConfigManager(_testDir);

        Assert.True(Directory.Exists(configManager.GetTemplatesFolderPath()));
        Assert.True(File.Exists(Path.Combine(_testDir, "templates.json")));
        Assert.Equal("{}", File.ReadAllText(Path.Combine(_testDir, "templates.json")));
    }

    [Fact]
    public void SetTemplatePath_SavesRelativePath()
    {
        var configManager = new ConfigManager(_testDir);
        string fullPath = Path.Combine(configManager.GetTemplatesFolderPath(), "test.json");
        File.WriteAllText(fullPath, "{}");

        configManager.SetTemplatePath("RgbStrip", "Alice", fullPath);

        string configContent = File.ReadAllText(Path.Combine(_testDir, "templates.json"));
        var config = JObject.Parse(configContent);
        Assert.Equal("test.json", config["Templates"]?["RgbStrip"]?["Alice"]?.Value<string>());
    }

    [Fact]
    public void GetTemplatePath_ReturnsFullPath()
    {
        var configManager = new ConfigManager(_testDir);
        string fullPath = Path.Combine(configManager.GetTemplatesFolderPath(), "test.json");
        File.WriteAllText(fullPath, "{}");
        configManager.SetTemplatePath("RgbStrip", "Alice", fullPath);

        string loadedPath = configManager.GetTemplatePath("RgbStrip", "Alice");

        Assert.Equal(fullPath, loadedPath);
    }

    [Fact]
    public void GetTemplatePath_NoPath_ReturnsNull()
    {
        var configManager = new ConfigManager(_testDir);

        string loadedPath = configManager.GetTemplatePath("RgbStrip", "Alice");

        Assert.Null(loadedPath);
    }

    [Fact]
    public void SetTemplatePath_OutsideTemplatesFolder_ThrowsException()
    {
        var configManager = new ConfigManager(_testDir);
        string invalidPath = Path.Combine(_testDir, "invalid.json");

        var exception = Assert.Throws<ArgumentException>(() => configManager.SetTemplatePath("RgbStrip", "Alice", invalidPath));
        Assert.Contains("Template file must be located in the 'templates' folder", exception.Message);
    }

    [Fact]
    public void GetAllChainTypes_ReturnsChainTypes()
    {
        var configManager = new ConfigManager(_testDir);
        string fullPath = Path.Combine(configManager.GetTemplatesFolderPath(), "test.json");
        File.WriteAllText(fullPath, "{}");
        configManager.SetTemplatePath("RgbStrip", "Alice", fullPath);
        configManager.SetTemplatePath("Type1", "Apple", fullPath);

        var chainTypes = configManager.GetAllChainTypes();

        Assert.Contains("RgbStrip", chainTypes);
        Assert.Contains("Type1", chainTypes);
        Assert.Equal(2, chainTypes.Count());
    }

    public void Dispose()
    {
        // if (Directory.Exists(_testDir))
        // {
        //     Directory.SetCurrentDirectory(Path.GetTempPath());
        //     Directory.Delete(_testDir, true);
        // }
    }
}