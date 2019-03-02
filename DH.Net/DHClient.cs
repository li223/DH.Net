using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DH.Net
{
    /// <summary>
    /// Main Client Class
    /// </summary>
    public class DHClient : IDisposable
    {
        /// <summary>
        /// Delegate for DHClientErrors
        /// </summary>
        /// <param name="message">The error message if applicable</param>
        /// <returns></returns>
        public delegate Task DHClientError(string message);

        /// <summary>
        /// DHClientError Event
        /// </summary>
        public event DHClientError DHClientErrored;

        private readonly string _baseRequest = "https://discordhub.com/api";
        private string _apiKey { get; set; }
        private HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Main ctor
        /// </summary>
        /// <param name="api_key"></param>
        public DHClient(string api_key)
        {
            this._apiKey = api_key;
            this._httpClient.DefaultRequestHeaders.Add("X-API-KEY", this._apiKey);
        }

        /// <summary>
        /// Exchange points between 2 users
        /// </summary>
        /// <param name="take_from">The ID of the user to remove points from</param>
        /// <param name="give_to">The ID of the user to give points to</param>
        /// <param name="guild_id">The ID of the guild</param>
        /// <param name="amount">The amount of points to exchange</param>
        /// <returns></returns>
        public async Task ExchangePointsAsync(ulong take_from, ulong give_to, ulong guild_id, int amount)
        {
            await RemovePointsAsync(take_from, guild_id, amount).ConfigureAwait(false);
            await GivePointsAsync(give_to, guild_id, amount).ConfigureAwait(false);
        }

        /// <summary>
        /// Gives a set number of points to a user
        /// </summary>
        /// <param name="target_id">The Id of the target user</param>
        /// <param name="guild_id">The Id of the guild the user is in</param>
        /// <param name="amount">The amount of points to give to the user</param>
        /// <returns>The new balance of the user on success, or null if unsuccessful</returns>
        public async Task<ulong?> GivePointsAsync(ulong target_id, ulong guild_id, int amount)
        {
            var res = await _httpClient.PostAsync(new Uri($"{_baseRequest}/points/add?user_id={target_id}&server_id={guild_id}&amount={amount}"), null).ConfigureAwait(false);
            var data = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (res.IsSuccessStatusCode) return (ulong)JObject.Parse(data).SelectToken("points");
            else
            {
                this.DHClientErrored?.Invoke(JObject.Parse(data).SelectToken("message").ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets the current points balance of the user
        /// </summary>
        /// <param name="target_id">The Id of the target user</param>
        /// <param name="guild_id">The Id of the guild the user is in</param>
        /// <returns>The balance of the user on success, or null if unsuccessful</returns>
        public async Task<ulong?> GetBalanceAsync(ulong target_id, ulong guild_id)
        {
            var res = await _httpClient.GetAsync(new Uri($"{_baseRequest}/points/balance?user_id={target_id}&server_id={guild_id}")).ConfigureAwait(false);
            var data = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (res.IsSuccessStatusCode) return (ulong)JObject.Parse(data).SelectToken("points");
            else
            {
                this.DHClientErrored?.Invoke(JObject.Parse(data).SelectToken("message").ToString());
                return null;
            }
        }

        /// <summary>
        /// Removes a set number of points from a user
        /// </summary>
        /// <param name="target_id">The Id of the target user</param>
        /// <param name="guild_id">The Id of the guild the user is in</param>
        /// <param name="amount">The amount of points to give to the user</param>
        /// <returns>The new balance of the user on success, or null if unsuccessful</returns>
        public async Task<ulong?> RemovePointsAsync(ulong target_id, ulong guild_id, int amount)
        {
            var res = await _httpClient.PostAsync(new Uri($"{_baseRequest}/points/remove?user_id={target_id}&server_id={guild_id}&amount={amount}"), null).ConfigureAwait(false);
            var data = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (res.IsSuccessStatusCode) return (ulong)JObject.Parse(data).SelectToken("points");
            else
            {
                this.DHClientErrored?.Invoke(JObject.Parse(data).SelectToken("message").ToString());
                return null;
            }
        }

        /// <summary>
        /// Gives an item to a user from the shop
        /// </summary>
        /// <param name="target_id">The Id of the target user</param>
        /// <param name="guild_id">The Id of the guild the user is in</param>
        /// <param name="item_id">ID of the Item. Please note this is not the Discord role ID, but the item ID generated by DiscordHub.</param>
        /// <returns>Returns true on success</returns>
        public async Task<bool> GiveItemAsync(ulong target_id, ulong guild_id, ulong item_id)
        {
            var res = await _httpClient.PostAsync(new Uri($"{_baseRequest}/item/give?user_id={target_id}&server_id={guild_id}&item_id={item_id}"), null).ConfigureAwait(false);
            var data = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (res.IsSuccessStatusCode)
            {
                if (JObject.Parse(data).SelectToken("message").ToString().StartsWith("Successfully")) return true;
                else return false;
            }
            else
            {
                this.DHClientErrored?.Invoke(JObject.Parse(data).SelectToken("message").ToString());
                return false;
            }
        }

        /// <summary>
        /// Gives a set number of exp to a user
        /// </summary>
        /// <param name="target_id">The Id of the target user</param>
        /// <param name="guild_id">The Id of the guild the user is in</param>
        /// <param name="amount">The amount of exp to give to the user</param>
        /// <returns>The new exp amount of the user on success, or null if unsuccessful</returns>
        public async Task<ulong?> GiveExpAsync(ulong target_id, ulong guild_id, int amount)
        {
            var res = await _httpClient.PostAsync(new Uri($"{_baseRequest}/ranking/exp/add?user_id={target_id}&server_id={guild_id}&amount={amount}"), null).ConfigureAwait(false);
            var data = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (res.IsSuccessStatusCode) return (ulong)JObject.Parse(data).SelectToken("exp");
            else
            {
                this.DHClientErrored?.Invoke(JObject.Parse(data).SelectToken("message").ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets the current exp and level of the user
        /// </summary>
        /// <param name="target_id">The Id of the target user</param>
        /// <param name="guild_id">The Id of the guild the user is in</param>
        /// <returns>The user exp info on success, or null if unsuccessful</returns>
        public async Task<UserExpLevel?> GetExpInfoAsync(ulong target_id, ulong guild_id)
        {
            var res = await _httpClient.GetAsync(new Uri($"{_baseRequest}/ranking/user/info?user_id={target_id}&server_id={guild_id}")).ConfigureAwait(false);
            var cont = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (res.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<UserExpLevel>(cont);
                return data;
            }
            else
            {
                this.DHClientErrored?.Invoke(JObject.Parse(cont).SelectToken("message").ToString());
                return null;
            }
        }

        /// <summary>
        /// Removes a set number of exp from a user
        /// </summary>
        /// <param name="target_id">The Id of the target user</param>
        /// <param name="guild_id">The Id of the guild the user is in</param>
        /// <param name="amount">The amount of exp to give to the user</param>
        /// <returns>The new exp amount of the user on success, or null if unsuccessful</returns>
        public async Task<ulong?> RemoveExpAsync(ulong target_id, ulong guild_id, int amount)
        {
            var res = await _httpClient.PostAsync(new Uri($"{_baseRequest}/ranking/exp/remove?user_id={target_id}&server_id={guild_id}&amount={amount}"), null).ConfigureAwait(false);
            var data = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (res.IsSuccessStatusCode) return (ulong)JObject.Parse(data).SelectToken("exp");
            else
            {
                this.DHClientErrored?.Invoke(JObject.Parse(data).SelectToken("message").ToString());
                return null;
            }
        }

        /// <summary>
        /// Idunno
        /// </summary>
        /// <param name="guild_id">Id of the target guild</param>
        /// <returns>Idunno</returns>
        [Obsolete("This endpoulong seems non-functional and only returns an error response")]
        public async Task<object> GetAllUsersAsync(ulong guild_id)
        {
            var res = await _httpClient.PostAsync(new Uri($"{_baseRequest}/ranking/user/all/ids?&server_id={guild_id}"), null).ConfigureAwait(false);
            var data = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (res.IsSuccessStatusCode) return data;
            else
            {
                this.DHClientErrored?.Invoke(JObject.Parse(data).SelectToken("message").ToString());
                return null;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() => GC.SuppressFinalize(this);
    }
}
