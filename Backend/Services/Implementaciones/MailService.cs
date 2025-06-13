using Entidades.DTOs;
using MailKit;
using MimeKit;
using System.Net.Mail;

namespace Backend.Services.Implementaciones
{
    public class MailService : IMailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;

        public int Timeout { get; set; } = 100000; // Default timeout in milliseconds
        public MailKit.Security.SslKeyExchangeAlgorithm SslKeyExchangeAlgorithm { get; set; } = MailKit.Security.SslKeyExchangeAlgorithm.None;

        public MailService(IConfiguration configuration)
        {
            _smtpServer = configuration["Smtp:Server"];
            _smtpPort = int.Parse(configuration["Smtp:Port"]);
            _smtpUser = configuration["Smtp:User"];
            _smtpPass = configuration["Smtp:Password"];
            _fromEmail = configuration["Smtp:FromEmail"];
        }

        public async Task SendProximityAlertAsync(PagoProgramadoDTO pago)
        {
            if (pago.ProximoVencimiento.HasValue &&
                pago.ProximoVencimiento.Value.ToDateTime(new TimeOnly()) - DateTime.Now <= TimeSpan.FromDays(1))
            {
                var template = File.ReadAllText("ProximityAlertEmail.html");
                template = template.Replace("{titulo}", pago.Titulo)
                                  .Replace("{monto}", pago.Monto.ToString("C"))
                                  .Replace("{fechaVencimiento}", pago.ProximoVencimiento.Value.ToString("dd/MM/yyyy"))
                                  .Replace("{descripcion}", pago.Descripcion ?? "Sin descripción");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Sistema de Pagos", _fromEmail));
                message.To.Add(new MailboxAddress(pago.Titulo, pago.UsuarioId)); // Assuming UsuarioId is email
                message.Subject = $"Alerta: Pago {pago.Titulo} próximo a vencer";
                message.Body = new TextPart("html") { Text = template };

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message, CancellationToken.None); // Added CancellationToken
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendNewPaymentNotificationAsync(PagoProgramadoDTO pago)
        {
            if (pago.CreatedAt.Date == DateTime.Now.Date && pago.EsProgramado)
            {
                var template = File.ReadAllText("NewPaymentEmail.html");
                template = template.Replace("{titulo}", pago.Titulo)
                                  .Replace("{monto}", pago.Monto.ToString("C"))
                                  .Replace("{fechaVencimiento}", pago.ProximoVencimiento.Value.ToString("dd/MM/yyyy"))
                                  .Replace("{descripcion}", pago.Descripcion ?? "Sin descripción");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Sistema de Pagos", _fromEmail));
                message.To.Add(new MailboxAddress(pago.Titulo, pago.UsuarioId)); // Assuming UsuarioId is email
                message.Subject = $"Nuevo pago programado: {pago.Titulo}";
                message.Body = new TextPart("html") { Text = template };

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message, CancellationToken.None); // Added CancellationToken
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendPaymentCompletedNotificationAsync(PagoProgramadoDTO pago)
        {
            if (pago.Estado == "Pagado")
            {
                var template = File.ReadAllText("PaymentCompletedEmail.html");
                template = template.Replace("{titulo}", pago.Titulo)
                                  .Replace("{monto}", pago.Monto.ToString("C"))
                                  .Replace("{fechaPago}", DateTime.Now.ToString("dd/MM/yyyy"))
                                  .Replace("{descripcion}", pago.Descripcion ?? "Sin descripción");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Sistema de Pagos", _fromEmail));
                message.To.Add(new MailboxAddress(pago.Titulo, pago.UsuarioId)); // Assuming UsuarioId is email
                message.Subject = $"Pago completado: {pago.Titulo}";
                message.Body = new TextPart("html") { Text = template };

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message, CancellationToken.None); // Added CancellationToken
                await client.DisconnectAsync(true);
            }
        }
    }
}
