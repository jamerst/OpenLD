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
            // allow modifying other properties, but only if the value given is not null
            .ForAllOtherMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}