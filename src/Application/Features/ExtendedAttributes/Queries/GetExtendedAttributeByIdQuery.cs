using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.ExtendedAttributes.Queries;

[ExcludeFromCodeCoverage]
public sealed class GetExtendedAttributeByIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>
    : IQuery<Result<GetExtendedAttributeByIdResponse<TId, TEntityId>>>
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
    public TId Id { get; set; }
}

internal sealed class GetExtendedAttributeByIdQueryHandler<TId, TEntityId, TEntity, TExtendedAttribute>
    : IQueryHandler<GetExtendedAttributeByIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>,
        Result<GetExtendedAttributeByIdResponse<TId, TEntityId>>>
    where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
    where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
    where TId : IEquatable<TId>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<TId> _unitOfWork;

    public GetExtendedAttributeByIdQueryHandler(IUnitOfWork<TId> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GetExtendedAttributeByIdResponse<TId, TEntityId>>> Handle(
        GetExtendedAttributeByIdQuery<TId, TEntityId, TEntity, TExtendedAttribute> query,
        CancellationToken cancellationToken)
    {
        TExtendedAttribute extendedAttribute =
            await _unitOfWork.Repository<TExtendedAttribute>().GetByIdAsync(query.Id);
        var mappedExtendedAttribute =
            _mapper.Map<GetExtendedAttributeByIdResponse<TId, TEntityId>>(extendedAttribute);
        return await Result<GetExtendedAttributeByIdResponse<TId, TEntityId>>.SuccessAsync(mappedExtendedAttribute);
    }
}
