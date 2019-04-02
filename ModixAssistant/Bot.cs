/*

ooo        ooooo   .oooooo.   oooooooooo.    o8o  ooooooo  ooooo 
`88.       .888'  d8P'  `Y8b  `888'   `Y8b   `"'   `8888    d8'  
 888b     d'888  888      888  888      888 oooo     Y888..8P    
 8 Y88. .P  888  888      888  888      888 `888      `8888'     
 8  `888'   888  888      888  888      888  888     .8PY888.    
 8    Y     888  `88b    d88'  888     d88'  888    d8'  `888b   
o8o        o888o  `Y8bood8P'  o888bood8P'   o888o o888o  o88888o 

*/

using Discord;
using Discord.WebSocket;
using Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModixAssistant
{
    public class Bot : IBot
    {
        public const ulong LimitedToChannel = 562165429661990923;
        public const ulong LimitedToGuild = 143867839282020352;

        private MarkovChain<string> _chain;
        private Random _rand = new Random();
        private DiscordSocketClient _client;

        public Bot(DiscordSocketClient client)
        {
            _client = client;
        }

        public Task Ready()
        {
            _client.MessageReceived += HandleMessage;

            var channels = _client.GetGuild(LimitedToGuild).Channels
                .Where(IsPublic)
                .OfType<IMessageChannel>();

            string channelList = string.Join(", ", channels.Select(d => $"#{d.Name}"));
            Console.WriteLine($"Will be loading from channels: {channelList}");

            _ = Task.Run(async () =>
            {
                _chain = await GenerateChainFromChannels(channels, 750);
            });

            _client.Ready -= Ready;
            return Task.CompletedTask;
        }

        async Task<MarkovChain<string>> GenerateChainFromChannels(IEnumerable<IMessageChannel> channels, int messagesFromEach)
        {
            var chain = new MarkovChain<string>(1);
            int totalUsed = 0;

            foreach (var channel in channels)
            {
                Console.WriteLine($"Loading {messagesFromEach} messages from #{channel.Name}");

                int channelWeight = 2019 - channel.CreatedAt.Year;

                var messages =
                    (await channel.GetMessagesAsync(messagesFromEach).FlattenAsync())
                    .Where(m => string.IsNullOrWhiteSpace(m.Content) == false);

                int used = 0;
                foreach (var msg in messages)
                {
                    var parts = msg.Content
                        .Split(' ')
                        .Where(d => !d.Contains('<'));

                    chain.Add(parts, channelWeight);

                    used++;
                }

                Console.WriteLine($"Added {used} messages from #{channel.Name} to the corpus, at weight {channelWeight}");
                totalUsed += used;
            }

            Console.WriteLine($"Done loading corpus. Total corpus size: {totalUsed} messages.");

            return chain;
        }

        async Task HandleMessage(SocketMessage msg)
        {
            if (_chain == null) { return; }
            if (msg.Author.Id == _client.CurrentUser.Id) { return; }
            if (msg.Channel.Id != LimitedToChannel) { return; }
            if (string.IsNullOrWhiteSpace(msg.Content)) { return; }

            Console.WriteLine($"{msg.Author.Username}: {msg.Content}");

            var sentence = string.Join(" ", _chain.Chain(_rand));
            await msg.Channel.SendMessageAsync(sentence);
        }

        bool IsPublic(IGuildChannel channel)
        {
            var permissions = channel.GetPermissionOverwrite(_client.GetGuild(LimitedToGuild).EveryoneRole);
            return !permissions.HasValue ||
                (permissions.Value.ViewChannel != PermValue.Deny && permissions.Value.SendMessages != PermValue.Deny);
        }
    }
}
