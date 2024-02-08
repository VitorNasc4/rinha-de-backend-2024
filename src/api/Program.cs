using Microsoft.AspNetCore.Http.HttpResults;
using api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/clients/{id}/transacoes", (int id, Transacao transacao) =>
{
  if (!transacao.IsValid())
  {
    return Results.BadRequest("Transação inválida.");
  }

  return Results.Ok(transacao);
});


app.MapGet("/clients/{id}/extrato", (int id) =>
{
  var cliente = new Cliente()
  {
    Id = id,
    Limite = 10000,
    Saldo = 0
  };
  return cliente;
});

app.Run();
