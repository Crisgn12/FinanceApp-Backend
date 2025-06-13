using Entidades.DTOs;

namespace Backend.Services.Interfaces
{
    public interface IMailService
    {
        Task SendPaymentCompletedNotificationAsync(PagoProgramadoDTO pago);
        Task SendNewPaymentNotificationAsync(PagoProgramadoDTO pago);
        Task SendProximityAlertAsync(PagoProgramadoDTO pago);
    }
}
