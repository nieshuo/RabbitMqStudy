using MassTransit;
using Microservice.Bill.Api;
using Microservice.Bill.Api.Data;
using Microservice.Bill.Api.SignalR;
using Microservice.Common.Contracts.Inventory;
using Microservice.Common.Contracts.Stock;
using Microservice.Common.CQRS;
using Microservice.Common.MassTransit;
using Microservice.Common.Snowflake;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransitWithRabbitMq()
    .AddMediatRServices()
    .AddSnowflake(builder.Configuration);

//使用send发送，需要绑定queue
EndpointConvention.Map<AddStockEvent>(new Uri("queue:add-stock"));
EndpointConvention.Map<AddInventoryEvent>(new Uri("queue:add-ware-inventory"));

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<MessageHub>().AddSignalR();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MassBill_db")));

builder.Services.AddCors(options =>
{
    // this defines a CORS policy called "default"
    options.AddPolicy("default", policy =>
    {
        policy.AllowAnyOrigin()
            //policy.WithOrigins("http://172.20.0.206", "http://116.62.149.236:8081")
            .AllowAnyHeader()
            .AllowAnyMethod();
        //.AllowCredentials();
    });
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); //序列化时key为驼峰样式
    //options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
    //options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;//忽略循环引用
});

builder.Services.AddEndpointsApiExplorer();
var swaggerName = "v1";
var swaggerTitle = "系统Api文档";
var swaggerVersion = "1.0.0";
builder.Services.AddSwaggerGen(c =>
{
    //Bearer 的scheme定义
    var securityScheme = new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        //参数添加在头部
        In = ParameterLocation.Header,
        //使用Authorize头部
        Type = SecuritySchemeType.Http,
        //内容为以 bearer开头
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    //把所有方法配置为增加bearer头部信息
    var securityRequirement = new OpenApiSecurityRequirement
        {{
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "bearerAuth"
                }
            },
            new string[] {}
        }};

    //注册到swagger中
    c.AddSecurityDefinition("bearerAuth", securityScheme);
    c.AddSecurityRequirement(securityRequirement);
    c.SwaggerDoc(swaggerName, new OpenApiInfo
    {
        Title = swaggerTitle,
        Version = swaggerVersion,
        Description = $"接口描述"
    });

    //Locate the XML file being generated by ASP.NET...
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    //... and tell Swagger to use those XML comments.    
    //true:显示控制器层注释
    c.IncludeXmlComments(xmlPath, true);
    //对action的名称进行排序，如果有多个，就可以看见效果了
    c.OrderActionsBy(x => x.RelativePath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"/swagger/{swaggerName}/swagger.json", $"{swaggerTitle + ":" + swaggerVersion}");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MessageHub>("/messagehub");

app.Run();
