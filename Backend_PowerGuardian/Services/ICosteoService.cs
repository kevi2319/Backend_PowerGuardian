namespace Backend_PowerGuardian.Services
{
    public interface ICosteoService
    {
        Task<decimal> CalcularCostoProductoAsync(int productoId);
    }

}
