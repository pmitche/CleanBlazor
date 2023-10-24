using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Contracts;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Products.Commands.AddEdit;

public class AddEditProductCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Barcode { get; set; }
    public string Description { get; set; }
    public string ImageDataUrl { get; set; }
    public decimal Rate { get; set; }
    public int BrandId { get; set; }
    public UploadRequest UploadRequest { get; set; }
}

internal class AddEditProductCommandHandler : ICommandHandler<AddEditProductCommand, Result<int>>
{
    private readonly IStringLocalizer<AddEditProductCommandHandler> _localizer;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IUploadService _uploadService;

    public AddEditProductCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IMapper mapper,
        IUploadService uploadService,
        IStringLocalizer<AddEditProductCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _uploadService = uploadService;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(AddEditProductCommand command, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Repository<Product>().Entities.Where(p => p.Id != command.Id)
                .AnyAsync(p => p.Barcode == command.Barcode, cancellationToken))
        {
            return await Result<int>.FailAsync(_localizer["Barcode already exists."]);
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

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<int>.SuccessAsync(product.Id, _localizer["Product Saved"]);
        }
        else
        {
            Product product = await _unitOfWork.Repository<Product>().GetByIdAsync(command.Id);
            if (product == null)
            {
                return await Result<int>.FailAsync(_localizer["Product Not Found!"]);
            }

            product.Name = command.Name ?? product.Name;
            product.Description = command.Description ?? product.Description;
            if (uploadRequest != null)
            {
                product.ImageDataUrl = _uploadService.UploadAsync(uploadRequest);
            }

            product.Rate = command.Rate == 0 ? product.Rate : command.Rate;
            product.BrandId = command.BrandId == 0 ? product.BrandId : command.BrandId;
            await _unitOfWork.Repository<Product>().UpdateAsync(product);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<int>.SuccessAsync(product.Id, _localizer["Product Updated"]);
        }
    }
}
