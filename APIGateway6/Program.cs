using Ocelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//ocelot 
var ocelotConfig = new ConfigurationBuilder().AddJsonFile("Ocelot.json").Build();
builder.Services.AddOcelot(ocelotConfig).AddCacheManager(x =>
{
    x.WithDictionaryHandle();
}).AddConsul().AddPolly();

//identityserver
builder.Services.AddAuthentication().AddJwtBearer("Bearer", options =>
{
    options.Authority = ocelotConfig.GetSection("IdentityServerUrl").Value;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});
//identityserver policy
builder.Services.AddAuthorization(optionns =>
{
    optionns.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
//网关节点
app.UseOcelot().Wait();
//授权鉴权
app.UseAuthentication();
app.UseAuthorization();
app.Run();
