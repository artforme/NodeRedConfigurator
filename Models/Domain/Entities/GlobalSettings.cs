using Models.Domain.ValueObjects;

namespace Models.Domain.Entities;

public class GlobalSettings
{
    public Info AliceFlowId { get; }
    public Info AppleFlowId { get; }
    public Info CloudId { get; }
    public Info BridgeId { get; }
    public Info MqttServer { get; }
    public Info Model { get; }
    public Info SerialNumber { get; }

    public GlobalSettings(
        Info aliceFlowId,
        Info appleFlowId,
        Info cloudId,
        Info bridgeId,
        Info mqttServer,
        Info model,
        Info serialNumber
        )
    {
        AliceFlowId = aliceFlowId;
        AppleFlowId = appleFlowId;
        CloudId = cloudId;
        BridgeId = bridgeId;
        MqttServer = mqttServer;
        Model = model;
        SerialNumber = serialNumber;
    }
}