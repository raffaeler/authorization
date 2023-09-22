namespace AuthzDeviceGateway.Models;

public interface IDeviceCapabilities
{
    string Name { get; }
    int NumInputs { get; }
    int NumOutputs { get; }
}
