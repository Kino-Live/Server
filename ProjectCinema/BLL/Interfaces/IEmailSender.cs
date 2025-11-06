using System.Threading;
using System.Threading.Tasks;

namespace ProjectCinema.BLL.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
    }
}


