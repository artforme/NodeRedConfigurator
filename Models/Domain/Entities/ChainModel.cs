using Models.Domain.ValueOjbects;

namespace Models.Domain.Entities;

public class ChainModel
{
    public Address LocalAddress { get; }
    public Info DeviceType { get; }
    public Info LocalParameter { get; }
    public Info EndpointType { get; }
    public Info Name { get; }
    public bool GenerateForAlice { get; }
    public bool GenerateForHomeKit { get; }

    public ChainModel(
        Address localAddress,
        Info deviceType, 
        Info localParameter, 
        Info endpointType, 
        Info name, 
        bool generateForAlice, 
        bool generateForKit)
    {
        LocalAddress = localAddress;
        DeviceType = deviceType;
        LocalParameter = localParameter;
        EndpointType = endpointType;
        Name = name;
        GenerateForAlice = generateForAlice;
        GenerateForHomeKit = generateForKit;
    }
}