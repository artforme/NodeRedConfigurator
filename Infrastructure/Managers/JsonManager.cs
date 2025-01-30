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

    private readonly GlobalSettings _globalSettings;
    

    public JsonManager(
        IdGenerator idGenerator,
        CoordinateSetter coordinateSetter,
        IdNodesSetter idNodesSetter,
        PropertiesSetter propertiesSetter,
        GlobalSettings globalSettings
        )
    {
        _idGenerator = idGenerator;
        _coordinateSetter = coordinateSetter;
        _idNodesSetter = idNodesSetter;
        _propertiesSetter = propertiesSetter;
        _globalSettings = globalSettings;
    }

    public void Run()
    {
        throw new NotImplementedException();
    }

}