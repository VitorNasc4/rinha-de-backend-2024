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
    option.UseNpgsql("Host=db;Port=5432;Database=rinha;User Id=admin;Password=123;Minimum Pool Size=10;Maximum Pool Size=2000;Multiplexing=true;"));


builder.Services.AddSwaggerGen();

var app = builder.Build();

// ApplyMigrations(app);
app.UseSwagger();
app.UseSwaggerUI();

var clientes = new Dictionary<int, int>
{
    {1,   1000 * 100},
    {2,    800 * 100},
    {3,  10000 * 100},
    {4, 100000 * 100},
    {5,   5000 * 100}
};

app.MapPost("/clientes/{id}/transacoes", async (int id, CreateTranscaoDTO transacaoModel, Context context, IMapper mapper) =>
{
  if (!transacaoModel.IsValid())
  {
    return Results.BadRequest("Transação inválida.");
  }

  if (!clientes.ContainsKey(id))
  {
    return Results.NotFound("Cliente não encontrado.");
  }

  var limite = clientes[id];
  var transacao = mapper.Map<Transacao>(transacaoModel);
  transacao.IdCliente = id;

  await using var requisicao = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
  var cliente = await context.Clientes!.FirstAsync(c => c.Id == id);

  if (transacao.Tipo == "c")
  {
    cliente.Saldo += transacao.Valor;
    context.Transacoes!.Add(transacao);
    await context.SaveChangesAsync();
    await requisicao.CommitAsync();

    return Results.Ok(new ReturnTransacoesDTO
    {
      Saldo = cliente.Saldo,
      Limite = limite
    });
  }

  if (cliente.Saldo - transacao.Valor < -limite)
  {
    return Results.Problem(new ProblemDetails
    {
      Status = 422,
      Title = "Não é possível completar a transação",
      Detail = "A transação excede o limite de saldo disponível do cliente."
    });
  }

  cliente.Saldo -= transacao.Valor;
  context.Transacoes!.Add(transacao);

  await context.SaveChangesAsync();
  await requisicao.CommitAsync();

  return Results.Ok(new ReturnTransacoesDTO
  {
    Saldo = cliente.Saldo,
    Limite = limite
  });
});

app.MapGet("/clientes/{id}/extrato", async (int id, Context context) =>
{
  if (!clientes.ContainsKey(id))
  {
    return Results.NotFound("Cliente não encontrado.");
  }
  var limite = clientes[id];

  var extrato = await context.Clientes!
    .Where(cli => cli.Id == id)
    .Select(cli => new
    {
      Saldo = cli.Saldo,
      Limite = limite,
      Data_Extrato = DateTime.Now,
      Ultimas_Transacoes = context.Transacoes!
        .Where(t => t.IdCliente == id)
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

  var retornoExtrato = new returnExtratoDTO()
  {
    Saldo = new ReadSaldoDTO()
    {
      Total = extrato!.Saldo,
      Limite = extrato.Limite,
      Data_Extrato = extrato.Data_Extrato
    },
    Ultimas_Transacoes = extrato.Ultimas_Transacoes
  };

  return Results.Ok(retornoExtrato);
});

app.Run();

// void ApplyMigrations(IApplicationBuilder app)
// {
//   using (var scope = app.ApplicationServices.CreateScope())
//   {
//     try
//     {
//       var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
//       dbContext.Database.Migrate();
//     }
//     catch (Exception ex)
//     {
//       var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//       logger.LogError(ex, "Ocorreu um erro ao aplicar as Migrations");
//     }
//   }
// }