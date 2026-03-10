var builder = DistributedApplication.CreateBuilder(args);

var mySql = builder.AddMySql("mysql-server")
                   .WithImageTag("8.4.8") // latest LTS version
                   .AddDatabase("OmdbCacheDb");

var apiService = builder.AddProject<Projects.OmdbTerminal_ApiService>("apiservice")
                        .WithReference(mySql);

var app = builder.Build();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n=======================================================================");
Console.WriteLine(" OMDB TERMINAL IS RUNNING");
Console.WriteLine(" -----------------------------------------------------------------------");
Console.WriteLine(" 1. The Aspire Dashboard will open in your browser automatically");
Console.WriteLine(" 2. To view SWAGGER, click the 'apiservice' endpoint link in the dashboard!");
Console.WriteLine("=======================================================================\n");
Console.ResetColor();

app.Run();
