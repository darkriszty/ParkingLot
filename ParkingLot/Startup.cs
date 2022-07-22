using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ParkingLot.Bll;
using ParkingLot.Dal;

namespace ParkingLot
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<TicketDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ParkingLotDatabase")));
            services.AddTransient<ITicketsRepository, TicketsRepository>();
            services.AddTransient<TicketPriceCalculator>();
            services.AddTransient<TicketPaymentService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwaggerUI(options =>
                             {
                                 options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                                 options.RoutePrefix = string.Empty;
                             });

            UpdateDatabase(app);
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                                        .GetRequiredService<IServiceScopeFactory>()
                                        .CreateScope();
            using var context = serviceScope.ServiceProvider.GetRequiredService<TicketDbContext>();
            context.Database.Migrate();
        }
    }
}
