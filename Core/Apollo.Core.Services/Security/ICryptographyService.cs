namespace Apollo.Core.Services.Security
{
    public interface ICryptographyService
    {
        string Decrypt(string text);
        string Encrypt(string text);
    }
}
