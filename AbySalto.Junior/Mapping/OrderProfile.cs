using AutoMapper;
using AbySalto.Junior.Dtos;
using AbySalto.Junior.Models;

namespace AbySalto.Junior.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // Entity -> Read DTO
        CreateMap<OrderItem, OrderReadItemDto>()
            .ForCtorParam("LineTotal", opt => opt.MapFrom(s => s.UnitPrice * s.Quantity));

        CreateMap<Order, OrderReadDto>()
            .ForCtorParam("Total", opt => opt.MapFrom(s => s.Items.Sum(i => i.UnitPrice * i.Quantity)))
            .ForCtorParam("Items", opt => opt.MapFrom(s => s.Items));
      

        // Create DTO -> Entity
        CreateMap<OrderItemCreateDto, OrderItem>();

        CreateMap<OrderCreateDto, Order>()
            .ForMember(d => d.Status, opt => opt.MapFrom(_ => OrderStatus.Pending))
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
