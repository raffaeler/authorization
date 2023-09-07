namespace AuthzDeviceGateway.Models;

public record DeviceInput(int Id,
    int DeviceObjectId,
    string[] Values);
