using System.Collections.Generic;
using ExampleSpaMetadata.Infraestructure;
using ExampleSpaMetadata.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpaMetadata;

namespace ExampleSpaMetadata
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IExampleDataService, ExampleDataService>()
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IExampleDataService exampleDataService)
        {
            if (!env.IsDevelopment())
            {
                app.UseRewriter(new RewriteOptions().AddRedirectToHttps(301));
            }

            app.UseCors("CorsPolicy");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                spa.ApplicationBuilder.UseMetaDataMiddleware(new List<MetadataMiddlewareOptions>
                {
                    new MetadataMiddlewareOptions
                    {
                        FilterPath = "/",
                        GetTokens = (route) => {
                            return new List<MetadataToken>{
                                new MetadataToken {
                                    Key = AppKeys.TwitterCardTokenKey,
                                    Value = AppKeys.TwitterCardTokenValue
                                },
                                new MetadataToken {
                                    Key = AppKeys.MetadataTitleKey,
                                    Value = AppKeys.MetadataTitleValue
                                },
                                new MetadataToken {
                                    Key = AppKeys.MetadataImageKey,
                                    Value = AppKeys.MetadataImageValue
                                },
                                new MetadataToken {
                                    Key = AppKeys.MetadataDescriptionKey,
                                    Value = AppKeys.MetadataDescriptionValue
                                }
                            };
                        }
                    },
                    new MetadataMiddlewareOptions
                    {
                        FilterPath = "page1",
                        GetTokens = (route) => {
                            var results = exampleDataService.Get(route).Result;

                            return new List<MetadataToken>{
                                new MetadataToken {
                                    Key = AppKeys.TwitterCardTokenKey,
                                    Value = AppKeys.TwitterCardTokenValue
                                },
                                new MetadataToken {
                                    Key = AppKeys.MetadataTitleKey,
                                    Value = results.Title
                                },
                                new MetadataToken {
                                    Key = AppKeys.MetadataImageKey,
                                    Value = results.Image
                                },
                                new MetadataToken {
                                    Key = AppKeys.MetadataDescriptionKey,
                                    Value = results.Description
                                }
                            };
                        }
                    },
                    new MetadataMiddlewareOptions
                    {
                        FilterPath = "page2",
                        GetTokens = (route) => {

                            return new List<MetadataToken>{
                                new MetadataToken {
                                    Key = AppKeys.TwitterCardTokenKey,
                                    Value = AppKeys.TwitterCardTokenValue
                                },
                                new MetadataToken {
                                    Key = AppKeys.MetadataTitleKey,
                                    Value = "Custom title"
                                },
                                new MetadataToken {
                                    Key = AppKeys.MetadataImageKey,
                                    Value = AppKeys.MetadataImageValue
                                },
                                new MetadataToken {
                                    Key = AppKeys.MetadataDescriptionKey,
                                    Value = "Custom description"
                                }
                            };
                        }
                    }
                });
            });
        }
    }
}
