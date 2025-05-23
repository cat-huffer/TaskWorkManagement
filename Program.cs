using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskWorkManagement.Models;
using TaskWorkManagement.Data;

//初始化Web应用构建器。自动加载appsettings.json等配置文件；设置默认日志、配置等基础设施
var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器。注册了MVC模式所需的服务；支持Controller、View和Razor Pages
builder.Services.AddControllersWithViews()
     .AddRazorRuntimeCompilation(); // <-- 在这里添加运行时编译


// 数据库 EntityFrameworkCore 注入。注册数据库上下文
builder.Services.AddDbContext<TaskWorkManagementContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TaskWorkManagementContext") ?? throw new InvalidOperationException("Connection string 'TaskWorkManagementContext' not found.")));

//注册HTTP上下文访问器
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(); // 配置 session 访问服务

//构建WebApplication实例
var app = builder.Build();

//添加数据库种子初始值设定项
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// 配置 HTTP 请求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//将所有HTTP请求重定向到HTTPS，提高安全性
app.UseHttpsRedirection();

// 静态文件中间件（放在路由之前）
app.UseStaticFiles(); // ← 关键位置

app.UseRouting();
app.UseSession(); // 使用 session 中间件
app.UseAuthorization();

app.MapStaticAssets();

//配置默认控制器路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();

//启动应用程序
app.Run();
