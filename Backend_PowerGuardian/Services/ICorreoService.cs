namespace Backend_PowerGuardian.Services
{
    public interface ICorreoService
    {
        Task EnviarCorreoRegistroConSku(string email, string sku);
        Task EnviarCorreoAsociarDispositivo(string email, string sku);
    }
}
