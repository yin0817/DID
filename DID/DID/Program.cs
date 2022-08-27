using Autofac;
using Autofac.Extensions.DependencyInjection;
using DID.Helps;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder()
.AddJsonFile("appsettings.json").Build();
// Add services to the container.

//全局添加需要认证
builder.Services.AddControllers().AddMvcOptions(options => options.Filters.Add(new AuthorizeFilter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//log4net
builder.Services.AddLogging(cfg =>
{
    cfg.AddLog4Net("Config/log4net.config");
});
builder.Services.AddSingleton(new AppSettings(configuration));
//Autofac注入
builder.Host
.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterAssemblyTypes(Assembly.Load(Assembly.GetExecutingAssembly().GetName().Name!))
       .Where(t => t.Name.EndsWith("Service"))
       .AsImplementedInterfaces();
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "DID",
        Description = "DID接口"
    });
    //添加安全定义
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "请输入token,格式为 Bearer xxxxxxxx（注意中间必须有空格）",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    //添加安全要求
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme{
                Reference =new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id ="Bearer"
                }
            },new string[]{ }
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//jwt认证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //取出私钥
        var secretByte = Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]);
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            //验证发布者
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            //验证接收者
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Authentication:Audience"],
            //验证是否过期
            ValidateLifetime = true,
            //验证私钥
            IssuerSigningKey = new SymmetricSecurityKey(secretByte)
        };
    });

builder.Services.AddMemoryCache();

builder.Services.AddCors(c =>
{
    if (AppSettings.GetValue<bool>("Cors", "EnableAllIPs"))
    {
        //允许任意跨域请求
        c.AddPolicy(AppSettings.GetValue("Cors", "PolicyName"),
            policy =>
            {
                policy
                    .SetIsOriginAllowed(host => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
    }
    else
    {
        c.AddPolicy(AppSettings.GetValue("Cors", "PolicyName"),
            policy =>
            {
                policy
                    .WithOrigins(AppSettings.GetValue("Cors", "IPs").Split(','))
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    }
});

//builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);//请求参数为空判断

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

//静态文件
var upload = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images/AuthImges/");
if (!Directory.Exists(upload)) Directory.CreateDirectory(upload);
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(upload), //用于定位资源的文件系统
    RequestPath = new PathString("/Images/AuthImges") //请求地址
});

// CORS跨域
app.UseCors(AppSettings.GetValue("Cors", "PolicyName"));
//添加jwt验证
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//异常
app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>();
        if (exception != null)
        {
            app.Logger.LogError(exception.Error.Message);
            var error = new Response {Code = 1 ,Message = "服务器错误!"};
            var errObj = JsonConvert.SerializeObject(error);
            await context.Response.WriteAsync(errObj).ConfigureAwait(false);
        }
    });
});

app.Run();
