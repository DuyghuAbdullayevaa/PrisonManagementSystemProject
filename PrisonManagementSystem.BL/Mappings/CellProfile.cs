using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Cell;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;

public class CellProfile : Profile
{
    public CellProfile()
    {
        CreateMap<CreateCellDto, Cell>()
            .ForMember(dest => dest.CurrentOccupancy, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => CellStatus.Available))
            .ReverseMap();

        CreateMap<UpdateCellDto, Cell>()
            .ForMember(dest => dest.Status, opt => opt.PreCondition(src => src.Status == CellStatus.UnderMaintenance))
            .ForMember(dest => dest.CurrentOccupancy, opt => opt.Ignore());

        CreateMap<Cell, GetCellDto>()
            .ForMember(dest => dest.PrisonName, opt => opt.MapFrom(src => src.Prison.Name))
            .ReverseMap();
    }
}
