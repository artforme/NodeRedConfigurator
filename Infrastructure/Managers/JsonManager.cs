using Infrastructure.Generators;
using Infrastructure.JsonProcessing;
using Models.Domain.Entities;

namespace Infrastructure.Managers;

public class JsonManager
{
    private readonly ChainManager _chainManager;
    
    private readonly TemplateManager _templateManager;

    private readonly IdGenerator _idGenerator;

    private readonly CoordinateSetter _coordinateSetter;

    private readonly IdNodesSetter _idNodesSetter;

    private readonly PropertiesSetter _propertiesSetter;

    private readonly GlobalSettings _globalSettings;
    

    public JsonManager(
        ChainManager chainManager,
        TemplateManager templateManager,
        IdGenerator idGenerator,
        CoordinateSetter coordinateSetter,
        IdNodesSetter idNodesSetter,
        PropertiesSetter propertiesSetter,
        GlobalSettings globalSettings
        )
    {
        _chainManager = chainManager;
        _templateManager = templateManager;
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