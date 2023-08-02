var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMembershipBlazorServices(
    userEndpoints => builder.Configuration.GetSection(
        UserEndpointsOptions.SectionKey).Bind(userEndpoints),
    appOptions => builder.Configuration.GetSection(
        AppOptions.SectionKey).Bind(appOptions))
    .AddMembershipMessageLocalizer()
    .AddMembershipValidators();


builder.Services.AddHttpClient<WebApiClient>(httpClient =>
httpClient.BaseAddress = new Uri("https://localhost:7242"))
    .AddMembershipBearerTokenHandler();

await builder.Build().RunAsync();
