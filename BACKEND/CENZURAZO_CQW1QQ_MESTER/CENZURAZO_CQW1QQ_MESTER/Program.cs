
using CENZURAZO_CQW1QQ_MESTER.Data;
using CENZURAZO_CQW1QQ_MESTER.Services;



namespace CENZURAZO_CQW1QQ_MESTER
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // IoC
            builder.Services.AddDbContext<CensorDbContext>();
            builder.Services.AddTransient<ICensorRepository, CensorRepository>();

            object value = builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseRouting();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");

            app.MapGet("/", () => "Hello World!");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(x => x
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins("http://localhost:5500")
                .SetIsOriginAllowed(origin => true));

            app.Run();
        }
    }
}