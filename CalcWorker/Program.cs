// See https://aka.ms/new-console-template for more information
using CalcWorker.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
var client = new MongoClient("mongodb+srv://root:root@apicluster.cuy8h.mongodb.net/calc?retryWrites=true&w=majority");
var database = client.GetDatabase("calc");
IMongoCollection<Calc> calcs = database.GetCollection<Calc>("calcs");

var factory = new ConnectionFactory() { HostName = "localhost" };
int soma;
int number1;
int number2;
string idConta = "";
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "Calc",
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var calc = JsonSerializer.Deserialize<Calc>(message);

        Console.WriteLine($" Calculo  Status =  Pendente", message);



        int mydelay = 2000;
        Thread.Sleep(mydelay);
        AtualizarProcessando(calcs);
        Console.WriteLine($"  Calculo  Status =  Processando", message);
        Thread.Sleep(mydelay);
        AtualizarProcessado(calcs);
        AtualizarResultado(calcs);
        Console.WriteLine($"  Calculo  Status =  Processado", message);

    };
    channel.BasicConsume(queue: "Calc",
                         autoAck: true,
                         consumer: consumer);



    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();


}

void AtualizarProcessando(IMongoCollection<Calc> calcs)
{


    var filtro = Builders<Calc>.Filter.Where(x => x.status == "Pendente");
    var alteracao = Builders<Calc>.Update.Set(x => x.status, "Processando");
    calcs.UpdateMany(filtro, alteracao);

}

void AtualizarProcessado(IMongoCollection<Calc> calcs)
{
    var colecao = database.GetCollection<Calc>("calcs");

    var filtro = Builders<Calc>.Filter.Where(x => x.status == "Processando");
    var alteracao = Builders<Calc>.Update.Set(x => x.status, "Processado");
    calcs.UpdateMany(filtro, alteracao);

}
void AtualizarResultado(IMongoCollection<Calc> calcs)
{
    var builder = Builders<Calc>.Filter;

    var filtro = builder.Where(x => x.number1 == number1) & builder.Where(x => x.number2 == number2);
    var alteracao = Builders<Calc>.Update.Set(x => x.resultado, soma);
    calcs.UpdateMany(filtro, alteracao);
    Console.WriteLine("O total da Soma é : " + soma);


}