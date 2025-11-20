    using Aspire.Hosting;
using Microsoft.AspNetCore.Builder;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container
//var sql = builder.AddSqlServer("sqlserver").WithDataVolume("sqlserver-data");

// Add SQL database (this creates a database inside the container)
//var db = sql.AddDatabase("BTPayProDb");
// Register your API and connect it to the DB
//builder.AddProject<Projects.BTPayPro_Api>("btpaypro-api")
// .WithReference(db);
// Ajouter ton API comme service Aspire

// Exemple si tu veux une DB SQL
// var db = builder.AddSqlServer("sqldb").AddDatabase("btpayprodb");
var sqlserver = builder.AddSqlServer("sqlserver")
    .WithDataVolume() // persistance des données
    .AddDatabase("BTPayProDb");

var api = builder.AddProject<Projects.BTPayPro_Api>("api");
var web = builder.AddProject<Projects.BTPayPro_WebUI>("web")
                 .WithReference(api); // frontend depends on API

api.WithReference(sqlserver);
// Si ton Api a besoin d’une DB, ajoute la dépendance
// api.WithReference(db);

builder.Build().Run();  
