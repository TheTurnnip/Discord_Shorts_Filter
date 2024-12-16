using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Shorts_Filter.tools
{
    /// <summary>
    /// A Static class with methods that are used to update the current state of the bot.
    /// </summary>
    internal static class UpdateGuild
    {
        /// <summary>
        /// Get an updated guild object that contains any changes that have been made since the guild object
        /// was created.
        /// </summary>
        /// <param name="client">The client(bot) that will be updated.</param>
        /// <param name="guildID">The ID of the guild that needs to be updated.</param>
        /// <returns>A new Socket Guild that has the most recent data.</returns>
        internal static async Task<SocketGuild >GetUpdatedGuild(DiscordSocketClient client, ulong guildID)
        {
            RestGuild guild = await client.Rest.GetGuildAsync(guildID);
            return client.GetGuild(guild.Id);
        }
    }
}
