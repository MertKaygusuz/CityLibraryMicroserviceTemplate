using Grpc.Core;
using Grpc.Core.Interceptors;

namespace BookServiceApi.Interceptors
{
    public class GrpcHeadersInterceptor(IHttpContextAccessor httpContextAccessor) : Interceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var metadata = new Metadata
            {
                { "Authorization", _httpContextAccessor.HttpContext.Request.Headers.Authorization },
                { "User-Agent", _httpContextAccessor.HttpContext.Request.Headers.UserAgent },
                { "Accept-Language", _httpContextAccessor.HttpContext.Request.Headers.AcceptLanguage }
            };
            
            var callOption = context.Options.WithHeaders(metadata);
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, callOption);

            return base.AsyncUnaryCall(request, context, continuation);
        }
    }
}