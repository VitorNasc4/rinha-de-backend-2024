using api.Models;
using Microsoft.EntityFrameworkCore;
using api.Contexto;
using api.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<Context>
    (option =>
    option.UseNpgsql("Host=db;Port=5432;Database=rinha;User Id=admin;Password=123"));


builder.Services.AddSwaggerGen();

var app = builder.Build();

ApplyMigrations(app);
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/clients/{id}/transacoes", async (int id, CreateTranscaoDTO transacaoModel, Context context, IMapper mapper) =>
{
  if (!transacaoModel.IsValid())
  {
    return Results.BadRequest("Transação inválida.");
  }

  await using var requisicao = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

  var cliente = await context.Clientes!.Include(c => c.Transacoes)
    .FirstOrDefaultAsync(c => c.Id == id);
  if (cliente == null)
  {
    return Results.NotFound("Cliente não encontrado.");
  }

  var transacao = mapper.Map<Transacao>(transacaoModel);

  cliente.Transacoes ??= new List<Transacao>();
  cliente.Transacoes.Add(transacao);

  if (transacao.Tipo == "c")
  {
    cliente.Saldo += transacao.Valor;
  }
  else if (cliente.Saldo - transacao.Valor < -cliente.Limite)
  {
    return Results.Problem(new ProblemDetails
    {
      Status = 422,
      Title = "Não é possível completar a transação",
      Detail = "A transação excede o limite de saldo disponível do cliente."
    });
  }
  else
  {
    cliente.Saldo -= transacao.Valor;
  }

  await context.SaveChangesAsync();
  await requisicao.CommitAsync();

  return Results.Ok(new ReturnTransacoesDTO
  {
    Saldo = cliente.Saldo,
    Limite = cliente.Limite
  });
});

app.MapGet("/clients/{id}/extrato", async (int id, Context context) =>
{
  var extrato = await context.Clientes!
    .Where(cli => cli.Id == id)
    .Select(cli => new
    {
      Saldo = cli.Saldo,
      Limite = cli.Limite,
      Data_Extrato = DateTime.Now,
      Ultimas_Transacoes = cli.Transacoes!
        .OrderByDescending(t => t.CreatedAt)
        .Take(10)
        .Select(t => new ReadTransacoesDTO
        {
          Valor = t.Valor,
          Tipo = t.Tipo,
          Descricao = t.Descricao,
          Realizada_Em = t.CreatedAt
        }).ToList()
    })
    .FirstOrDefaultAsync();

  if (extrato == null)
  {
    return Results.NotFound("Cliente não encontrado.");
  }

  var retornoExtrato = new returnExtratoDTO()
  {
    Saldo = new ReadSaldoDTO()
    {
      Total = extrato.Saldo,
      Limite = extrato.Limite,
      Data_Extrato = extrato.Data_Extrato
    },
    Ultimas_Transacoes = extrato.Ultimas_Transacoes
  };

  return Results.Ok(retornoExtrato);
});

app.Run();

void ApplyMigrations(IApplicationBuilder app)
{
  using (var scope = app.ApplicationServices.CreateScope())
  {
    try
    {
      var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
      dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
      var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "Ocorreu um erro ao aplicar as Migrations");
    }
  }
}