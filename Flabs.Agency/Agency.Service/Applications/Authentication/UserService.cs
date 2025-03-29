using Agency.Service.Common;

namespace Agency.Service.Authentication;

public interface IUserService
{
    Task<User?> ValidateUserAsync(string userid, string password);
}
public class UserService : IUserService
{
    private readonly Dictionary<string, User> _users = new ()
    {
        {
            "agent1", new User
            {
                Id = "agent1",
                Name = "Agent 1 Name",
                Role = RoleName.Agent,
                Password = "12345"
            }
        },
        {
            "agent2", new User
            {
                Id = "agent2",
                Name = "Agent 2 Name",
                Role = RoleName.Agent,
                Password = "12345"
            }
        },
        {
            "agent3", new User
            {
                Id = "agent3",
                Name = "Fake 3 Name",
                Role = RoleName.FakeAgent,
                Password = "12345"
            }
        }
    };
    
    public Task<User?> ValidateUserAsync(string userid, string password)
    {
        if (_users.TryGetValue(userid, out var user) && user.Password == password)
            return Task.FromResult<User?>(user);

        return Task.FromResult<User?>(null);
    }
}

public record User
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } =  string.Empty;
    public string Password { get; set; } =  string.Empty;
}
