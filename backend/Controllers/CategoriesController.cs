using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monefy.Api.DTOs.Categories;
using Monefy.Api.DTOs.Common;
using Monefy.Api.Models.Enums;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryDto>>>> GetAll(
        [FromQuery] TransactionType? type, CancellationToken ct)
    {
        var categories = await service.GetAllAsync(type, ct);
        return Ok(ApiResponse<IReadOnlyList<CategoryDto>>.Ok(categories));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(Guid id, CancellationToken ct)
    {
        var category = await service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(category));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(
        [FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var category = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = category.Id },
            ApiResponse<CategoryDto>.Ok(category, "Category created."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(
        Guid id, [FromBody] UpdateCategoryDto dto, CancellationToken ct)
    {
        var category = await service.UpdateAsync(id, dto, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(category, "Category updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Category deleted."));
    }
}
