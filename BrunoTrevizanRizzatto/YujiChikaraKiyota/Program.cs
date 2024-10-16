// Avaliação Desenvolvimento Web Quarta-Feira Manhã
// Integrantes: Bruno Trevizan Rizzatto e Yuji Chikara Kiyota

using Microsoft.AspNetCore.Http.HttpResults;
using YujiChikaraKiyota.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();

// Pagina inicial projeto
app.MapGet("/", () => "Avaliação DevWeb Bruno e Yuji");

// Cadastrar funcionário
app.MapPost("/API/funcionario/cadastrar", ([FromBody] Funcionario funcionario, [FromServices] AppDataContext ctx) =>
{
    ctx.Funcionarios.Add(funcionario);
    ctx.SaveChanges();
    return Results.Created("", funcionario);
});

// Listar funcionário
app.MapGet("/API/funcionario/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Funcionarios.Any())
    {
        return Results.Ok(ctx.Funcionarios.ToList());
    }

    return Results.NotFound();
});

app.MapPost("/API/folha/cadastrar", ([FromBody] Folha folhas, [FromServices] AppDataContext ctx)=>
{
    // Salario bruto
    folhas.SalarioBruto = folhas.Valor * folhas.Quantidade;
    // calculo imposto de renda
    if (folhas.SalarioBruto > 1903.98 && folhas.SalarioBruto <= 2826.65)
    {
        folhas.ImpostoIrrf = (folhas.SalarioBruto * (7.5 / 100)) - 142.80;
    }else if (folhas.SalarioBruto > 2826.65 && folhas.SalarioBruto <= 3751.05)
    {
        folhas.ImpostoIrrf = (folhas.SalarioBruto * (15.0 / 100)) - 354.80;
    }else if (folhas.SalarioBruto > 3751.05 && folhas.SalarioBruto <= 4664.68)
    {
        folhas.ImpostoIrrf = (folhas.SalarioBruto * (22.5 / 100)) - 636.13;
    }else if (folhas.SalarioBruto > 4664.68)
    {
        folhas.ImpostoIrrf = (folhas.SalarioBruto * (27.5 / 100)) - 869.36;
    }
    else
    {
        folhas.ImpostoIrrf = 0;
    }
    //calculo fgts
    folhas.ImpostoFgts = (folhas.SalarioBruto * 0.08);

    if (folhas.SalarioBruto > 1693.72 && folhas.SalarioBruto <= 2822.90)
    {
        folhas.ImpostoInss = (folhas.SalarioBruto * 0.09);
    }else if (folhas.SalarioBruto > 2822.90 && folhas.SalarioBruto <= 5645.80)
    {
        folhas.ImpostoInss = (folhas.SalarioBruto * 0.11);
    }else if (folhas.SalarioBruto > 5645.80)
    {
        folhas.ImpostoInss = 621.03;
    }else
    {
        folhas.ImpostoInss = (folhas.SalarioBruto * 0.08);
    }
    
    //calculo salario liquido
    folhas.SalarioLiquido = folhas.SalarioBruto - folhas.ImpostoIrrf - folhas.ImpostoInss;
    folhas.Funcionario = new Funcionario();
    ctx.Folhas.Add(folhas);
    ctx.SaveChanges();
    return Results.Created("", folhas);
});

app.MapGet("/API/folha/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Folhas.Any())
    {
        return Results.Ok(ctx.Folhas.ToList());
    }

    return Results.NotFound();
});

app.MapPost("/API/folha/buscar/{cpf}/{mes}/{ano}",([FromRoute] string cpf, int mes, int ano, [FromServices] AppDataContext ctx) =>
{

    Folha? folha = ctx.Folhas.Find(cpf, mes, ano);
    if (folha is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(folha);

});
app.Run();
