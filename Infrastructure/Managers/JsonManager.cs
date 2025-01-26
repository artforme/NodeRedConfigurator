using Infrastructure.Generators;
using Infrastructure.JsonProcessing;
using Models.Domain.Entities;
using Models.Domain.ValueOjbects;

namespace Infrastructure.Managers;

public class JsonManager
{
    private readonly Dictionary<Guid, Chain> _chains = new Dictionary<Guid, Chain>();
    
    private readonly IdGenerator _idGenerator = new IdGenerator();
    
    private readonly CoordinateSetter _coordinateSetter = new CoordinateSetter();
    
    private readonly IdNodesSetter _idNodesSetter = new IdNodesSetter(new IdGenerator());
    
    private readonly PropertiesSetter _propertiesSetter = new PropertiesSetter();

    private readonly Info _aliceFlowId;
    
    private readonly Info _appleFlowId;

    private readonly Info _cloudId;

    private readonly Info _mqttServer;
    
    public Info Model { get; set; }
    
    

    public JsonManager()
    {
        _aliceFlowId = new Info(_idGenerator.GenerateSecureIdNodes());
        _appleFlowId = new Info(_idGenerator.GenerateSecureIdNodes());
        _cloudId = new Info(_idGenerator.GenerateSecureIdNodes());
        _mqttServer = new Info(_idGenerator.GenerateSecureIdNodes());
    }

    public void Run()
    {
        throw new NotImplementedException();
    }

}