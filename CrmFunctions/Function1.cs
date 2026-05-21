using CrmFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace CrmFunctions;

public class CustomerTrigger
{
    private readonly ILogger _logger;

    private const string MailtrapHost = "sandbox.smtp.mailtrap.io";
    private const int MailtrapPort = 2525;
    private const string MailtrapUsername = "0283695c6e1497";
    private const string MailtrapPassword = "2d0fafba4cba8d";

    public CustomerTrigger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CustomerTrigger>();
    }

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