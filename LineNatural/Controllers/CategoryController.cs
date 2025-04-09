using AutoMapper;
using LineNatural.DTOs.Category;
using LineNatural.Entities;
using LineNatural.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LineNatural.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        //Get

        [HttpGet(Name = "GetCategories")]
        public async Task<IActionResult> GetCategories([FromQuery]string? search)
        {
            List<CategoryDto> categoryList;

            if (!string.IsNullOrEmpty(search))
            {
                categoryList = _mapper.Map<List<CategoryDto>>(await _categoryRepository.GetAllAsync(u => u.CategoryName.Contains(search)));
                return Ok(categoryList);
            }

            categoryList = _mapper.Map<List<CategoryDto>>(await _categoryRepository.GetAllAsync());
            return Ok(categoryList);
        }


        [HttpGet("{id:int}", Name = "GetCategory")]
        public async Task<IActionResult> GetCategory(int id, [FromQuery]string? search)
        {
            try
            {
                Category category;
                if (id <= 0 )
                {
                    throw new Exception();
                }

                if (!string.IsNullOrEmpty(search))
                {
                    category = await _categoryRepository.GetAsync(u => u.Id ==id && u.CategoryName.Contains(search));
                    
                    if (category == null)
                    {
                        return NotFound();
                    }

                    return Ok(_mapper.Map<CategoryDto>(category));
                }

                category = await _categoryRepository.GetAsync(u => u.Id == id);

                if (category == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<CategoryDto>(category));
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        //Post
        [HttpPost("PostCategory")]
        public async Task<IActionResult> PostCategory([FromBody]CategoryCreateDto categoryCreateDto)
        {
            Category categoryAdd;

            //Esta funcion no sera vista debido a que VS envia directamente un badrequest cuando no se cumple con
            //la validacion de los parametros de CategoryCreateDto
            //if (categoryCreateDto == null) 
            //{
            //    return BadRequest();
            //}

            var categoryExist = await _categoryRepository.GetAsync(u => u.CategoryName.ToLower() == categoryCreateDto.CategoryName.ToLower());
            if (categoryExist != null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            categoryAdd = _mapper.Map<Category>(categoryCreateDto);
            await _categoryRepository.CreateAsync(categoryAdd);

            return CreatedAtRoute("GetCategory", new { id = categoryAdd.Id }, _mapper.Map<CategoryDto>(categoryAdd));
        }

        //Put
        [HttpPut("{id:int}", Name = "PutCategory")]
        public async Task<IActionResult> PutCategory(int id, [FromBody]CategoryUpdateDto categoryUpdateDto)
        {
            try
            {
                Category categoryUpdate;
                if (id <= 0 )
                {
                    throw new Exception();
                }

                if (id != categoryUpdateDto.Id)
                {
                    return BadRequest();
                }

                var categoryExist = await _categoryRepository.GetAsync(u => u.Id == id, traked: false);
                if (categoryExist == null)
                {
                    return NotFound();
                }

                categoryUpdate = _mapper.Map<Category>(categoryUpdateDto);
                await _categoryRepository.UpdateAsync(categoryUpdate);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //Delete
        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                Category categoryDelete;
                if(id <= 0)
                {
                    throw new Exception();
                }

                categoryDelete = await _categoryRepository.GetAsync(u => u.Id == id);
                if (categoryDelete == null) 
                {
                    return NotFound();
                }

                await _categoryRepository.RemoveAsync(categoryDelete);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
