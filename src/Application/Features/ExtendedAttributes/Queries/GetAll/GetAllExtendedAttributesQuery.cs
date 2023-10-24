using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;

namespace BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAll;

public class GetAllExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>
    : IQuery<Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>>
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
}

internal class GetAllExtendedAttributesQueryHandler<TId, TEntityId, TEntity, TExtendedAttribute>
    : IQueryHandler<GetAllExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>,
        Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>>
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
    private readonly IAppCache _cache;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<TId> _unitOfWork;

    public GetAllExtendedAttributesQueryHandler(IUnitOfWork<TId> unitOfWork, IMapper mapper, IAppCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>> Handle(
        GetAllExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute> request,
        CancellationToken cancellationToken)
    {
        Task<List<TExtendedAttribute>> GetAllExtendedAttributes() => _unitOfWork.Repository<TExtendedAttribute>().GetAllAsync();
        List<TExtendedAttribute> extendedAttributeList = await _cache.GetOrAddAsync(
            ApplicationConstants.Cache.GetAllEntityExtendedAttributesCacheKey(typeof(TEntity).Name),
            GetAllExtendedAttributes);
        var mappedExtendedAttributes =
            _mapper.Map<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>(extendedAttributeList);
        return await Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>.SuccessAsync(
            mappedExtendedAttributes);
    }
}
