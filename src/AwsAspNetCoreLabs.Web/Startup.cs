#region Imports
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using AwsAspNetCoreLabs.Models.Settings;
using AwsAspNetCoreLabs.S3;
using AwsAspNetCoreLabs.SQS;
using AwsAspNetCoreLabs.SES;
#endregion

namespace AwsAspNetCoreLabs.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure service settings using appsettings 
            services.Configure<S3Settings>(Configuration.GetSection("Storage"));
            services.Configure<SQSSettings>(Configuration.GetSection("Queueing"));
            services.Configure<SESSettings>(Configuration.GetSection("Email"));

            services.AddSingleton<INoteStorageService, S3NoteStorageService>();
            services.AddSingleton<IEventPublisher, SQSEventPublisher>();
            services.AddSingleton<INoteEmailService, SESNoteEmailService>();

            //// Adds a Redis in-memory implementation of IDistributedCache.
            //services.AddDistributedRedisCache(options =>
            //{
            //    options.Configuration = Configuration["Caching:RedisPrimaryEndpoint"];
            //});

            services.AddSingleton(provider =>
                new DistributedCacheEntryOptions()
                .SetSlidingExpiration(
                    TimeSpan.FromMinutes(
                        int.Parse(Configuration["Caching:CacheExpirationInMinutes"])
                    )
                )
            );

            //services.AddDynamoDbSession(
            //    Configuration["Session:DynamoDbTableName"],
            //    Configuration["Session:DynamoDbRegion"],
            //    options =>
            //    {
            //        // Set a short timeout for easy testing.
            //        options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(Configuration["Session:IdleTimeoutInMinutes"]));
            //        options.Cookie.HttpOnly = true;
            //    });

            services.AddMvc();

            // call this in case you need aspnet-user-authtype/aspnet-user-identity
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                int? pageCount = context.Session.GetInt32("PageCount");

                // increment the number of pages viewed in the current session
                if (pageCount.HasValue)
                {
                    context.Session.SetInt32("PageCount", pageCount.Value + 1);
                }
                else
                {
                    context.Session.SetInt32("PageCount", 1);
                }

                await next.Invoke();
            });

            //app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
