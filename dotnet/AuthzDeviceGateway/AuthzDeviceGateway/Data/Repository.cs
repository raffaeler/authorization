using System.Text.Json;

using AuthzDeviceGateway.Configuration;
using AuthzDeviceGateway.Models;

using CommonAuth;

using Microsoft.Extensions.Options;

namespace AuthzDeviceGateway.Data;

public interface IRepository
{
    ICollection<DeviceType> DeviceTypes { get; }
    ICollection<DeviceObject> DeviceObjects { get; }
    ICollection<DeviceInput> DeviceInputs { get; }
}

internal record Storage(
    ICollection<DeviceType> DeviceTypes,
    ICollection<DeviceObject> DeviceObjects,
    ICollection<DeviceInput> DeviceInputs);

public class Repository : IRepository
{
    private readonly ILogger<Repository> _logger;
    private readonly RepositoryConfiguration _repositoryConfiguration;

    public Repository(ILogger<Repository> logger,
        IOptions<RepositoryConfiguration> repositoryConfiguration)
    {
        _logger = logger;
        _repositoryConfiguration = repositoryConfiguration.Value;

        DeviceTypes = new List<DeviceType>();
        DeviceObjects = new List<DeviceObject>();
        DeviceInputs = new List<DeviceInput>();

        try
        {
            if (File.Exists(_repositoryConfiguration.Filename))
            {
                var json = File.ReadAllText(_repositoryConfiguration.Filename);
                var storage = JsonSerializer.Deserialize<Storage>(json);
                if (storage?.DeviceTypes != null)
                    DeviceTypes.AddRange(storage.DeviceTypes);
                if (storage?.DeviceObjects != null)
                    DeviceObjects.AddRange(storage.DeviceObjects);
                if (storage?.DeviceInputs != null)
                    DeviceInputs.AddRange(storage.DeviceInputs);


            }
        }
        catch(Exception err)
        {
            _logger.LogError(err, "Error loading data from the json file");
        }
    }

    public ICollection<DeviceType> DeviceTypes { get; }
    public ICollection<DeviceObject> DeviceObjects { get; }
    public ICollection<DeviceInput> DeviceInputs { get; }

    public Task Save()
    {
        _logger.LogInformation($"Saving DeviceTypes:{DeviceTypes.Count} DeviceObjects:{DeviceObjects.Count} DeviceInputs:{DeviceInputs.Count}");

        var storage = new Storage(DeviceTypes, DeviceObjects, DeviceInputs);
        var json = JsonSerializer.Serialize(storage);
        return File.WriteAllTextAsync(json, _repositoryConfiguration.Filename);
    }

}
