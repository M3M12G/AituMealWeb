using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AituMealWeb.Core.DTO;
using AituMealWeb.Core.Entities;
using AituMealWeb.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apiSandBox.Controllers
{
    [Authorize(Roles = Role.Admin)] // only admin can use CRUD for MealCategory
    [Route("api/[controller]")]
    [ApiController]
    public class MealCategoriesController : ControllerBase
    {
        private readonly IMealCategoryRepository _mealCategoryRepository;

        public MealCategoriesController(IMealCategoryRepository mealCategoryRepository)
        {
            _mealCategoryRepository = mealCategoryRepository;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FindAllMCAsync()
        {
                IEnumerable<MealCategory> mcDetails = await _mealCategoryRepository.GetAllRecs();
                ICollection<MealCategoryDetails> mealCategories = new LinkedList<MealCategoryDetails>();

                foreach (MealCategory mc in mcDetails)
                {
                    MealCategoryDetails mCdetailed = new MealCategoryDetails()
                    {
                        Id = mc.Id,
                        Category = mc.Category,
                    };
                    mealCategories.Add(mCdetailed);
                }
                return Ok(mealCategories);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindMC(Guid id)
        {
                var category = await _mealCategoryRepository.GetRecById(id);
            if (category != null) 
            {
                MealCategoryDetails detailedMC = new MealCategoryDetails()
                {
                    Id = category.Id,
                    Category = category.Category
                };
                
                return Ok(detailedMC);
            }

            return BadRequest($"No meal category found for id {id}");
                
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateNewMC(MealCategoryDetails newMc)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState);

            try
            {
                MealCategory newMC = new MealCategory()
                {
                    Id = Guid.NewGuid(),
                    Category = newMc.Category,
                };
                await _mealCategoryRepository.CreateRec(newMC);

                return StatusCode(201);
            }
            catch
            {
                return BadRequest("Some problems occured during Category creation!");
            }
        }

        [HttpPut("change/{id}")]
        public async Task<IActionResult> ChangeMC(Guid id,MealCategoryDetails CategoryForUpd)
        {
            if (!ModelState.IsValid)           //Validation mechanism invoke
                return BadRequest(ModelState);
            try
            {
                MealCategory forUpdateMC = new MealCategory()
                {
                    Id = id,
                    Category = CategoryForUpd.Category,
                };
                await _mealCategoryRepository.UpdateRec(forUpdateMC.Id, forUpdateMC);
                return NoContent();
            }
            catch
            {
                return BadRequest("Some problems occured during Update action!");
            }
        }

        [HttpDelete("deletion/{id}")]
        public async Task<IActionResult> RemoveMC(Guid id)
        {
            try 
            {
                await _mealCategoryRepository.DeleteRec(id);
                return NoContent();
            } catch 
            {
                return BadRequest($"Some problems occured during deletion of category with Id {id}");
            }
        }
    }
}
