using RVM.MenuNaMao.Application.Services;
using QRCoder;

namespace RVM.MenuNaMao.Infrastructure.Services;

public sealed class QrCodeService : IQrCodeService
{
    public byte[] GenerateQrCodePng(string content)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
        using var code = new PngByteQRCode(data);
        return code.GetGraphic(10);
    }
}
