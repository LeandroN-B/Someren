using Someren.Repositories;

namespace Someren
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();
            builder.Services.AddControllersWithViews();

            var connectionString = builder.Configuration.GetConnectionString("test1database")
                                   ?? throw new InvalidOperationException("Connection string 'test1database' not found.");

            // Register repositories
            builder.Services.AddScoped<IDrinkRepository>(_ => new DrinkRepository(connectionString));
            builder.Services.AddScoped<IDrinkOrderRepository>(_ => new DrinkOrderRepository(connectionString));
            builder.Services.AddScoped<ILecturerDrinkOrderRepository>(_ => new LecturerDrinkOrderRepository(connectionString));

            builder.Services.AddSingleton<IRoomRepository, RoomRepository>();
            builder.Services.AddSingleton<ILecturerRepository, LecturerRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IActivityRepository, ActivityRepository>();




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
