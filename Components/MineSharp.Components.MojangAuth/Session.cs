using MineSharp.Components.Core.Logging;
using MineSharp.Components.Core.Types;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MineSharp.Components.MojangAuth
{
    public class Session
    {
        private static readonly Logger Logger = Logger.GetLogger();
        
        public string Username { get; }
        public UUID Uuid { get; }
        public string ClientToken { get; }
        public string SessionToken { get; }
        public bool OnlineSession { get; }

        public Session(string username, UUID uuid, string clientToken, string sessionToken, bool isOnline)
        {
            this.Username = username;
            this.Uuid = uuid;
            this.ClientToken = clientToken;
            this.SessionToken = sessionToken;
            this.OnlineSession = isOnline;
        }

        public static Session OfflineSession(string username) => new Session(username, Guid.NewGuid(), "", "", false);
        public static Session OfflineSession(string username, UUID uuid) => new Session(username, uuid, "", "", false);
        public static Session TokenSession(string username, UUID uuid, string clientToken, string sessionToken) => new Session(username, uuid, clientToken, sessionToken, true);
        public static async Task<Session> MicrosoftSession(MicrosoftAuth.DeviceCodeHandler handler) => await MicrosoftAuth.MicrosoftLogin(handler);

        public async Task<bool> JoinServer(string serverHash)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            HttpContent body = new StringContent(JsonConvert.SerializeObject(new JoinServerBlob {
                ServerId = serverHash,
                AccessToken = this.SessionToken,
                SelectedProfile = this.Uuid.ToString().Replace("-", "").ToLower()
            }));

            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri("https://sessionserver.mojang.com/session/minecraft/join"), body);

            if (response.IsSuccessStatusCode) return true;
            Logger.Error("Session server returned error code: " + response.StatusCode + "  " + await response.Content.ReadAsStringAsync());
            return false;

        }

        private struct JoinServerBlob
        {
            [JsonProperty("accessToken")]
            public string AccessToken;

            [JsonProperty("selectedProfile")]
            public string SelectedProfile;

            [JsonProperty("serverId")]
            public string ServerId;
        }
    }
}
