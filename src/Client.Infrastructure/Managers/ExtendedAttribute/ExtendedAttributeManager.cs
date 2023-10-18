using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.Export;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAll;
using BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.ExtendedAttribute;

public class ExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute>
    : IExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute>
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
    private readonly HttpClient _httpClient;

    public ExtendedAttributeManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<string>> ExportToExcelAsync(
        ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute> request)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(
            string.IsNullOrWhiteSpace(request.SearchString) && !request.IncludeEntity && !request.OnlyCurrentGroup
                ? ExtendedAttributesEndpoints.Export(typeof(TEntity).Name,
                    request.EntityId,
                    request.IncludeEntity,
                    request.OnlyCurrentGroup,
                    request.CurrentGroup)
                : ExtendedAttributesEndpoints.ExportFiltered(typeof(TEntity).Name,
                    request.SearchString,
                    request.EntityId,
                    request.IncludeEntity,
                    request.OnlyCurrentGroup,
                    request.CurrentGroup));
        return await response.ToResult<string>();
    }

    public async Task<IResult<TId>> DeleteAsync(TId id)
    {
        HttpResponseMessage response =
            await _httpClient.DeleteAsync($"{ExtendedAttributesEndpoints.Delete(typeof(TEntity).Name)}/{id}");
        return await response.ToResult<TId>();
    }

    public async Task<IResult<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>> GetAllAsync()
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync(ExtendedAttributesEndpoints.GetAll(typeof(TEntity).Name));
        return await response.ToResult<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>();
    }

    public async Task<IResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>> GetAllByEntityIdAsync(
        TEntityId entityId)
    {
        var route = ExtendedAttributesEndpoints.GetAllByEntityId(typeof(TEntity).Name, entityId);
        HttpResponseMessage response = await _httpClient.GetAsync(route);
        return await response.ToResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>();
    }

    public async Task<IResult<TId>> SaveAsync(
        AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> request)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(ExtendedAttributesEndpoints.Save(typeof(TEntity).Name), request);
        return await response.ToResult<TId>();
    }
}
