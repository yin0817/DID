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

//ȫ�������Ҫ��֤
builder.Services.AddControllers().AddMvcOptions(options => options.Filters.Add(new AuthorizeFilter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//log4net
builder.Services.AddLogging(cfg =>
{
    cfg.AddLog4Net("Config/log4net.config");
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton(new AppSettings(configuration));
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

//Autofacע��
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
        Description = "DID�ӿ�"
    });
    //��Ӱ�ȫ����
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "������token,��ʽΪ Bearer xxxxxxxx��ע���м�����пո�",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    //��Ӱ�ȫҪ��
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

//jwt��֤
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //ȡ��˽Կ
        var secretByte = Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]);
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            //��֤������
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            //��֤������
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Authentication:Audience"],
            //��֤�Ƿ����
            ValidateLifetime = true,
            //��֤˽Կ
            IssuerSigningKey = new SymmetricSecurityKey(secretByte)
            //ע�����ǻ������ʱ�䣬�ܵ���Чʱ��������ʱ�����jwt�Ĺ���ʱ�䣬��������ã�Ĭ����5����
            //ClockSkew = TimeSpan.FromSeconds(4)
        };
    });

builder.Services.AddMemoryCache();

builder.Services.AddCors(c =>
{
    if (AppSettings.GetValue<bool>("Cors", "EnableAllIPs"))
    {
        //���������������
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

//builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);//�������Ϊ���ж�

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

//��̬�ļ�
var upload = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images/");
if (!Directory.Exists(upload)) Directory.CreateDirectory(upload);
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(upload), //���ڶ�λ��Դ���ļ�ϵͳ
    RequestPath = new PathString("/Images") //�����ַ
});

// CORS����
app.UseCors(AppSettings.GetValue("Cors", "PolicyName"));
//���jwt��֤
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//�쳣
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
            var error = new Response {Code = 1 ,Message = "����������!"};
            var errObj = JsonConvert.SerializeObject(error);
            await context.Response.WriteAsync(errObj).ConfigureAwait(false);
        }
    });
});

app.Run();
