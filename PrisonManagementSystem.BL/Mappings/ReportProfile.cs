using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Prison;
using PrisonManagementSystem.BL.DTOs.RequestFeedback;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Mappings
{

    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            // Mapping for CreatePrisonDto to Prison Entity
            CreateMap<CreateReportDto, Report>().ReverseMap();


            // Mapping for UpdatePrisonDto to Prison Entity
            CreateMap<UpdateReportDto, Report>().ReverseMap();


            // Mapping for GetPrisonDto to Prison Entity
            CreateMap<Report, GetReportDto>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
              .ForMember(dest => dest.ReportType, opt => opt.MapFrom(src => src.ReportType)) 
              .ForMember(dest => dest.Descriptions, opt => opt.MapFrom(src => src.Descriptions)) 
              .ForMember(dest => dest.Incident, opt => opt.MapFrom(src => src.RelatedIncident)).ReverseMap();
        }
    }
}
