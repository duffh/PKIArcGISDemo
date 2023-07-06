using CommunityToolkit.Maui.Views;

namespace PKIArcGISDemo;

public partial class CertPasswordPopup : Popup
{
	public CertPasswordPopup(string fileName)
	{
		InitializeComponent();
        Initialize(fileName);
	}

    private void Initialize(string fileName)
    {
        PasswordEntryLabel.Text += fileName;
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