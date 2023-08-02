namespace Membership.Abstractions.Options;
public class IDPClientInfoOptions
{
    public const string SectionKey = "OAuth2:IDPClients";
    public IEnumerable<IDPClientInfo> IDPClients { get; set; }
}
