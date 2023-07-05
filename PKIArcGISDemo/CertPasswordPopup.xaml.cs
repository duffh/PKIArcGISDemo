using CommunityToolkit.Maui.Views;

namespace PKIArcGISDemo;

public partial class CertPasswordPopup : Popup
{
	public CertPasswordPopup()
	{
		InitializeComponent();
	}

    private void OkButton_Clicked(object sender, EventArgs e)
    {
        this.Close(PasswordEntry.Text);
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        this.Close();
    }
}