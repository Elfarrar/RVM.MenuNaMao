namespace RVM.MenuNaMao.Application.Services;

public interface IQrCodeService
{
    byte[] GenerateQrCodePng(string content);
}
