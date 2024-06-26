using SpotLights.Core.Interfaces.Identity;
using SpotLights.Domain.Model.Identity;
using SpotLights.Infrastructure.Interfaces.Identity;
using SpotLights.Shared;

namespace SpotLights.Core.Services.Identity;

internal class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserInfo> FindAsync(int id)
    {
        return await _userRepository.FindAsync(id);
    }

    public async Task<UserDto> FirstByIdAsync(int id)
    {
        return await _userRepository.FirstByIdAsync(id);
    }

    public async Task<IEnumerable<UserInfoDto>> GetAsync(bool isAdmin)
    {
        return await _userRepository.GetAsync(isAdmin);
    }

    public async Task<UserInfoDto?> GetAsync(int id, bool isAdmin)
    {
        return await _userRepository.GetAsync(id, isAdmin);
    }
}
