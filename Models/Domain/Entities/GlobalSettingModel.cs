using Models.Domain.ValueOjbects;

namespace Models.Domain.Entities;

public class GlobalSettingsModel
{
    public Info CabinetModel { get; set; }
    public Info SerialNumber { get; set; }

    public GlobalSettingsModel(Info cabinet, Info serialNumber)
    {
        CabinetModel = cabinet;
        SerialNumber = serialNumber;
    }
}