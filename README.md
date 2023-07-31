#### Workflow:

1. User enters a `Portal` address and `PortalItem` ID for retrieval
2. User clicks button to load the portal item
3. A `ChallengeHandler` is configured to return a `Credential` object when creating the `Portal` instance.
	3a. If a `Credential` has already been generated then return this `Credential`
3b. If a `Credential` has not been generated then prompt the user to select a .pfx file using the MAUI `FilePicker`. A popup password entry is then presented to create an `X509Certificate2` object. The certificate and the portal address are then used to generate a `CertificateCredential` for the `Portal` instance.
4. Once the `Portal` instance has been generated a `PortalItem` can be retrieved and added to the map.

#### Credential creation process:

```
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {
                var passwordPopup = new CertPasswordPopup(result.FileName);

                var passwordResult = await this.ShowPopupAsync(passwordPopup);

                using var stream = await result.OpenReadAsync();
                var buffer = new byte[stream.Length];
                stream.Read(buffer);

                var cert = new X509Certificate2(buffer, (string)passwordResult);

                _credential = new CertificateCredential(new Uri(PortalAddressEntry.Text), cert);
              }
```
#### Important:
In order for this workflow to work on iOS, MacCatalyst and Android the following code snippet must be included in `MauiProgram.cs`:

```
.UseArcGISRuntime(config =>
                config.ConfigureHttp(http =>
                    http.UseHttpMessageHandler<SocketsHttpHandler>()
                )
            );
```  
On Android and iOS devices the appropriate root certificates must be installed to access Esri services.

#### Notes for iOS + MacCatalyst:
Unfortunately at this time it is not possible to retrieve existing credentials in an iOS keychain with .NET MAUI. As a result the current working method is to use `FilePicker` to select the .pfx file and generate a certificate then a credential for a given `Portal` address. 

#### Notes for Android:
A `network_security_config.xml` file must be added to Platforms\Android\Resources\xml with the following content:

```
<?xml version="1.0" encoding="UTF-8" ?>
<network-security-config>
  <base-config cleartextTrafficPermitted="true">
    <trust-anchors>
      <certificates src="system" />
      <certificates src="user" />
    </trust-anchors>
  </base-config>
</network-security-config>
```
And the following line must be included in the `AndroidManifest.xml file`:
```
<application android:networkSecurityConfig="@xml/network_security_config" />
```

More information on this can be found in the Android Developer guide on [Network Security Configuration](https://developer.android.com/training/articles/security-config).
