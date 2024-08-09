using Asp.Versioning;
using LojaVirtual.Server.Contexts;
using LojaVirtual.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("LojaVirtual.Server.NewTest")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IProdutosRepository, ProdutoRepository>();

#region DocumentacaoApi
string versao = "v1";
var informacoesApi = new OpenApiInfo
{
    Version = versao,
    Title = "API de uma loja virtual",
    Description = "Esse é um estudo sobre APIs",
    TermsOfService = new Uri("https://exemplo.com/termos"),
    Contact = new OpenApiContact
    {
        Name = "Guilherme",
        Email = "guilhermealves@email.com"
    },
    License = new OpenApiLicense
    {
        Name = "Licença",
        Url = new Uri("https://exemplo.com/licenca")
    }
};
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(versao, informacoesApi);
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
}
);
#endregion

#region Database
string conexao = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conexao));
#endregion

#region Versionamento
builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true; // Se o cliente não informar a versão da api então será utilizada a versão padrão.
    option.DefaultApiVersion = new ApiVersion(1, 0); // Define a versão padrão da API
    option.ReportApiVersions = true; //The allow the API Version information to be reported in the client  in the response header. This will be useful for the client to understand the version of the API they are interacting with.
    option.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"), // Habilitando o versionamento via query string (api-version)
        new HeaderApiVersionReader("X-Version"), // Habilitando o versionamento pelo header da requisição (X-Version)
        new MediaTypeApiVersionReader("ver")); // Habilitando o versionamento via media type (ver)                                              
}).AddApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV"; //The say our format of our version number “‘v’major[.minor][-status]”
    options.SubstituteApiVersionInUrl = true; //This will help us to resolve the ambiguity when there is a routing conflict due to routing template one or more end points are same.
});
#endregion

#region Cors
builder.Services.AddCors
(
    options => options.AddDefaultPolicy
    (
        policy =>
        {
            policy.WithOrigins("http://localhost:5173").AllowAnyMethod();
        }
    )
);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }