using Application.DTOs.Categories;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Api.Controllers
{
    public class CategoriesController : ApiControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService) => _categoryService = categoryService;

        // GET /api/categories
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken ct)
        {
            var result = await _categoryService.GetCategoriesAsync(ct);
            return Ok(result);
        }
    }
}
