using BCrypt.Net;
using UserProject.Core.DTOs;
using UserProject.DataAccess.Entities;
using UserProject.DataAccess.Repositories.Interfaces;
using UserProject.Core.Services.Interfaces;
using UserProject.Core.Security;
using Microsoft.AspNetCore.Http.HttpResults;

namespace UserProject.Core.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService( IUserRepository userRepository, IProfileRepository profileRepository, IJwtService jwtService,IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _jwtService = jwtService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant());
            if (existingUser != null)
                throw new InvalidOperationException("this user already exist.");
            Guid userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = dto.Email.Trim().ToLowerInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                FullName = dto.FirstName + " " + dto.LastName,
                Profile = new Profile
                {
                    FullName = dto.FirstName + " " + dto.LastName,
                    Email = dto.Email.Trim().ToLowerInvariant(),
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,

                }

            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var user = await _userRepository.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant());
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Wrong Email or password ! .");
            }

            
            var accessToken = _jwtService.GenerateToken(user);
            var refreshTokenString = _jwtService.GenerateRefreshToken();

           
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            return new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = refreshTokenEntity.ExpiresAt,

                Role = user.Role,
                Email = user.Email,
                FullName = user.Profile?.FullName ?? user.Email,
                UserId = user.Id
            };
        }

        public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is needed.");

            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token has expired.");

          
            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null)
                throw new UnauthorizedAccessException();

            
            storedToken.IsRevoked = true;
            storedToken.ReplacedByToken = _jwtService.GenerateRefreshToken();

           
            var newRefreshTokenEntity = new RefreshToken
            {
                Token = storedToken.ReplacedByToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);
            await _refreshTokenRepository.SaveChangesAsync();

         
            var newAccessToken = _jwtService.GenerateToken(user);

            return new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = storedToken.ReplacedByToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = newRefreshTokenEntity.ExpiresAt,
                ProfileUrl = user.Profile?.ProfileUrl,
                Role = user.Role,
                Email = user.Email,
                FullName = user.Profile?.FullName ?? user.Email,
                UserId = user.Id
            };
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (token != null)
            {
                token.IsRevoked = true;
                await _refreshTokenRepository.SaveChangesAsync();
            }
        }
    }
}