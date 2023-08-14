using FundaReport.Services;
using FundaReport.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
var settings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

var corsPolicyName = "_corsOriginsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy.WithOrigins(settings.FrontendSettings.BaseUrl);
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ReportService>();
builder.Services.AddHttpClient<FundaHttpService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(settings.FundaApiSettings.BaseUrl));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();
