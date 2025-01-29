using Infrastructure.Generators;
using Infrastructure.JsonProcessing;
using Models.Domain.Entities;
using Models.Domain.ValueOjbects;

namespace Infrastructure.Managers;

public class JsonManager
{
    private readonly Dictionary<Guid, Chain> _chains = new Dictionary<Guid, Chain>();

    private readonly IdGenerator _idGenerator;

    private readonly CoordinateSetter _coordinateSetter;

    private readonly IdNodesSetter _idNodesSetter;

    private readonly PropertiesSetter _propertiesSetter;

    private readonly Info _aliceFlowId;
    
    private readonly Info _appleFlowId;

    private readonly Info _cloudId;

    private readonly Info _bridgeId;

    private readonly Info _mqttServer;

    private readonly Info _model;

    private readonly Info _serialNumber;
    

    public JsonManager(
        IdGenerator idGenerator,
        CoordinateSetter coordinateSetter,
        IdNodesSetter idNodesSetter,
        PropertiesSetter propertiesSetter,
        Info model,
        Info serialNumber)
    {
        _idGenerator = idGenerator;
        _coordinateSetter = coordinateSetter;
        _idNodesSetter = idNodesSetter;
        _propertiesSetter = propertiesSetter;
        _model = model;
        _serialNumber = serialNumber;
        _aliceFlowId = new Info(idGenerator.GenerateSecureIdNodes());
        _appleFlowId = new Info(idGenerator.GenerateSecureIdNodes());
        _cloudId = new Info(idGenerator.GenerateSecureIdNodes());
        _bridgeId = new Info(idGenerator.GenerateSecureIdNodes());
        _mqttServer = new Info(idGenerator.GenerateSecureIdNodes());
    }

    public void Run()
    {
        throw new NotImplementedException();
    }

}