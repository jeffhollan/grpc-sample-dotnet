using Grpc.Net.Client;
using grpc_backend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<grpc_backend.Greeter.GreeterClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GRPC_SERVER_ADDRESS"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/hello", async (Greeter.GreeterClient client) =>
{
    var reply = await client.SayHelloAsync(
        new grpc_backend.HelloRequest { Name = "Azure Container Apps "}
    );
    return reply.Message;
})
.WithName("GetHello");

app.Run();