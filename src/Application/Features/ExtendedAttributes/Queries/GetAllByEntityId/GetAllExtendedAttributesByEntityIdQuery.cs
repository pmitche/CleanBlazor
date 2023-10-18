using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;

public class GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>
    : IRequest<Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>>
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
    public GetAllExtendedAttributesByEntityIdQuery(TEntityId entityId) => EntityId = entityId;

    public TEntityId EntityId { get; set; }
}

internal class GetAllExtendedAttributesByEntityIdQueryHandler<TId, TEntityId, TEntity, TExtendedAttribute>
    : IRequestHandler<GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>,
        Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>>
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
    private readonly IAppCache _cache;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<TId> _unitOfWork;

    public GetAllExtendedAttributesByEntityIdQueryHandler(IUnitOfWork<TId> unitOfWork, IMapper mapper, IAppCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>> Handle(
        GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute> request,
        CancellationToken cancellationToken)
    {
        Task<List<TExtendedAttribute>> GetAllExtendedAttributesByEntityId() =>
            _unitOfWork.Repository<TExtendedAttribute>()
                .Entities.Where(x => x.EntityId.Equals(request.EntityId))
                .ToListAsync(cancellationToken);

        List<TExtendedAttribute> extendedAttributeList = await _cache.GetOrAddAsync(
            ApplicationConstants.Cache.GetAllEntityExtendedAttributesByEntityIdCacheKey(typeof(TEntity).Name,
                request.EntityId),
            GetAllExtendedAttributesByEntityId);
        var mappedExtendedAttributes =
            _mapper.Map<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>(extendedAttributeList);
        return await Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>.SuccessAsync(
            mappedExtendedAttributes);
    }
}
