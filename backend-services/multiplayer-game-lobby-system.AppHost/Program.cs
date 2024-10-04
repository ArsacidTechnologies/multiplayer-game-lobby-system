var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.GameActionHandlerService>("gameactionhandlerservice");

builder.Build().Run();
