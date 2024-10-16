using System.Net.Http.Headers;

namespace PRN231_ProjectQA_MVC.Authorizations
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                // Add token into header Authorization
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Call the request again with the next handler in the pipeline
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
