# FamilyShow ClickOnce Certificate Information

## Certificate Details

**Created:** January 26, 2026
**Expires:** January 26, 2029 (3 years)
**Type:** Self-Signed Code Signing Certificate
**Subject:** CN=FamilyShow Publisher
**Thumbprint:** B823468A623866F0EE5BADFDACB2579194FA6C9D

## Certificate Files

- **PFX File:** `FamilyShow_TemporaryKey.pfx`
- **Password:** `FamilyShow2025`
- **Location:** `D:\Code\github\FamilyShow\FamilyShow\`

## Certificate Storage

The certificate is installed in the following Windows certificate stores:
- **Personal Store:** `Cert:\CurrentUser\My`
- **Trusted Publishers:** `Cert:\CurrentUser\TrustedPublisher`

## Project Configuration

The FamilyShow.csproj file has been updated with:
```xml
<SignManifests>true</SignManifests>
<ManifestCertificateThumbprint>B823468A623866F0EE5BADFDACB2579194FA6C9D</ManifestCertificateThumbprint>
<ManifestKeyFile>FamilyShow_TemporaryKey.pfx</ManifestKeyFile>
```

## ClickOnce Deployment

The project is now ready for ClickOnce deployment. To publish:

1. Right-click on the FamilyShow project in Visual Studio
2. Select "Publish..."
3. Follow the Publish Wizard to configure deployment location and settings
4. The application will be automatically signed with the certificate

## For Other Developers

If other developers need to build this project, they should either:

### Option 1: Disable Signing (for local development)
Change in FamilyShow.csproj:
```xml
<SignManifests>false</SignManifests>
```

### Option 2: Install the Certificate
1. Import `FamilyShow_TemporaryKey.pfx` into their certificate store
2. Use password: `FamilyShow2025`
3. Install to both "Personal" and "Trusted Publishers" stores

### Option 3: Create Their Own Certificate
Run in PowerShell:
```powershell
$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=FamilyShow Publisher" `
    -KeyUsage DigitalSignature -FriendlyName "FamilyShow Code Signing Certificate" `
    -CertStoreLocation "Cert:\CurrentUser\My" -NotAfter (Get-Date).AddYears(3)
    
$pwd = ConvertTo-SecureString -String "YourPassword" -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath "FamilyShow_TemporaryKey.pfx" -Password $pwd

# Update the thumbprint in FamilyShow.csproj with $cert.Thumbprint
```

## Security Notes

?? **Important:** This is a self-signed certificate suitable for:
- Internal distribution
- Development and testing
- Personal use

For public distribution, consider obtaining a certificate from a trusted Certificate Authority (CA) such as:
- DigiCert
- Sectigo
- GlobalSign

## Backup

**Important:** Keep a backup of the PFX file in a secure location. If lost, you won't be able to:
- Sign updates to already-deployed applications
- Maintain trust chain for existing installations

Backup Location Recommendation:
- Secure password manager
- Encrypted USB drive
- Secure cloud storage (encrypted)

---
*Generated: January 26, 2026*
