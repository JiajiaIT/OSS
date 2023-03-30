using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Reflection;

namespace OSS
{
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OSS WebAPI HelpPage", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath, true);
            });

            // 设置允许所有来源跨域
            services.AddCors(options => options.AddPolicy("any",
            builder =>
            {
                builder.WithOrigins("urls")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(_ => true) // =AllowAnyOrigin()
                    .AllowCredentials();
            }));

            //开启br和gzip压缩
            services.AddResponseCompression(options =>
            {
                //options.Providers.Add<CustomResponseCompressionProvider>();
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();


            });

            //配置BrotliCompression 压缩级别  Optimal(最佳的压缩方式,耗费的时间较长)  Fastest(最快的压缩方式) NoCompression(不进行压缩)
            services.Configure<BrotliCompressionProviderOptions>(config =>
            {
                config.Level = CompressionLevel.Fastest;
            });
            //配置Gzip  压缩级别  Optimal(最佳的压缩方式,耗费的时间较长)  Fastest(最快的压缩方式) NoCompression(不进行压缩)
            services.Configure<GzipCompressionProviderOptions>(config =>
            {
                config.Level = CompressionLevel.Fastest;
            });


            long limitSize = (long)1024 * 1024 * 1024 * 1024 * 10;
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = limitSize;
                options.MultipartHeadersCountLimit = 100;
            }
            );
            //上传文件大小限制Kestrel设置
            services.Configure<KestrelServerOptions>(options =>
            {
                // Set the limit to 256 MB
                options.Limits.MaxRequestBodySize = limitSize;
            });
            //上传文件大小限制IIS设置
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = limitSize;
            });
            //返回Json不变小写
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OSS WebAPI HelpPage v1"));


            app.UseRouting();

            app.UseResponseCompression();

            app.UseAuthorization();
            // 使用跨域配置
            app.UseCors("any");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseFileServer(true);
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}

