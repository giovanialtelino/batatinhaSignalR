using BatatinhaSignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Channels;
using BatatinhaSignalR.Model;
using BatatinhaSignalR.HostedService;

namespace BatatinhaSignalR
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRazorPages();
            services.AddSignalR();

            services.AddControllers();

            services.AddSwaggerGen();

            var channelOne = Channel.CreateUnbounded<ValueControllerModelOne>();
            services.AddSingleton<ChannelReader<ValueControllerModelOne>>(channelOne.Reader);
            services.AddSingleton<ChannelWriter<ValueControllerModelOne>>(channelOne.Writer);

            var channelTwo = Channel.CreateUnbounded<ValueControllerModelTwo>();
            services.AddSingleton<ChannelReader<ValueControllerModelTwo>>(channelTwo.Reader);
            services.AddSingleton<ChannelWriter<ValueControllerModelTwo>>(channelTwo.Writer);

            services.AddHostedService<CalculatorHostedService>();
            services.AddHostedService<KafkaReader>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapRazorPages();
            });
        }
    }
}
