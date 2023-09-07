using AuthzDeviceGateway.Authorization;
using AuthzDeviceGateway.Data;
using AuthzDeviceGateway.Models;

using CommonAuth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace AuthzDeviceGateway.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository _repository;
    private readonly AuthServerConfiguration _authServerConfiguration;

    public IndexModel(ILogger<IndexModel> logger,
        IAuthorizationService authorizationService,
        IOptions<AuthServerConfiguration> authServerConfigurationOption,
        IRepository repository)
    {
        _logger = logger;
        _authorizationService = authorizationService;
        _repository = repository;
        _authServerConfiguration = authServerConfigurationOption.Value;
    }

    public ICollection<DeviceObject> GetDevices()
    {
        return _repository.DeviceObjects;
    }

    public void OnGetAsync()
    {
        //this.PageContext.
    }

    public IActionResult OnGetLoginAsync()
    {
        if (this.User.Identity?.IsAuthenticated == true)
        {
            // extension method defined in CommonAuth
            return this.SignOut(_authServerConfiguration);
        }
        else
        {
            // extension method defined in CommonAuth
            return this.SignIn(_authServerConfiguration);
        }
    }

    //public IActionResult OnGetLogoutAsync()
    //{
    //}

    //public void OnPost()
    //{
    //}

    public void OnPostAsync()
    {
    }

    public void OnPostLoginAsync()
    {
    }


}
