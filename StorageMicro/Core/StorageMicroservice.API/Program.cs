using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using StorageMicroservice.Application.Commands;
using StorageMicroservice.Infrastructure.Data;
using StorageMicroservice.Infrastructure.EventBus;
using StorageMicroservice.Infrastructure.Repository;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using StorageMicroservice.Domain.Entities;
using StorageMicroservice.Infrastructure.StorageProviders;
using Amazon.S3;
using Microsoft.Extensions.Options;
using Amazon;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Don't serialize null values
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    // Pretty print JSON
    options.SerializerOptions.WriteIndented = true;
});
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        if (activity != null)
        {
            context.ProblemDetails.Extensions.TryAdd("traceId", activity.Id);
        }
    };
});

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(builder.Configuration["MongoDb:ConnectionString"]));
builder.Services.AddScoped<MongoDbContext>();
builder.Services.AddScoped<IRepository<FileMetadata>, FileRepository>();
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
// Configure  Settings
builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<AzureBlobStorageSettings>(builder.Configuration.GetSection("Storage:AzureBlob"));
builder.Services.Configure<S3StorageSettings>(builder.Configuration.GetSection("Storage:S3"));
builder.Services.Configure<LocalStorageSettings>(builder.Configuration.GetSection("Storage:Local"));
// Register IAmazonS3
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<S3StorageSettings>>().Value;
    return new AmazonS3Client(settings.AccessKey, settings.SecretKey, RegionEndpoint.GetBySystemName(settings.Region));
});
// Configure Azure Blob Storage Settings
// Register BlobServiceClient
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<AzureBlobStorageSettings>>().Value;
    return new BlobServiceClient(settings.ConnectionString);
});

builder.Services.AddSingleton<S3StorageProvider>();
builder.Services.AddSingleton<AzureBlobStorageProvider>();
builder.Services.AddSingleton<LocalStorageProvider>();
builder.Services.AddSingleton<IStorageProvider, StorageProviderFactory>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UploadFileCommand).Assembly));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Storage Microservice")
            .WithDownloadButton(true)
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
    });

    app.UseRouting();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseDefaultFiles();

app.MapControllers();

app.Run();
