using UserProject.Core.DTOs;
using UserProject.DataAccess.Entities;

namespace UserProfileServiceProject.Mappings
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            
            CreateMap<Profile, ProfileDto>()
                .ForMember(dest => dest.Bmi, opt => opt.MapFrom(src =>
                    src.HeightCm.HasValue && src.WeightKg.HasValue && src.HeightCm.Value > 0
                        ? Math.Round(src.WeightKg.Value / (src.HeightCm.Value / 100m * src.HeightCm.Value / 100m), 1)
                        : (decimal?)null))
                .ForMember(dest => dest.Allergies,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Allergies)
                        ? new List<string>()
                        : src.Allergies.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()));
           
            CreateMap<ProfileDto, Profile>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Allergies, opt => opt.MapFrom(src =>
                    src.Allergies != null && src.Allergies.Any()
                        ? string.Join(",", src.Allergies)
                        : string.Empty));

            CreateMap<CreateProfileDto, Profile>()
              .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
              .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
              .ForMember(dest => dest.Allergies,
                     opt => opt.MapFrom(src => src.Allergies != null ? string.Join(",", src.Allergies) : null));

          
            CreateMap<Profile, UserProfileSummary>()
                .ForMember(dest => dest.Weightkg,
                    opt => opt.MapFrom(src => src.WeightKg.HasValue
                        ? (double)src.WeightKg.Value
                        : 0))

                .ForMember(dest => dest.HeightCm,
                    opt => opt.MapFrom(src => src.HeightCm.HasValue
                        ? (double)src.HeightCm.Value
                        : 0))

                .ForMember(dest => dest.Allergies,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrWhiteSpace(src.Allergies)
                            ? new List<string>()
                            : src.Allergies
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(a => a.Trim())
                                .ToList()
                    ));
            CreateMap<User, UserDto>()
          .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
          .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
          .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
          .ForMember(dest => dest.ProfileImageUrl , opt => opt.MapFrom(src => src.Profile.ProfileUrl));

            CreateMap<UpdateProfileDto, Profile>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.ProfileUrl, opt => opt.Ignore()) 
            .ForMember(dest => dest.Allergies, opt => opt.MapFrom(src =>
                !string.IsNullOrWhiteSpace(src.Allergies)
                    ? src.Allergies
                    : string.Empty));
            
        }
    }
}