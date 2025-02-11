using Sheyaaka.Configuration.Extentions;

var builder = WebApplication.CreateBuilder(args);

//This is a custom extension method that registers core host configurations (like logging) before the DI Container is set up
BuilderHostExtensions.RegisterHost(builder.Host);

//This is a custom extension method that registers services to the DI Container
//This is done to keep the Startup.cs clean and readable
ServiceCollectionExtensions.RegisterServices(builder);

var app = builder.Build();

//This is a custom extension method that configures the application pipeline
//This is done to keep the Startup.cs clean and readable
ApplicationPipelineExtensions.ConfigurePipeline(app);

app.Run();
