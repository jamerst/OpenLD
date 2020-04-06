using AutoMapper;

using openld.Models;

namespace openld.Mapping {
    public class RiggedFixtureProfile : Profile {
        public RiggedFixtureProfile() {
            CreateMap<RiggedFixture, RiggedFixture>()
                // ignore properties not to be overwritten
                .ForMember(f => f.Id, opt => opt.Ignore())
                .ForMember(f => f.Fixture, opt => opt.Ignore())
                .ForMember(f => f.Structure, opt => opt.Ignore())
                .ForMember(f => f.Position, opt => opt.Ignore())
                // allow modifying numerical values, but only if >= 0 (-1 used to signal not provided as they are initialized to 0 by default, but 0 may be a valid value)
                .ForMember(f => f.Angle, opt => opt.Condition((src, dest, srcMember) => srcMember >= 0))
                .ForMember(f => f.Channel, opt => opt.Condition((src, dest, srcMember) => srcMember >= 0))
                .ForMember(f => f.Address, opt => opt.Condition((src, dest, srcMember) => srcMember >= 0))
                .ForMember(f => f.Universe, opt => opt.Condition((src, dest, srcMember) => srcMember >= 0))
                // allow modifying other properties, but only if the value given is not null
                .ForAllOtherMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}