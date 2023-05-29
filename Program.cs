
using Babel.Licensing;

// Create a new configuration object
BabelLicensingConfiguration config = new BabelLicensingConfiguration() {
    // Set the service URL
    ServiceUrl = "http://localhost:5005",

    // Set the public key used to verify the license signature
    SignatureProvider = RSASignature.FromKeys("MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDE1VRiIdr6fiVZKve7NVgjIvGdRiRx0Mjjm+Yzf6tLbzFnxLs0fat5EoRcubxx0QQQDfydsJBE/fc7cwRWSrE2xK6X4Eb4W8O47pCMjqvTQZfDqQywEZJrLlxpp9hlKz6FDYX4SagrjmP1gdw8olo+n+IBz8ubkNxRhvycikxuDQIDAQAB")
};

// Create the client object used to communicate with the server
BabelLicensing client = new BabelLicensing(config);

// Use BabelServiceLicenseProvider to cache a local copy of the license for offline use
BabelServiceLicenseProvider provider = new BabelServiceLicenseProvider(client) {
    // Refresh the license contacting the server every 5 days
    // If set to TimeSpan.Zero, the license will be validated every time on the server
    LicenseRefreshInterval = TimeSpan.FromDays(5)
};

// Register the license provider with BabelLicenseManager
BabelLicenseManager.RegisterLicenseProvider(typeof(Program), provider);

// Check if the user key was set
if (provider.UserKey == null)
{
    // If the user key was not entered, the license has not been activated yet.

    // Ask the user to activate the license
    Console.WriteLine("Please enter your activation license key: ");
    string? userKey = Console.ReadLine();

    try
    {
        await client.ActivateLicenseAsync(userKey, typeof(Program));

        Console.WriteLine("License activated.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not activate license: {ex.Message}");
        return;
    }
}

try
{
    // Validate the license
    // This will contact the server to validate the license if the local copy is expired
    // You can customize the validation interval using the LicenseValidationInterval property
    var license = await BabelLicenseManager.ValidateAsync(typeof(Program));

    Console.WriteLine($"License {license.Id} valid.");

    // Ask to deactiva the license
    Console.WriteLine("Do you want to deactivate the license? (y/n)");
    string? answer = Console.ReadLine();

    if (answer?.ToLower() == "y")
    {
        try
        {
            await client.DeactivateLicenseAsync(provider.UserKey);
            Console.WriteLine("License deactivated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not deactivate license: {ex.Message}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"License not valid: {ex.Message}");
    return;
}


