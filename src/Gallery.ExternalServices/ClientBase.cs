using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Gallery.ExternalServices
{
    public class ClientBase
    {
        private readonly string _url;
        private readonly int _timeoutSeconds;

        protected ClientBase(string url, int timeoutSeconds)
        {
            _url = url;
            _timeoutSeconds = timeoutSeconds;
        }

        protected async Task<T> Get<T>(string url)
        {
            var restClient = new RestClient(_url)
                         {
                             Timeout = _timeoutSeconds * 1000
                         };

            var request = new RestRequest(url);

            var cancellationTokenSource = new CancellationTokenSource();

            var restResponse = await restClient
                .ExecuteTaskAsync<T>(request, cancellationTokenSource.Token);

            if (!restResponse.IsSuccessful)
            {
                if (restResponse.StatusCode == 0 && restResponse.ErrorException != null)
                {
                    throw new ExternalServiceHttpException(0, restResponse.ErrorException.Message);
                }

                throw new ExternalServiceHttpException((int)restResponse.StatusCode, restResponse.StatusDescription);
            }

            return restResponse.Data;
        }
    }

}
