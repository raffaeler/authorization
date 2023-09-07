using AuthzDeviceGateway.Authorization;
using AuthzDeviceGateway.Data;
using AuthzDeviceGateway.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthzDeviceGateway.Pages;

[DeviceConfigureAuthorize]
public class DeviceAdminModel : PageModel
{
    private readonly ILogger<DeviceAdminModel> _logger;
    private readonly IRepository _repository;

    public DeviceAdminModel(ILogger<DeviceAdminModel> logger,
        IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    public void OnGet()
    {
        _repository.DeviceTypes.Add(new DeviceType(
            1, "DHT11", "Temperature sensor with 2 Celsius precision", 1, 0));
    }
}
