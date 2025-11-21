using KingdomHospital.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;
using KingdomHospital.Application.Mappers;
using KingdomHospital.Application.Services;
using KingdomHospital.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSerilog((services, lc) =>
    lc.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// MOI  Add EF Core DbContext (SQL Server) 
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalDb")));

//Ajouter les Mappers au DI Transient
builder.Services.AddTransient<SpecialtyMapper>();
builder.Services.AddTransient<DoctorMapper>();
builder.Services.AddTransient<PatientMapper>();
builder.Services.AddTransient<MedicamentMapper>();
builder.Services.AddTransient<ConsultationMapper>();
builder.Services.AddTransient<OrdonnanceMapper>();
builder.Services.AddTransient<OrdonnanceLigneMapper>();

//Ajouter les service au DI scoped
builder.Services.AddScoped<SpecialtyService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<MedicamentService>();
builder.Services.AddScoped<ConsultationService>();
builder.Services.AddScoped<OrdonnanceService>();
builder.Services.AddScoped<OrdonnanceLigneService>();

//Ajouter les repositories au DI
builder.Services.AddScoped<SpecialtyRepository>();
builder.Services.AddScoped<PatientRepository>();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<ConsultationRepository>();
builder.Services.AddScoped<OrdonnanceRepository>();
builder.Services.AddScoped<OrdonnanceLigneRepository>();
builder.Services.AddScoped<MedicamentRepository>();




var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();
    SeedData.Initialize(context);
}

app.Run();
