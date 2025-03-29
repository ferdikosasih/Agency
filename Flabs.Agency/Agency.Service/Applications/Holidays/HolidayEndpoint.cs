using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Agency.Service.Common;
using Agency.Service.Entities;
using Agency.Service.Infrastructure;
using EFCore.BulkExtensions;
using FastEndpoints;
using Microsoft.VisualBasic;

namespace Agency.Service.Holidays;

public class HolidayEndpoint(ApplicationDbContext dbContext) : EndpointWithoutRequest<HolidayResponse?>
{
    public override void Configure()
    {
        Post("/holidays");
        Description(builder =>
        {
            builder
                .Produces<HolidayResponse>(200)
                .Produces(400)
                .Produces(401)
                .Produces(403)
                .Produces(500);
        });
        Summary(s =>
        {
            s.Summary = "Import holidays calendar";
        });
        Roles(RoleName.Agent);
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        string path = "Data/Holidays.json";
        var response = new HolidayResponse();
        if (!File.Exists(path))
        {
            await SendNotFoundAsync(cancellationToken);
            return;
        }
        await using var json =  File.OpenRead("Data/Holidays.json");
        var jsonOptions = new JsonSerializerOptions()
        {
            Converters = { new DateOnlyConverterToUtc() }
        }; 
        response = await JsonSerializer
            .DeserializeAsync<HolidayResponse>(json,jsonOptions, cancellationToken: cancellationToken);
        if (response is null)
        {
            await SendErrorsAsync(500,cancellationToken);
            return;
        }
        await dbContext.BulkInsertOrUpdateAsync<Holiday>(
            response.Holidays.AsEnumerable(),cancellationToken:cancellationToken);
        await SendOkAsync(response,cancellationToken);
    }
}

public class DateOnlyConverterToUtc : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateOnly.ParseExact(reader.GetString()!, DateFormat, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}