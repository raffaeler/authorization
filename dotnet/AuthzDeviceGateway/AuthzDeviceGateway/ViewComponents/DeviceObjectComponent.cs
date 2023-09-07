using AuthzDeviceGateway.Data;

using Microsoft.AspNetCore.Mvc;

namespace AuthzDeviceGateway.ViewComponents;

public class DeviceObjectComponent : ViewComponent
{
    private readonly IRepository _repository;

    public DeviceObjectComponent(IRepository repository)
    {
        _repository = repository;
    }

    public Task<IViewComponentResult> InvokeAsync(int id)
    {
        var item = _repository.DeviceObjects.FirstOrDefault(d => d.Id == id);
        return Task.FromResult<IViewComponentResult>(View(item));
    }
}
