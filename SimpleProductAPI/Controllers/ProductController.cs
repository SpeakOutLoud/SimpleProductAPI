using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpleProductAPI.Models;
using SimpleProductAPI.Repositories;
using System.ComponentModel.DataAnnotations;

namespace SimpleProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _dataRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository dataRepository,
                                   IMapper mapper)
        {
            _dataRepository = dataRepository;
            _mapper = mapper;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid page or pageSize. Both must be positive integers.");
            }

            var products = await _dataRepository.GetPagedReponseAsync(page, pageSize);
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _dataRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> PostProduct([Required] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var product = _mapper.Map<Product>(productDto);
            product = await _dataRepository.AddAsync(product);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [Required] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var existingProduct = await _dataRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Price = productDto.Price;
            existingProduct.Quantity = productDto.Quantity;
            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;

            await _dataRepository.UpdateAsync(existingProduct);
            return CreatedAtAction(nameof(GetProduct), new { id = existingProduct.Id }, existingProduct);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _dataRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _dataRepository.DeleteAsync(product);   
            return NoContent();
        }
    }
}
