using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using LapisApi.Extensions;
using LapisApi.Filter;
using LapisApi.Middleware;
using LapisApi.Services.Seed;
using LapisApi.App.BackgroundJobs.Scheduler;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomServices();
builder.Services.AddCustomSwagger();
builder.Services.AddApplicationDbContext(builder.Configuration);


builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();

builder.Services.Configure<ApiBehaviorOptions>(options =>
  {
    options.SuppressModelStateInvalidFilter = true;
  }
);

builder.Services.AddScoped<ActiveUserAuthorizationFilter>();

builder.Services.AddControllers(options =>
  {
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<AuthorizeByEnvironmentFilter>();
  }
);


builder.Services.AddDirectoryBrowser();


builder.Services.Configure<FormOptions>(options =>
  {
    options.MultipartBodyLengthLimit = 25 * 1024 * 1024; // 25MB
  }
);

builder.Services.AddCustomOptions(builder.Configuration);

builder.Services.AddMemoryCache();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCustomHangfire(builder.Configuration);

builder.Services.AddCustomIdentity();

builder.Services.AddCustomJwtAuthentication(builder.Configuration);


builder.Services.AddAuthorization();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddAutoMapper(typeof(Program));

builder.AddCustomSerilog();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();


app.UseFileServer(new FileServerOptions
  {
    FileProvider = new PhysicalFileProvider(
      Path.Combine(builder.Environment.ContentRootPath, "wwwroot")
    ),
    //RequestPath = "/uploads",
    EnableDirectoryBrowsing = true
  }
);


using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  await SeederService.SeedRolesAndAdminAsync(services);
}


/**********************************************************************************/
app.UseRouting();

if (!app.Environment.IsDevelopment())
{ 
  app.UseMiddleware<ExceptionMiddleware>();
}
//app.UseMiddleware<BypassAuthMiddleware>();

app.UseMiddleware<BotProtectionMiddleware>();

app.UseHttpsRedirection();

app.UseMiddleware<RateLimitMiddleware>();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// app.UseCors("AllowSpecificOrigins");
app.UseCors();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
  {
    Authorization = new IDashboardAuthorizationFilter[]
    {
    }
  }
);

HangfireRecurringJobs.RegisterRecurringJobs();
app.Run();