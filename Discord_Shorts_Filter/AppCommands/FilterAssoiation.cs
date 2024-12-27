using Discord_Shorts_Filter.Logging;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using SlashCommandBuilder = Discord.SlashCommandBuilder;

namespace Discord_Shorts_Filter.AppCommands;

public class FilterAssoiation : IAppCommand
{
    public string CommandName { get; } = "filter_assoiation";

    private DiscordSocketClient _client;
    
    private Logger CommandLogger { get; set; } = Logger.GetLogger("FilterAssoiation Logger", LogLevel.Info);   
        
    public FilterAssoiation(DiscordSocketClient client)
    {
        _client = client;
    }

    public FilterAssoiation(DiscordSocketClient client, Logger logger) : this(client)
    {
        CommandLogger = logger;
    }
    
    public async Task AddCommandAsync(ulong guildID)
    {
        SlashCommandBuilder commandBuilder = new SlashCommandBuilder();
        commandBuilder.WithName(CommandName);
        commandBuilder.WithDefaultMemberPermissions(GuildPermission.Administrator);
        commandBuilder.WithDescription("Add, Delete a filter/post channel relationship.");
        
        // Create the add sub-command.
        commandBuilder.AddOption(new SlashCommandOptionBuilder()
            .WithName("add")
            .WithDescription("Add a new assoiation between a filter and post channel.")
            .WithType(ApplicationCommandOptionType.SubCommand)
            .AddOption(
                "filter_channel",
                ApplicationCommandOptionType.Channel,
                "The filter channel from where videos come from.",
                isRequired: true
                )
            .AddOption(
                "post_channel",
                ApplicationCommandOptionType.Channel,
                "The channel to where videos from the filter are posted",
                isRequired: true
                )
        );
        
        //Create the delete sub-command.
        commandBuilder.AddOption(new SlashCommandOptionBuilder()
            .WithName("update")
            .WithDescription("Update an assoiation between a filter and post channel.")
            .WithType(ApplicationCommandOptionType.SubCommand)
            .AddOption(
                "filter_channel",
                ApplicationCommandOptionType.Channel,
                "The filter channel from where videos come from.",
                isRequired: true
                )
            .AddOption(
                "post_channel",
                ApplicationCommandOptionType.Channel,
                "The channel to where videos from the filter are posted",
                isRequired: true
                )
            .AddOption(
                "new_filter_channel",
                ApplicationCommandOptionType.Channel,
                "The new filter channel from where videos come from.",
                isRequired: true
            )
            .AddOption(
                "new_post_channel",
                ApplicationCommandOptionType.Channel,
                "The new channel to where videos from the filter are posted",
                isRequired: true
            )
        );
        
        //Create the update sub-command
        commandBuilder.AddOption(new SlashCommandOptionBuilder()
            .WithName("delete")
            .WithDescription("Delete an assoiation between a filter and post channel.")
            .WithType(ApplicationCommandOptionType.SubCommand)
            .AddOption(
                "filter_channel",
                ApplicationCommandOptionType.Channel,
                "The filter channel from where videos come from.",
                isRequired: true
            )
            .AddOption(
                "post_channel",
                ApplicationCommandOptionType.Channel,
                "The channel to where videos from the filter are posted",
                isRequired: true
            )
        );
        
        try
        {            
            await _client.GetGuild(guildID).CreateApplicationCommandAsync(commandBuilder.Build());
            CommandLogger.Info($"Added {CommandName} to {_client.GetGuild(guildID).Name}.");
        }
        catch (HttpException exception) 
        {
            string response = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            CommandLogger.Error(response);
        }
    }

    public async Task HandleCommandAsync(SocketSlashCommand command)
    {
        if (command.GuildId == null)
        {
            await command.RespondAsync("This command must be used in a server!", ephemeral: true);
        }
        
        string? subCommand = command.Data.Options.FirstOrDefault()?.Name;

        if (subCommand == null)
        {
            await command.RespondAsync("You can not run this command without a sub-command selection.");
        }
        
        CommandLogger.Debug($"Sub-Command: {subCommand} was selected.");

        switch (subCommand)
        {
            case "add":
                await HandleAddCommand(command);
                break;
            case "delete":
                await HandleDeleteCommand(command);
                break;
            case "update":
                await HandleUpdateCommand(command);
                break;
        }
    }
    
    private async Task HandleAddCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("Channel filter ID will now post to ID", ephemeral: true);
    }

    private async Task HandleDeleteCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("The filter ID will no longer post to ID", ephemeral: true);
    }

    private async Task HandleUpdateCommand(SocketSlashCommand command)
    {
        await command.RespondAsync("Updated Filter/Post realtionship. " +
                                   "\nFilter ID will now post to ID", ephemeral: true);
    }
}