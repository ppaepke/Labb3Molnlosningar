using Labb3Molnlösningar.Endpoints;
using Labb3Molnlösningar.Interface;
using Labb3Molnlösningar.Repositories;
using Labb3Molnlösningar.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Cosmos DB klient
var cosmosClient = new CosmosClient(
    "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    new CosmosClientOptions
    {
        HttpClientFactory = () => new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }),
        ConnectionMode = ConnectionMode.Gateway,
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    });

builder.Services.AddSingleton(cosmosClient);
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
builder.Services.AddSingleton<ISellerRepository, SellerRepository>();
builder.Services.AddSingleton<CustomerService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapCustomerEndpoints();
app.MapSellerEndpoints();

app.Run();