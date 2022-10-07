using FastEndpoints;
using FastEndpoints.Swagger;
using Sample.Marketing;
using Sample.Sales;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc();

builder.Services.AddApiCompositor();
builder.Services.AddMarketingHandlers();
builder.Services.AddSalesHandlers();

var app = builder.Build();
app.UseDefaultExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerUi3(s => s.ConfigureDefaults());

app.Run();