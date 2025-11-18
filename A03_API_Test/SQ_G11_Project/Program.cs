/*
 * FILE          : Program.cs
 * PROJECT       : SENG2020 - Group Project #01
 * PROGRAMMER    : Burhan Shibli, Daeseong Yu, Nick Turco, Sungmin Leem
 * FIRST VERSION : 2025-10-31
 * DESCRIPTION   : The entry point of the program
 */

using TaskManagement.Services;

namespace TaskManagement
{
    /*
     * NAME     : 
     * PURPOSE  : 
     */
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<TaskManagement.Services.TaskServices>();
            builder.Services.AddScoped<TaskManagement.Repositories.TaskRepository>();
            builder.Services.AddScoped<PriorityServices>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<TaskManagement.Repositories.TaskRepository>();
            builder.Services.AddScoped<TaskManagement.Services.AssigneeServices>();

            var app = builder.Build();

            //// Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
