using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SpaMetadata
{
    public class MetaDataMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<MetadataMiddlewareOptions> _options;


        public MetaDataMiddleware(RequestDelegate next, List<MetadataMiddlewareOptions> options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;

            var validOption = _options.Where(option => path.Contains(option.FilterPath)).LastOrDefault();

            if (validOption == null)
            {
                return;
            }

            var newContent = string.Empty;

            var existingBody = httpContext.Response.Body;

            using (var newBody = new MemoryStream())
            {
                httpContext.Response.Body = newBody;

                await _next(httpContext);

                if (httpContext.Response.StatusCode >= StatusCodes.Status300MultipleChoices || !httpContext.Response.ContentType.Contains("text/html"))
                {
                    return;
                }

                httpContext.Response.Body = existingBody;

                newBody.Seek(0, SeekOrigin.Begin);

                newContent = new StreamReader(newBody).ReadToEnd();

                var tokens = validOption.GetTokens(path);

                if (tokens != null)
                {
                    foreach (MetadataToken token in tokens)
                    {
                        newContent = newContent.Replace(token.Key, token.Value);
                    }
                }

                httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(newContent);

                await httpContext.Response.WriteAsync(newContent);
            }
        }
    }

    public static class MetaDataMiddlewareExtension
    {
        public static IApplicationBuilder UseMetaDataMiddleware(this IApplicationBuilder builder, List<MetadataMiddlewareOptions> options)
        {
            return builder.UseMiddleware<MetaDataMiddleware>(options);
        }
    }
}
