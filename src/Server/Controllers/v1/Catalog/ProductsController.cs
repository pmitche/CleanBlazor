using BlazorHero.CleanArchitecture.Application.Features.Products.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.Products.Commands.Delete;
using BlazorHero.CleanArchitecture.Application.Features.Products.Queries.Export;
using BlazorHero.CleanArchitecture.Application.Features.Products.Queries.GetAllPaged;
using BlazorHero.CleanArchitecture.Application.Features.Products.Queries.GetProductImage;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers.v1.Catalog;

public class ProductsController : BaseApiController<ProductsController>
{
    /// <summary>
    ///     Get All Products
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="searchString"></param>
    /// <param name="orderBy"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Products.View)]
    [HttpGet]
    public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, string orderBy = null)
    {
        PaginatedResult<GetAllPagedProductsResponse> products =
            await Mediator.Send(new GetAllProductsQuery(pageNumber, pageSize, searchString, orderBy));
        return Ok(products);
    }

    /// <summary>
    ///     Get a Product Image by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Products.View)]
    [HttpGet("image/{id:int}")]
    public async Task<IActionResult> GetProductImageAsync(int id)
    {
        Result<string> result = await Mediator.Send(new GetProductImageQuery(id));
        return Ok(result);
    }

    /// <summary>
    ///     Add/Edit a Product
    /// </summary>
    /// <param name="command"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Products.Create)]
    [HttpPost]
    public async Task<IActionResult> Post(AddEditProductCommand command) => Ok(await Mediator.Send(command));

    /// <summary>
    ///     Delete a Product
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Status 200 OK response</returns>
    [Authorize(Policy = Permissions.Products.Delete)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => Ok(await Mediator.Send(new DeleteProductCommand { Id = id }));

    /// <summary>
    ///     Search Products and Export to Excel
    /// </summary>
    /// <param name="searchString"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Products.Export)]
    [HttpGet("export")]
    public async Task<IActionResult> Export(string searchString = "") =>
        Ok(await Mediator.Send(new ExportProductsQuery(searchString)));
}
