namespace Identity.API.DtoModels.Device;

public class DeviceAuthorizationInputModel : ConsentInputModel
{
    public string UserCode { get; set; }
}
