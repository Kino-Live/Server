using AutoMapper;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Interfaces;

namespace ProjectCinema.BLL.Services
{
    public class UserService : GenericService<UserDTO, User>, IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository, IMapper mapper) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDTO> CreateUserAsync(UserCreateDTO userCreateDTO)
        {
            User user = _mapper.Map<User>(userCreateDTO);
            user.UserRole = UserRole.User;
            user.CreatedAt = DateTime.Now;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            return _mapper.Map<UserDTO>(user);

        }

        public async Task<UserProfileDTO> GetUserProfileDTOByIdAsync(int userId)
        {

            if(await _userRepository.GetByIdAsync(userId) == null)
            {
                throw new KeyNotFoundException($"User with id equal {userId} does not exists");
            }

            User user = await _userRepository.GetByIdAsync(userId);

            return _mapper.Map<UserProfileDTO>(user);   

        }

        public async Task<UserDTO> UpdateUserAsync(UserUpdateDTO userUpdateDTO, int userId)
        {

            User user = await _userRepository.GetByIdAsync(userId);
            _mapper.Map(userUpdateDTO, user);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            return _mapper.Map<UserDTO>(user);

        }
    }
}
