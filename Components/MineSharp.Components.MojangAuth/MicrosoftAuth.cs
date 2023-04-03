using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth.Microsoft.MsalClient;
using Microsoft.Identity.Client;
using MineSharp.Components.Core.Types;

namespace MineSharp.Components.MojangAuth
{
    public static class MicrosoftAuth
    {
        private static string ClientID = "389b1b32-b5d5-43b2-bddc-84ce938d6737";

        public delegate void DeviceCodeHandler(DeviceCodeResult deviceCode);
        
        /// <summary>
        /// Logins using a Microsoft Account
        /// </summary>
        /// <param name="handler">
        /// When the user has to login in the browser, handler() is called. It should open up a browser window and show the user the deviceCode.UserCode
        /// If none is provided, the link will open up in the default browser and the device code is written to the console
        /// </param>
        /// <returns>A Session instance</returns>
        public static async Task<Session> MicrosoftLogin(DeviceCodeHandler handler)
        {
            var app = await MsalMinecraftLoginHelper.BuildApplicationWithCache(ClientID);
            var loginHandler = new LoginHandlerBuilder()
                .ForJavaEdition()
                .WithMsalOAuth(app, factory => factory.CreateDeviceCodeApi(result =>
                {
                    handler(result);
                    return Task.CompletedTask;
                }))
                .Build();

            var result = await loginHandler.LoginFromOAuth();
            var mSession = result.GameSession;

            return new Session(
                mSession.Username!,
                UUID.Parse(mSession.UUID!),
                mSession.ClientToken!,
                mSession.AccessToken!,
                true);
        }
    }
}
