namespace LoggingWithOpenTelemetry.Api.Services;

public interface IUserService
{
    bool Login(string username, string password);
}
