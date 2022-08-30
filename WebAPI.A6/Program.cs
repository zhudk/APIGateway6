using ConsulBase6;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.WriteIndented = true; });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//consul
builder.Services.AddHealthChecks();
builder.Services.AddConsul();

var app = builder.Build();

// 获取服务配置项
var serviceOptions = app.Services.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

//配置健康检测地址，.net core 内置的健康检测地址中间件
app.UseHealthChecks(serviceOptions.HealthCheck);
app.UseConsul();

app.Run();
