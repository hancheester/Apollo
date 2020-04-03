using Apollo.Core.Infrastructure;
using Apollo.Core.Services.Interfaces;
using Apollo.Web.Framework;
using Apollo.Web.Framework.Services.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Twitter;
using Owin;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Apollo.FrontStore
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieName = SystemCookieNames.AuthenticationCookie,
                LoginPath = new PathString("/login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = ApplicationSecurityStampValidator.OnValidateIdentity(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, userId) => Task.FromResult(manager.CreateIdentity(userId)),
                        manager: EngineContext.Current.Resolve<IAccountService>())
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["google:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["google:ClientSecret"],
                Caption = "Google",
            });

            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");
            
            app.UseTwitterAuthentication(new TwitterAuthenticationOptions()
            {
                ConsumerKey = ConfigurationManager.AppSettings["twitter:ConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["twitter:ConsumerSecret"],
                Provider = new TwitterAuthenticationProvider
                {
                    OnAuthenticated = (context) =>
                    {
                        context.Identity.AddClaim(new Claim("urn:twitter:access_token", context.AccessToken));
                        context.Identity.AddClaim(new Claim("urn:twitter:access_secret", context.AccessTokenSecret));
                        return Task.FromResult(0);
                    }
                },
                BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(new[]
                {
                    "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                    "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                    "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                    "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                    "‎add53f6680fe66e383cbac3e60922e3b4c412bed", // Symantec Class 3 EV SSL CA - G3
                    "4eb6d578499b1ccf5f581ead56be3d9b6744a5e5", // VeriSign Class 3 Primary CA - G5
                    "5168FF90AF0207753CCCD9656462A212B859723B", // DigiCert SHA2 High Assurance Server C‎A 
                    "B13EC36903F8BF4701D498261A0802EF63642BC3" // DigiCert High Assurance EV Root CA
                }),
            });

            app.UseFacebookAuthentication(
               appId: ConfigurationManager.AppSettings["facebook:AppId"],
               appSecret: ConfigurationManager.AppSettings["facebook:AppSecret"]);

        }
    }
}