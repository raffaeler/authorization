namespace AuthzDeviceGateway.Models;

public record DeviceType(
    int Id,
    string Name,
    string Description,
    int NumInputs,
    int NumOutputs) : IDeviceCapabilities;
