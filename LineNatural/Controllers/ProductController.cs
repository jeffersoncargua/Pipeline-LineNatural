using AutoMapper;
using LineNatural.DTOs.Product;
using LineNatural.Entities;
using LineNatural.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LineNatural.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productoRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public ProductController(IProductRepository productRepository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _productoRepository = productRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }


        //Get
        [HttpGet(Name = "GetProducts")]
        public async Task<IActionResult> GetProducts([FromQuery]string? search)
        {
            List<ProductoDto> productoList;
            if (!string.IsNullOrEmpty(search))
            {
                productoList = _mapper.Map<List<ProductoDto>>(await _productoRepository.GetAllAsync(u => u.ProductName.Contains(search)));
                return Ok(productoList);
            }

            productoList = _mapper.Map<List<ProductoDto>>(await _productoRepository.GetAllAsync());
            return Ok(productoList);
        }

        [HttpGet("{id:int}",Name = "GetProduct")]
        public async Task<IActionResult> GetProduct(int id, [FromQuery]string? search)
        {
            try
            {
                Producto producto;
                if (id <= 0)
                {
                    throw new Exception();
                }

                if (!string.IsNullOrEmpty(search))
                {
                    producto = await _productoRepository.GetAsync(u => u.Id == id && u.ProductName.Contains(search),includeProperties:"Category");
                    if(producto == null)
                    {
                        return NotFound();
                    }

                    return Ok(_mapper.Map<ProductoDto>(producto));
                }

                producto = await _productoRepository.GetAsync(u => u.Id == id,includeProperties:"Category");
                if (producto == null)
                {
                    return NotFound();
                }

                return Ok(_mapper.Map<ProductoDto>(producto));
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        //Post 
        //[HttpPost("PostProduct")] permite especificar la ruta en la que se debe realizar la peticion en el API
        [HttpPost("PostProduct")]
        public async Task<IActionResult> PostProduct([FromBody]ProductoCreateDto productoCreateDto)
        {
            Producto productoAdd;

            if (await _productoRepository.GetAsync(u => u.ProductName.ToLower() == productoCreateDto.ProductName.ToLower()) != null || await _categoryRepository.GetAsync(u => u.Id == productoCreateDto.CategoryId) == null)
            {
                return BadRequest();
            }

            productoAdd = _mapper.Map<Producto>(productoCreateDto);
            await _productoRepository.CreateAsync(productoAdd);

            return CreatedAtRoute("GetProduct", new { id = productoAdd.Id }, _mapper.Map<ProductoDto>(productoAdd));
        }

        //Put
        [HttpPut("{id:int}",Name = "PutProduct")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductoUpdateDto productoUpdateDto)
        {
            try
            {
                Producto productoUpdate;
                if (id <= 0)
                {
                    throw new Exception();
                }

                if (await _categoryRepository.GetAsync(u => u.Id == productoUpdateDto.CategoryId, traked: false) == null || id != productoUpdateDto.Id)
                {
                    return BadRequest();
                }

                if (await _productoRepository.GetAsync(u => u.Id == id,traked:false) == null)
                {
                    return NotFound();
                }

                productoUpdate = _mapper.Map<Producto>(productoUpdateDto);
                await _productoRepository.UpdateAsync(productoUpdate);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //Delete
        [HttpDelete("{id:int}",Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                Producto productoDelete;
                if (id <= 0)
                {
                    throw new Exception();
                }

                productoDelete = await _productoRepository.GetAsync(u => u.Id == id);
                if (productoDelete == null)
                {
                    return NotFound();
                }

                await _productoRepository.RemoveAsync(productoDelete);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
