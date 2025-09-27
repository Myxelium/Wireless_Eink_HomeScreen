using HomeApi.Registration;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(builder.Configuration.GetSection("Seq"));
});
builder.Services.AddHttpClient();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddIntegration(builder.Configuration);

var app = builder.Build();

app.UseStaticFiles();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
