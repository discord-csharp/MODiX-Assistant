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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModixAssistant
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            client.Log += LogHelper.Log;

            //AI dependency injection
            var type = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(d => typeof(IBot).IsAssignableFrom(d));

            IBot bot = Activator.CreateInstance(type, client) as IBot;
            client.Ready += bot.Ready;

            await client.StartAsync();
            await client.LoginAsync(TokenType.Bot, await File.ReadAllTextAsync("token.txt"));

            await Task.Delay(-1);
        }
    }
}
