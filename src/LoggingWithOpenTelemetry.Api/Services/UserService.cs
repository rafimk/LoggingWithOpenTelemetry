using LoggingWithOpenTelemetry.Api.Models;
using System.Diagnostics;

namespace LoggingWithOpenTelemetry.Api.Services;

public class UserService(ILogger<UserService> logger) : IUserService
{
    private static readonly List<User> _user = new()
    {
        new User("codemaze", "P@ssw0rd")
    };

    private readonly ActivitySource _activitySource = new("Tracing.Net");

    public bool Login(string username, string password)
    {
        using var activity = _activitySource.StartActivity("Login");

        using var _ = logger.BeginScope(new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("username", username)
        });

        logger.LogInformation("Searching for user");

        if (_user.Any(x => x.Equals(new(username, password))))
        {
            logger.LogInformation("User found, loggin in");
            return true;
        }

        logger.LogWarning("User not found");
        return false;
    }
}
