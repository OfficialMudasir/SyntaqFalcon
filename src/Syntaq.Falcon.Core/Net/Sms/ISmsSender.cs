using System.Threading.Tasks;

namespace Syntaq.Falcon.Net.Sms
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}