using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands.Delete;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands.Import;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Queries.Export;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Queries.GetById;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers.v1.Catalog;

public class BrandsController : BaseApiController<BrandsController>
{
    /// <summary>
    ///     Get All Brands
    /// </summary>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Brands.View)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Result<List<GetAllBrandsResponse>> brands = await Mediator.Send(new GetAllBrandsQuery());
        return Ok(brands);
    }

    /// <summary>
    ///     Get a Brand By Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Status 200 Ok</returns>
    [Authorize(Policy = Permissions.Brands.View)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        Result<GetBrandByIdResponse> brand = await Mediator.Send(new GetBrandByIdQuery { Id = id });
        return Ok(brand);
    }

    /// <summary>
    ///     Create/Update a Brand
    /// </summary>
    /// <param name="command"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Brands.Create)]
    [HttpPost]
    public async Task<IActionResult> Post(AddEditBrandCommand command) => Ok(await Mediator.Send(command));

    /// <summary>
    ///     Delete a Brand
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Brands.Delete)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => Ok(await Mediator.Send(new DeleteBrandCommand { Id = id }));

    /// <summary>
    ///     Search Brands and Export to Excel
    /// </summary>
    /// <param name="searchString"></param>
    /// <returns></returns>
    [Authorize(Policy = Permissions.Brands.Export)]
    [HttpGet("export")]
    public async Task<IActionResult> Export(string searchString = "") =>
        Ok(await Mediator.Send(new ExportBrandsQuery(searchString)));

    /// <summary>
    ///     Import Brands from Excel
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [Authorize(Policy = Permissions.Brands.Import)]
    [HttpPost("import")]
    public async Task<IActionResult> Import(ImportBrandsCommand command) => Ok(await Mediator.Send(command));
}
