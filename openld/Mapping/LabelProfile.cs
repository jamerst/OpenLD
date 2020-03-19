using AutoMapper;

using openld.Models;

public class LabelProfile : Profile {
    public LabelProfile() {
        CreateMap<Label, Label>()
            // ignore properties not to be overwritten
            .ForMember(l => l.Id, opt => opt.Ignore())
            .ForMember(l => l.View, opt => opt.Ignore())
            .ForMember(l => l.Position, opt => opt.Ignore())
            .ForAllOtherMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}