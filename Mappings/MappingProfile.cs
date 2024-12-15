using AutoMapper;
using RestaurantApi.Models;
using RestaurantApi.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<UserDto, User>();

        CreateMap<UserCreateDto, User>();

        CreateMap<UserUpdateDto, User>();
        CreateMap<User, UserUpdateDto>();

        CreateMap<Restaurant, RestaurantDto>();

        CreateMap<RestaurantDto, Restaurant>();

        CreateMap<RestaurantCreateDto, Restaurant>();

        CreateMap<RestaurantUpdateDto, Restaurant>();
        CreateMap<Restaurant, RestaurantUpdateDto>();

        CreateMap<Dish, DishDto>();

        CreateMap<DishDto, Dish>();

        CreateMap<DishCreateDto, Dish>();

        CreateMap<DishUpdateDto, Dish>();
        CreateMap<Dish, DishUpdateDto>();

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDishes));

        CreateMap<OrderDto, Order>()
            .ForMember(dest => dest.OrderDishes, opt => opt.MapFrom(src => src.OrderDetails));

        CreateMap<OrderCreateDto, Order>();

        CreateMap<OrderUpdateDto, Order>();
        CreateMap<Order, OrderUpdateDto>();

        CreateMap<OrderDish, OrderDetailDto>();

        CreateMap<OrderDetailDto, OrderDish>();
        CreateMap<Role, RoleDto>();

        CreateMap<RoleDto, Role>();
    }
}
