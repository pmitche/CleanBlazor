using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Commands.Delete;
using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Queries.Export;
using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Queries.GetById;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers.Utilities.Misc;

[Route("api/[controller]")]
[ApiController]
public class DocumentTypesController : BaseApiController<DocumentTypesController>
{
    /// <summary>
    ///     Get All Document Types
    /// </summary>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.DocumentTypes.View)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Result<List<GetAllDocumentTypesResponse>> documentTypes = await Mediator.Send(new GetAllDocumentTypesQuery());
        return Ok(documentTypes);
    }

    /// <summary>
    ///     Get Document Type By Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Status 200 Ok</returns>
    [Authorize(Policy = Permissions.DocumentTypes.View)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        Result<GetDocumentTypeByIdResponse> documentType = await Mediator.Send(new GetDocumentTypeByIdQuery(id));
        return Ok(documentType);
    }

    /// <summary>
    ///     Create/Update a Document Type
    /// </summary>
    /// <param name="command"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.DocumentTypes.Create)]
    [HttpPost]
    public async Task<IActionResult> Post(AddEditDocumentTypeCommand command) => Ok(await Mediator.Send(command));

    /// <summary>
    ///     Delete a Document Type
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.DocumentTypes.Delete)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        Ok(await Mediator.Send(new DeleteDocumentTypeCommand(id)));

    /// <summary>
    ///     Search Document Types and Export to Excel
    /// </summary>
    /// <param name="searchString"></param>
    /// <returns></returns>
    [Authorize(Policy = Permissions.DocumentTypes.Export)]
    [HttpGet("export")]
    public async Task<IActionResult> Export(string searchString = "") =>
        Ok(await Mediator.Send(new ExportDocumentTypesQuery(searchString)));
}
