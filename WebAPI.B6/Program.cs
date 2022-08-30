using ConsulBase6;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//consul
builder.Services.AddHealthChecks();
builder.Services.AddConsul();

var app = builder.Build();
// ��ȡ����������
var serviceOptions = app.Services.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
//���ý�������ַ��.net core ���õĽ�������ַ�м��
app.UseHealthChecks(serviceOptions.HealthCheck);
app.UseConsul();
app.Run();
