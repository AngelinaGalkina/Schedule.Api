using Planday.Schedule.ApiClient;
using Planday.Schedule.Infrastructure.ApiClient;
using Planday.Schedule.Infrastructure.Providers;
using Planday.Schedule.Infrastructure.Providers.Interfaces;
using Planday.Schedule.Infrastructure.Queries.Insert;
using Planday.Schedule.Infrastructure.Queries.Select;
using Planday.Schedule.Infrastructure.Queries.Update;
using Planday.Schedule.Queries.Insert;
using Planday.Schedule.Queries.Select;
using Planday.Schedule.Queries.Update;
using Planday.Schedule.Services;
using Planday.Schedule.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IConnectionStringProvider>(new ConnectionStringProvider(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ISelectShiftsQuery, SelectShiftsQuery>();
builder.Services.AddScoped<ISelectEmployeeQuery, SelectEmployeeQuery>();
builder.Services.AddScoped<IUpdateShiftsQuery, UpdateShiftsQuery>();
builder.Services.AddScoped<IInsertShiftsQuery, InsertShiftsQuery>();
builder.Services.AddScoped<IEmailApiClient, EmailApiClient>();
builder.Services.AddScoped<ICreateShiftValidator, CreateShiftValidator>();
builder.Services.AddScoped<IAssignShiftToEmployeeValidator, AssignShiftToEmployeeValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
