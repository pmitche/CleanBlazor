using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Contracts;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Products.Commands;

[ExcludeFromCodeCoverage]
public sealed record AddEditProductCommand : ICommand<Result<int>>
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Barcode { get; init; }
    public string Description { get; init; }
    public string ImageDataUrl { get; set; }
    public decimal Rate { get; init; }
    public int BrandId { get; init; }
    public UploadRequest UploadRequest { get; init; }
}

internal sealed class AddEditProductCommandHandler : ICommandHandler<AddEditProductCommand, Result<int>>
{
    private readonly IStringLocalizer<AddEditProductCommandHandler> _localizer;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUploadService _uploadService;

    public AddEditProductCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUploadService uploadService,
        IStringLocalizer<AddEditProductCommandHandler> localizer,
        IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _uploadService = uploadService;
        _localizer = localizer;
        _productRepository = productRepository;
    }

    public async Task<Result<int>> Handle(AddEditProductCommand command, CancellationToken cancellationToken)
    {
        if (await _productRepository.Entities.Where(p => p.Id != command.Id)
                .AnyAsync(p => p.Barcode == command.Barcode, cancellationToken))
        {
            return Result.Fail<int>(_localizer["Barcode already exists."]);
        }

        UploadRequest uploadRequest = command.UploadRequest;
        if (uploadRequest != null)
        {
            uploadRequest.FileName = $"P-{command.Barcode}{uploadRequest.Extension}";
        }

        if (command.Id == 0)
        {
            var product = _mapper.Map<Product>(command);
            if (uploadRequest != null)
            {
                product.ImageDataUrl = _uploadService.UploadAsync(uploadRequest);
            }

            _productRepository.Add(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(product.Id, _localizer["Product Saved"]);
        }
        else
        {
            var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
            if (product == null)
            {
                return Result.Fail<int>(_localizer["Product Not Found!"]);
            }

            product.Name = command.Name ?? product.Name;
            product.Description = command.Description ?? product.Description;
            if (uploadRequest != null)
            {
                product.ImageDataUrl = _uploadService.UploadAsync(uploadRequest);
            }

            product.Rate = command.Rate == 0 ? product.Rate : command.Rate;
            product.BrandId = command.BrandId == 0 ? product.BrandId : command.BrandId;
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(product.Id, _localizer["Product Updated"]);
        }
    }
}
