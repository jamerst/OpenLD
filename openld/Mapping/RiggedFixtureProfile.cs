using AutoMapper;

using openld.Models;

public class RiggedStructureProfile : Profile {
    public RiggedStructureProfile() {
        CreateMap<RiggedFixture, RiggedFixture>()
            // ignore properties not to be overwritten
            .ForMember(f => f.Id, opt => opt.Ignore())
            .ForMember(f => f.Fixture, opt => opt.Ignore())
            .ForMember(f => f.Structure, opt => opt.Ignore())
            .ForMember(f => f.Position, opt => opt.Ignore())
            // allow modifying numerical values, but only if != 0 (when not provided they are initialised to 0, not null)
            .ForMember(f => f.Angle, opt => opt.Condition((src, dest, srcMember) => srcMember != 0))
            .ForMember(f => f.Channel, opt => opt.Condition((src, dest, srcMember) => srcMember != 0))
            .ForMember(f => f.Address, opt => opt.Condition((src, dest, srcMember) => srcMember != 0))
            .ForMember(f => f.Universe, opt => opt.Condition((src, dest, srcMember) => srcMember != 0))
            // allow modifying other properties, but only if the value given is not null
            .ForAllOtherMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}