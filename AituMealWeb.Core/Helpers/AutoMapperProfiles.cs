using AituMealWeb.Core.DTO;
using AituMealWeb.Core.DTO.MenuDTOs;
using AituMealWeb.Core.DTO.OrderDetailDTOs;
using AituMealWeb.Core.DTO.UserDTOs;
using AituMealWeb.Core.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AituMealWeb.Core.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Meal, MealDetails>();
            CreateMap<User, UserDetails>();
            CreateMap<Menu, MenuDetails>();
            CreateMap<Menu, MealOnMenu>();
        }
    }
}
