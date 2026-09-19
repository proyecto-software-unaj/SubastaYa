using Application.DTOs.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
    }
}
