using CommunityToolkit.Maui.Views;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using System.Security.Cryptography.X509Certificates;

namespace PKIArcGISDemo;

public partial class MainPage : ContentPage
{
    private Credential _credential;

    public MainPage()
    {
        InitializeComponent();

        this.BindingContext = new MapViewModel();
    }

    private async void LoadPortalItemButton_Clicked(object sender, EventArgs e)
    {
        await GetPortalItem();
    }

    private async Task<Credential> SelectCert()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                var passwordPopup = new CertPasswordPopup(result.FileName);

                var passResult = await this.ShowPopupAsync(passwordPopup);

                var cert = new X509Certificate2(result.FullPath, (string)passResult);
                _credential = new CertificateCredential(new Uri(PortalAddressEntry.Text), cert);

                GeneratedCredentialsLabel.Text += cert.FriendlyName;
                GeneratedCredentialsLabel.IsVisible = true;
            }

            return _credential;
        }
        catch(Exception ex) 
        {
            await DisplayAlert("Error", $"Failed to load certificate: {ex.Message}", "OK");
            return null;
        }
    }

    private async Task<Credential> CreateCertCredential(CredentialRequestInfo info)
    {
        try
        {
            if (info.AuthenticationType == AuthenticationType.Certificate)
            {
                if (_credential != null)
                {
                    return _credential;
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await SelectCert();
                    });

                    return _credential;
                }
            }
        }
        catch (Exception ex)
        {
            // do something.
        }

        return null;
    }

    public async Task GetPortalItem()
    {
        try
        {
            AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(CreateCertCredential);

            var portal = await ArcGISPortal.CreateAsync(new Uri(PortalAddressEntry.Text));

            var portalItem = await PortalItem.CreateAsync(portal, PortalItemIdEntry.Text);

            mapView.Map = new Esri.ArcGISRuntime.Mapping.Map(portalItem);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load portal item: {ex.Message}", "OK");
        }
    }
}

