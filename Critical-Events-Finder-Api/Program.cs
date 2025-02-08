using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Critical_Events_Finder_Api.Interfaces;
using Critical_Events_Finder_Api.Models;
using Critical_Events_Finder_Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Configuration.AddUserSecrets<Program>();

// Bind AWS settings using IOptions<T>
builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AWS"));

// Register IAmazonS3 using IOptions<AwsSettings>
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var awsSettings = sp.GetRequiredService<IOptions<AwsSettings>>().Value;
    var credentials = new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey);
    var region = RegionEndpoint.GetBySystemName(awsSettings.Region);
    return new AmazonS3Client(credentials, region);
});

// Register services
builder.Services.AddScoped<ICriticalEventsService, CriticalEventsService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Critical Events Finder API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles(); 
app.UseStaticFiles();  
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
