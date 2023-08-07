namespace Membership.Blazor.Components;

public partial class ExternalIDPsButtons
{
    [Inject]
    IAuthorizeService AuthorizeService { get; set; }

    [Parameter]
    public ScopeAction ScopeAction { get; set; }

    [Parameter]
    public RenderFragment<string> ButtonContent { get; set; }

    [Parameter]
    public string ReturnUri { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    ExternalIDPInfo[] IDPs => AuthorizeService.IDPInfos;

    string ContentPath =>
        $"_content/{this.GetType().Assembly.GetName().Name}";

    string ImagePath(ExternalIDPInfo idpInfo) =>
        string.IsNullOrWhiteSpace(idpInfo.ImagePath) ?
        $"{ContentPath}/images/idps/{idpInfo.ProviderId}.png" :
        idpInfo.ImagePath;

    async void BuildUrl(string providerId)
    {
        await AuthorizeService.AuthorizeAsync(providerId, ScopeAction, ReturnUri);
    }
}
