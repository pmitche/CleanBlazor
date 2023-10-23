using BlazorHero.CleanArchitecture.Shared.Enums;

namespace BlazorHero.CleanArchitecture.Contracts;

public class UploadRequest
{
    public string FileName { get; set; }
    public string Extension { get; set; }
    public UploadType UploadType { get; set; }
    public byte[] Data { get; set; }
}
