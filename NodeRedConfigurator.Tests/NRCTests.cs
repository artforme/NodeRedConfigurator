using Xunit;
using Infrastructure.Generators;
using Infrastructure.JsonProcessing;
using Newtonsoft.Json.Linq;

namespace Tests;

public class IdGeneratorTests
{
    [Fact]
    public void GenerateSecureIdNodes_DefaultLength_ReturnsValidId()
    {
        var idGenerator = new IdGenerator();
        
        var id = idGenerator.GenerateSecureIdNodes();
        
        Assert.NotNull(id);
        Assert.Equal(16, id.Length);
    }

    [Fact]
    public void GenerateSecureIdNodes_CustomLength_ReturnsValidId()
    {
        var idGenerator = new IdGenerator();
        
        var id = idGenerator.GenerateSecureIdNodes(8);
        
        Assert.NotNull(id);
        Assert.Equal(8, id.Length);
    }
}

public class CoordinateSetterTests
{
    [Fact]
    public void UpdateCoordinates_UpdatesYValuesCorrectly()
    {
        var coordinateSetter = new CoordinateSetter();
        var json = @"[
                { 'tab': 'flow1', 'height': 10, 'template': [{ 'y': 5 }] },
                { 'tab': 'flow2', 'height': 20, 'template': [{ 'y': 10 }] }
            ]";
        var data = JArray.Parse(json);
        var flowCounters = new Dictionary<string, int>();
        
        coordinateSetter.UpdateCoordinates(data, flowCounters);
        
        Assert.Equal(15, data[0]["template"][0]["y"].Value<int>()); // 5 + 10
        Assert.Equal(30, data[1]["template"][0]["y"].Value<int>()); // 10 + 20
    }

    [Fact]
    public void UpdateCoordinates_UpdatesYValuesSameFlowsCorrectly()
    {
        var coordinateSetter = new CoordinateSetter();
        var json = @"[
                { 'tab': 'flow1', 'height': 10, 'template': [{ 'y': 5 }] },
                { 'tab': 'flow1', 'height': 20, 'template': [{ 'y': 10 }] }
            ]";
        var data = JArray.Parse(json);
        var flowCounters = new Dictionary<string, int>();
        
        coordinateSetter.UpdateCoordinates(data, flowCounters);
        
        Assert.Equal(15, data[0]["template"][0]["y"].Value<int>()); // 5 + 10
        Assert.Equal(40, data[1]["template"][0]["y"].Value<int>()); // 10 + 30
    }
    
    [Fact]
    public void UpdateCoordinates_UpdatesYValuesRealJsonCorrectly()
    {
        var coordinateSetter = new CoordinateSetter();
        var json = @"[
            {
                ""Type"": ""Lamp ON/OFF"",
                ""device"": ""wb-mr6c"",
                ""tab"": ""HomeKit"",
                ""height"": 40,
                ""template"": [
                    {
                        ""id"": ""%%IDNode%%"",
                        ""type"": ""wirenboard-in"",
                        ""z"": ""%%IDTab%%"",
                        ""name"": ""%%name%%"",
                        ""server"": ""%%MQTTserver%%"",
                        ""channel"": [
                            ""/devices/%%device%%_%%dev_address%%/controls/%%control%%""
                        ],
                        ""outputAtStartup"": true,    
                        ""x"": 260,
                        ""y"": 70,
                        ""wires"": [
                            [
                                ""%%IDNode_1%%""
                            ]
                        ]
                    }
                ]
            }
        ]";
        var data = JArray.Parse(json);
        var flowCounters = new Dictionary<string, int>();
        
        coordinateSetter.UpdateCoordinates(data, flowCounters);
        
        Assert.Equal(110, data[0]["template"][0]["y"].Value<int>()); // 40+70
    }
    
    
}

public class IdNodesSetterTests
{
    [Fact]
    public void SearchAndSetIdNodes_ReplacesPlaceholdersCorrectly()
    {
        var idGenerator = new IdGenerator();
        var idNodesSetter = new IdNodesSetter(idGenerator);
        var json = "{ 'node': '%%IDNode_123%%' }";
        
        var result = idNodesSetter.SearchAndSetIdNodes(json);
        
        Assert.DoesNotContain("%%IDNode_123%%", result);
    }
    
    [Fact]
    public void SearchAndSetIdNodes_ReplacesPlaceholdersCorrectly_NodeInside4Arrays()
    {
        var idGenerator = new IdGenerator();
        var idNodesSetter = new IdNodesSetter(idGenerator);
        var json = @"
    {
        'array1': [
            {
                'array2': [
                    {
                        'array3': [
                            {
                                'array4': [
                                    {
                                        'node': '%%IDNode_123%%'
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }
        ]
    }";
    
        var result = idNodesSetter.SearchAndSetIdNodes(json);
        
        Assert.DoesNotContain("%%IDNode_123%%", result);
    }
}


public class PropertiesSetterTests
{
    [Fact]
    public void SearchAndSetProperties_ReplacesValuesCorrectly()
    {
        var propertiesSetter = new PropertiesSetter();
        var json = "{ 'key': 'oldValue' }";
        var token = JToken.Parse(json);
        
        propertiesSetter.SearchAndSetProperties(token, "oldValue", "newValue");
        
        Assert.Equal("newValue", token["key"].Value<string>());
    }
    
    [Fact]
    public void SearchAndSetProperties_ReplacesValuesCorrectly_KeyInside7Arrays()
    {
        var propertiesSetter = new PropertiesSetter();
        var json = @"
    {
        'array1': [
            {
                'array2': [
                    {
                        'array3': [
                            {
                                'array4': [
                                    {
                                        'array5': [
                                            {
                                                'array6': [
                                                    {
                                                        'array7': [
                                                            {
                                                                'key': 'oldValue'
                                                            }
                                                        ]
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }
        ]
    }";
        var token = JToken.Parse(json);
    
        propertiesSetter.SearchAndSetProperties(token, "oldValue", "newValue");
        
        Assert.Equal("newValue", token["array1"][0]["array2"][0]["array3"][0]["array4"][0]["array5"][0]["array6"][0]["array7"][0]["key"].Value<string>());
    }
}