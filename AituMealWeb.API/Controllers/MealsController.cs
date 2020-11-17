using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AituMealWeb.Core.DTO;
using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiSandBox.Controllers
{
    [Authorize(Roles = Role.Admin)] // only admin can use CRUD for MealCategory
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        private readonly IMealRepository _mealRepository;
        private readonly IMapper _mapper;

        public MealsController(IMealRepository mealRepository, IMapper mapper)
        {
            _mealRepository = mealRepository;
            _mapper = mapper;
        }

        [HttpGet] //shows detailed info about meals with receiving other data from mealcategories table
        public async Task<IActionResult> FindAllMealsAsync()
        {
            var mealsFromDb = await _mealRepository.GetAllRecsJoin();
            var mealsToReturn = _mapper.Map<IReadOnlyList<MealDetails>>(mealsFromDb);
            return mealsFromDb != null ? (IActionResult)Ok(mealsToReturn) : NoContent();
        }

        [HttpGet("search/{id}")] //shows meal details filtered by category id and same as FindAllMeals()
        public async Task<IActionResult> FindAllMealWithCategoryAsync(Guid id)
        {
            var mealsInfoDB = await _mealRepository.GetAllRecsJoinByCategoryId(id);
            var mealsInfoToReturn = _mapper.Map<IReadOnlyList<MealDetails>>(mealsInfoDB);
            return mealsInfoDB != null ? (IActionResult)Ok(mealsInfoToReturn) : NotFound("No meals exists for the defined category!");
        }

        [HttpGet("{id}")]//shows meal details with receiving other data from mealcategories table
        public async Task<IActionResult> FindMeal(Guid id)
        {
            var meal = await _mealRepository.GetMealById(id);
            var mealToReturn = _mapper.Map<MealDetails>(meal);
            return meal != null ? (IActionResult)Ok(mealToReturn) : NotFound($"No Meals found with Id {id}");

        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateNewMeal(MealCreate newMeal)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState);

            try
            {
                Meal mObj = new Meal()
                {
                    Id = Guid.NewGuid(),
                    Name = newMeal.Name,
                    Picture = newMeal.Picture,
                    Amount = newMeal.Amount,
                    Price = newMeal.Price,
                    MealCategoryId = newMeal.MealCategoryId,
                };
                await _mealRepository.CreateRec(mObj);

                return StatusCode(201);
            }
            catch
            {
                return BadRequest("Some problems occured during Meals creation action!");
            }
        }

        [HttpPut("change/{id}")]
        public async Task<IActionResult> ChangeMeal(Guid id,MealCreate updMeal)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState);

            try
            {
                Meal forUpdateMeal = new Meal()
                {
                    Id = id,
                    Name = updMeal.Name,
                    Picture = updMeal.Picture,
                    Amount = updMeal.Amount,
                    Price = updMeal.Price,
                    MealCategoryId = updMeal.MealCategoryId,
                };
                await _mealRepository.UpdateRec(forUpdateMeal.Id, forUpdateMeal);
                return NoContent();
            }
            catch
            {
                return BadRequest("Some problems occcured during update action!");
            }
        }

        [HttpDelete("deletion/{id}")]
        public async Task<IActionResult> RemoveMeal(Guid id)
        {
            try
            {
                await _mealRepository.DeleteRec(id);
                return NoContent();
            }
            catch
            {
                return BadRequest("Some problems occured during deletion process!");
            }
        }
    }
}
