using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AP.Entities.Enums;
using AP.Entities.Options;
using AP.Repositories.Category;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Eager = AP.Entities.Models.Eager;
using Models = AP.Entities.Models;

namespace AP.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        readonly IMapper _mapper;
        readonly ICategoryRepository _categoryRepository;

        public CategoriesController(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        #region GET

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Eager.Category>))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Get([FromQuery(Name = "conditions")] Conditions<Models.Category> conditions)
        {
            var categories = await _categoryRepository.GetCategoriesWithConditions(conditions);

            if(categories == null || !categories.Any())
                return NoContent();

            var mappedCategories = _mapper.Map<IEnumerable<Models.Category>, IEnumerable<Eager.Category>>(categories);

            return Ok(mappedCategories);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(Eager.Category))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get(Guid id)
        {
            if(id.Equals(Guid.Empty))
                return BadRequest(CommonResponseMessages.NoId);

            var category = await _categoryRepository.Retrieve(id);

            if(category == null)
                return NoContent();

            var categoryMapped = _mapper.Map<Models.Category, Eager.Category>(category);

            return Ok(categoryMapped);
        }

        #endregion GET

        #region POST

        [HttpPost]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Post([FromBody] Eager.Category category)
        {
            if(string.IsNullOrWhiteSpace(category.Name))
                return BadRequest(CommonResponseMessages.EmptyName);

            var categoryMapped = _mapper.Map<Eager.Category, Models.Category>(category);

            categoryMapped.CreatedOn = DateTime.Now;
            categoryMapped.ModifiedOn = null;

            var create = await _categoryRepository.Create(categoryMapped);
            return Created($"{this.Request.Scheme}://{this.Request.Host}/api/Categories/{categoryMapped.Id}", null);
        }

        #endregion POST

        #region PUT

        [HttpPut]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Put([FromBody] IEnumerable<Eager.Category> categories)
        {
            var mappedCategories = _mapper.Map<IEnumerable<Models.Category>>(categories);
            var result = await _categoryRepository.MassCategoriesUpdate(mappedCategories);
            if(result)
                return Ok();
            else 
                return BadRequest();
        }

        #endregion PUT

        #region DELETE

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if(id.Equals(Guid.Empty))
                return BadRequest(CommonResponseMessages.NoId);

            if(!_categoryRepository.Exists(id))
                return NoContent();

            await _categoryRepository.Delete(id);
            return Ok();
        }

        #endregion DELETE
    }
}