// ============================================================
// CrmFunctions - Azure Function med Cosmos DB Trigger
// ============================================================
// Detta projekt utgör trigger-lagret i systemet.
// 
// Flöde:
// 1. Ett dokument skapas/uppdateras i Cosmos DB (Customers-containern)
// 2. Cosmos DB Change Feed fångar upp ändringen
// 3. Denna Azure Function triggas automatiskt
// 4. Funktionen skickar ett email till ansvarig säljare via Mailtrap (SMTP)
//
// Lease-containern håller koll på var i Change Feed vi senast läste,
// så vi inte missar eller dubbelbehandlar ändringar.
// ============================================================

using CrmFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace CrmFunctions;

public class CustomerTrigger
{
    private readonly ILogger _logger;

    // Mailtrap SMTP-inställningar för lokal emailtestning
    private const string MailtrapHost = "sandbox.smtp.mailtrap.io";
    private const int MailtrapPort = 2525;
    private const string MailtrapUsername = "0283695c6e1497";
    private const string MailtrapPassword = "2d0fafba4cba8d";

    public CustomerTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CustomerTrigger>();
    }

    // Triggas automatiskt när ett dokument läggs till eller uppdateras
    // i Customers-containern i CrmDatabase
    [Function("CustomerTrigger")]
    public async Task Run([CosmosDBTrigger(
        databaseName: "CrmDatabase",
        containerName: "Customers",
        Connection = "CosmosDbConnectionString",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Customer> customers)
    {
        if (customers == null || customers.Count == 0) return;

        foreach (var customer in customers)
        {
            _logger.LogInformation($"Kund förändrad: {customer.Name}");

            if (customer.AssignedSeller == null ||
                string.IsNullOrWhiteSpace(customer.AssignedSeller.Email))
            {
                _logger.LogWarning("Ingen ansvarig säljare hittades för kunden.");
                continue;
            }

            await SendEmailAsync(customer);
        }
    }

    // Skickar email till ansvarig säljare med kundens uppgifter
    private async Task SendEmailAsync(Customer customer)
    {
        var subject = $"Du är ansvarig för kunden: {customer.Name}";
        var body = $"""
            Hej {customer.AssignedSeller.Name},

            Du har blivit ansvarig säljare för följande kund:

            Namn:     {customer.Name}
            Titel:    {customer.Title}
            Telefon:  {customer.Phone}
            Email:    {customer.Email}
            Adress:   {customer.Address}

            Vänliga hälsningar,
            CRM-systemet
            """;

        using var client = new SmtpClient(MailtrapHost, MailtrapPort)
        {
            Credentials = new NetworkCredential(MailtrapUsername, MailtrapPassword),
            EnableSsl = true
        };

        var message = new MailMessage(
            from: "crm@företag.se",
            to: customer.AssignedSeller.Email,
            subject: subject,
            body: body
        );

        await client.SendMailAsync(message);
        _logger.LogInformation($"Email skickat till {customer.AssignedSeller.Email}");
    }
}