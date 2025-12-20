using AutoMapper;
using PKT.Domain.Entities;
using PKT.Application.DTOs.Auth;
using PKT.Application.DTOs.Companies;
using PKT.Application.DTOs.Contacts;
using PKT.Application.DTOs.Leads;
using PKT.Application.DTOs.Opportunities;
using PKT.Application.DTOs.Notes;

namespace PKT.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();

        // Company mappings
        CreateMap<Company, CompanyDto>().ReverseMap();
        CreateMap<CreateCompanyDto, Company>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UpdateCompanyDto, Company>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Contact mappings
        CreateMap<Contact, ContactDto>().ReverseMap();
        CreateMap<CreateContactDto, Contact>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UpdateContactDto, Contact>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Lead mappings
        CreateMap<Lead, LeadDto>().ReverseMap();
        CreateMap<CreateLeadDto, Lead>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UpdateLeadDto, Lead>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Opportunity mappings
        CreateMap<Opportunity, OpportunityDto>().ReverseMap();
        CreateMap<CreateOpportunityDto, Opportunity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UpdateOpportunityDto, Opportunity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // Note mappings
        CreateMap<Note, NoteDto>().ReverseMap();
        CreateMap<CreateNoteDto, Note>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<UpdateNoteDto, Note>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
