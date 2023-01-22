using System.Threading.Tasks;

namespace Endeksa.Services.Abstract
{
    public interface IIpDetectorService
    {
        public string GetUserIP();
        public string GetLocation(string ip);
    }
}
