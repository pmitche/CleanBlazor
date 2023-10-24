using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands;
using BlazorHero.CleanArchitecture.Domain.Entities.ExtendedAttributes;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Server.Controllers.Utilities.ExtendedAttributes.Base;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers.Utilities.ExtendedAttributes.Misc;

public class
    DocumentExtendedAttributesController : ExtendedAttributesController<int, int, Document, DocumentExtendedAttribute>
{
    [Authorize(Policy = Permissions.DocumentExtendedAttributes.View)]
    public override Task<IActionResult> GetAll() => base.GetAll();

    [Authorize(Policy = Permissions.DocumentExtendedAttributes.View)]
    public override Task<IActionResult> GetAllByEntityId(int entityId) => base.GetAllByEntityId(entityId);

    [Authorize(Policy = Permissions.DocumentExtendedAttributes.View)]
    public override Task<IActionResult> GetById(int id) => base.GetById(id);

    [Authorize(Policy = Permissions.DocumentExtendedAttributes.Create)]
    public override Task<IActionResult> Post(
        AddEditExtendedAttributeCommand<int, int, Document, DocumentExtendedAttribute> command) => base.Post(command);

    [Authorize(Policy = Permissions.DocumentExtendedAttributes.Delete)]
    public override Task<IActionResult> Delete(int id) => base.Delete(id);

    [Authorize(Policy = Permissions.DocumentExtendedAttributes.Export)]
    public override Task<IActionResult> Export(
        string searchString = "",
        int entityId = default,
        bool includeEntity = false,
        bool onlyCurrentGroup = false,
        string currentGroup = "") => base.Export(searchString, entityId, includeEntity, onlyCurrentGroup, currentGroup);
}
