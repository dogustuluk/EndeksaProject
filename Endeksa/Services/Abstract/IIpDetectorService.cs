using System.Threading.Tasks;

namespace Endeksa.Services.Abstract
{
    public interface IIpDetectorService
    {
        string GetUserIP();
        string GetLocation(string ip);
    }
}
