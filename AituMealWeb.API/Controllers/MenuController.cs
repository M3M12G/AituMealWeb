using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AituMealWeb.Core.DTO.MenuDTOs;
using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AituMealWeb.API.Controllers
{
    [Authorize]//For every authorized user the just read operation is available
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;

        public MenuController(IMenuRepository menuRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        [HttpGet("{date:DateTime?}/{instock:bool?}")]//it is optional to see instock and outstock meals.
        public async Task<IActionResult> GetAllMenuRecs(DateTime? date = null ,bool instock = true)//By default everyone, who authorized, 
                                                                            //will see just inStock meals for today
        {
            if (date == null)
                date = DateTime.Today;

            var menuRecs = await _menuRepository.GetAllMenuJoin((DateTime)date,instock);
            ICollection<MenuScheduled> menuListToReturn = new LinkedList<MenuScheduled>();

            foreach (DateTime d in menuRecs.Select(mu => mu.MenuForDay).Distinct())
            {
                

                ICollection<MealsByTime> byTime = new LinkedList<MealsByTime>();

                foreach (int dt in menuRecs.Select(mu => mu.DayTime).Distinct())
                {
                    var byDt = menuRecs.Where(mu => mu.DayTime == dt && mu.MenuForDay == d);
                    var mapdMeals = _mapper.Map<ICollection<MealOnMenu>>(byDt);

                    MealsByTime singleTime = new MealsByTime()
                    {
                        DayTime = dt,
                        MealOnMenu = mapdMeals
                    };
                    byTime.Add(singleTime);
                }

                MenuScheduled singleMenuRec = new MenuScheduled()
                {
                    MenuFor = d,
                    MealsByTime = byTime
                };

                menuListToReturn.Add(singleMenuRec);

            }

            return menuRecs != null ? (IActionResult)Ok(menuListToReturn) : NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindMenuRec(Guid id)
        {
            var menuRecord = await _menuRepository.GetMenuById(id);
            var menuRecToReturn = _mapper.Map<MenuDetails>(menuRecord);
            return menuRecord != null ? (IActionResult)Ok(menuRecToReturn) : NotFound("No Meals found by this Menu details!");
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost("new")] //adding meals to menu
        public async Task<IActionResult> CreateMenuRecs(List<CreateMenu> menuList)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState);

            try
            {
                ICollection<Menu> menuListToSave = new LinkedList<Menu>();

                var duplicates = menuList.GroupBy(mu => new {mu.MealId, mu.MenuForDay, mu.DayTime}).Where(mu => mu.Count() > 1).SelectMany(mu => mu);

                if (duplicates.Count() > 0)
                    return BadRequest("You have duplicate menu records"); // you cannot assign one meal to the same time of day, after that to the same date

                foreach(CreateMenu menuRec in menuList)
                {
                    Menu singleMenuRec = new Menu()
                    {
                        Id = Guid.NewGuid(),
                        MenuForDay = menuRec.MenuForDay,
                        DayTime = menuRec.DayTime,
                        MealId = menuRec.MealId,
                        InStock = menuRec.InStock
                    };

                    menuListToSave.Add(singleMenuRec);
                }

                await _menuRepository.CreateMenu(menuListToSave);

                return StatusCode(201);
            }
            catch(Exception ex)
            {
                return BadRequest($"Some problems occured during Menu creation action! {ex.Message}");
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPut("change/{id}")]
        public async Task<IActionResult> ChangeMealRecOnMenu(CreateMenu menuForUpd, Guid id)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState);

            try
            {
                Menu forUpdMenu = new Menu()
                {
                    Id = id,
                    MenuForDay = menuForUpd.MenuForDay,
                    DayTime = menuForUpd.DayTime,
                    MealId = menuForUpd.MealId,
                    InStock = menuForUpd.InStock
                };

                await _menuRepository.UpdateRec(id,forUpdMenu); //id is menu Id, forUpdMenu is dto
                return NoContent();
            }
            catch 
            {
                return BadRequest("Some problems occured during Update action for Menu");            
            }
        }
        
        [Authorize(Roles = Role.AdminOrKassir)]
        [HttpPut("stockstatus/{mode:int}/{id}")]
        public async Task<IActionResult> SetMealOutOfStock(int mode,Guid id)
        {
            var menu = await _menuRepository.GetRecById(id);

            if (menu == null) {
                return BadRequest(NotFound("No matching menu record found!"));
            }

            switch(mode) //kassir can make meal out of stock, if it is finished and if meal would be renewed, then kassir can make it in stock
            {
                case 1:
                    menu.InStock = false;
                    break;
                case 2:
                    menu.InStock = true;
                    break;
                default:
                    return BadRequest("Only two operations avalable to menus");
            }

            try
            {
                Menu stockStatusUpd = new Menu()
                {
                    Id = id,
                    MenuForDay = menu.MenuForDay,
                    DayTime = menu.DayTime,
                    MealId = menu.MealId,
                    InStock = menu.InStock
                };

                await _menuRepository.UpdateRec(id, stockStatusUpd);
                return NoContent();
            }
            catch
            {
                return BadRequest("Some problems with updating the Status of stock!");
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("deletion/{id}")]
        public async Task<IActionResult> RemoveMenu(Guid id)
        {
            try
            {
                await _menuRepository.DeleteRec(id);
                return NoContent();
            }
            catch
            {
                return BadRequest("Some problems occured during deletion of Menu record!");
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("deletion/bydate/{date}")]
        public async Task<IActionResult> RemoveForDate(DateTime date)
        {
            try
            {
                await _menuRepository.DeleteForDate(date);
                return NoContent();
            }
            catch
            {
                return BadRequest($"Some problems occured during deletion of Menu records for date : {date}");
            }
        }

        [Authorize(Roles = Role.Admin)]//it is just for test))
        [HttpGet("isavailable/{mealid}")]
        public async Task<IActionResult> isMealAvailableForPurchase(Guid mealId)
        {
            try
            {
                var isMeal = await _menuRepository.isMealAvailable(mealId);

                if (isMeal)
                    return Ok("Yes");
                else
                    return Ok("No");
            }
            catch
            {
                return BadRequest("May be you forgot define meal ID");
            }
        }

    }
}
