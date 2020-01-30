using AutoMapper;

using openld.Models;

public class StructureProfile : Profile {
    public StructureProfile() {
        CreateMap<Structure, Structure>()
            // ignore properties not to be overwritten
            .ForMember(s => s.Id, opt => opt.Ignore())
            .ForMember(s => s.View, opt => opt.Ignore())
            .ForMember(s => s.Geometry, opt => opt.Ignore())
            .ForMember(s => s.Fixtures, opt => opt.Ignore())
            .ForMember(s => s.Rating, opt => opt.Condition((src, dest, srcMember) => srcMember != 0))
            // allow modifying other properties, but only if the value given is not null
            .ForAllOtherMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}