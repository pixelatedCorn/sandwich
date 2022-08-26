# Build Instructions
Clone the repo, open in visual studio 2022, and build.
To build for deployment, run `dotnet publish -f net6.0-windows10.0.19041.0 -c Release`
The installation package will be in the AppPackages directory in the Release directory
# Installation Instructions
## Get the App
You can either build the app for deployment or download the latest version from the releases tab
## Trust the Package Certificate
Right click on the *.msix* file and choose **Properties**
Select the **Digital Signatures** tab.
Choose the certificate then press **Details**
Select **View Certificate**
Select **Install Certificate**
Choose **Local Machine** then select **Next**.
*If you're prompted by User Account Control to __Do you want to allow this app to make changes to your device?__, select __Yes__*.
In the **Certificate Import Wizard** window, select **Place all certificates in the following store**.
Select **Browse..** and then choose the **Trusted People** store. Select **OK** to close the dialog.
Select **Next** and then **Finish**. You should see a dialog that says: **The import was successful**.
Select **OK** on any window opened as part of this process to close them all.
## Run the Installer
\*note\*: You MUST trust the certificate before you install the app.
Run the .msix file to install the app