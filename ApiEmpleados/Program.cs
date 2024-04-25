using ApiEmpleados.Data;
using ApiEmpleados.Repositories;
using ApiEmpleados.Helpers;
using Microsoft.EntityFrameworkCore;
using NSwag.Generation.Processors.Security;
using NSwag;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

#region NUGGETS
//JWBEARER
//SQLSERVER
//CORE
//NSwag.AspNetCore
#endregion

var builder = WebApplication.CreateBuilder(args);

#region SERVICE OAUTH

//CREAMOS UNA INSTANCIA DEL HELPER
HelperActionServicesOAuth helper =
    new HelperActionServicesOAuth(builder.Configuration);
builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);

#endregion

#region TOKEN CONFIG

//HABILITAMOS LOS SERVICIOS DE AUTHENTICATION QUE HEMOS 
//CREADO EN EL HELPER CON Action<>
builder.Services.AddAuthentication
    (helper.GetAuthenticationSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());

#endregion 



// Add services to the container.
builder.Services.AddTransient<EmpleadosRepository>();
string conn = builder.Configuration.GetConnectionString("SqlAzure");
builder.Services.AddDbContext<EmpleadoContext>(options =>
{
    options.UseSqlServer(conn);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region SWAGGER CONFIG

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Api Empleados";
    document.Description = "Api Empleados.";
    // CONFIGURAMOS LA SEGURIDAD JWT PARA SWAGGER,
    // PERMITE AÑADIR EL TOKEN JWT A LA CABECERA.
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
    url: "/swagger/v1/swagger.json",
    name: "api empleados");
    options.RoutePrefix = "";
    options.DocExpansion(DocExpansion.None);
});
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
