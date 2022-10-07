using System.Text.Json.Serialization;
using ApiCompositor.DependencyInjection;
using FastEndpoints;
using FastEndpoints.Swagger;
using NJsonSchema;
using Sample.Marketing;
using Sample.Sales;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(c => { c.SchemaType = SchemaType.OpenApi3; }, shortSchemaNames: true);

builder.Services.AddApiCompositor();
builder.Services.AddMarketingHandlers();
builder.Services.AddSalesHandlers();

var app = builder.Build();
app.UseDefaultExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Endpoints.ShortNames = true;
    c.Endpoints.RoutePrefix = "api";
    c.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
app.UseOpenApi();
app.UseSwaggerUi3(s =>
{
    s.ConfigureDefaults();
    s.DocExpansion = "list";
    s.TagsSorter = "alpha";
    s.OperationsSorter = "alpha";
});

app.Run();