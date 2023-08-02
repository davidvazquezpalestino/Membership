namespace Membership.Abstractions.Options;
public class AppClientInfoOptions
{
    public const string SectionKey = "OAuth2:AppClients";
    public IEnumerable<AppClientInfo> AppClients { get; set; }
}
