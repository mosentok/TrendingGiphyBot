<h1 align="center">
  Trending Giphy Bot
</h1>
<p align="center">
  Stay up to date with the latest trending gifs, right inside Discord.
</p>
<p align="center">
  <span>
    <a href="https://dev.azure.com/mosentok/TrendingGiphyBot/_build?definitionId=1">
      <img src="https://img.shields.io/azure-devops/tests/mosentok/trendinggiphybot/1.svg?label=Unit%20Tests"></img>
    </a>
  </span>
  <span>
    <a href="https://dev.azure.com/mosentok/TrendingGiphyBot/_build?definitionId=1">
      <img src="https://img.shields.io/azure-devops/coverage/mosentok/TrendingGiphyBot/1.svg?label=Code%20Coverage"></img>
    </a>
  </span>
</p>

&nbsp;|&nbsp;
-|-
<img src="https://discordapp.com/assets/fc0b01fe10a0b8c602fb0106d8189d9b.png" width="900px" />|<img src="https://media1.giphy.com/media/3o6gbbuLW76jkt8vIc/giphy.gif" width="900px" />

## Table of Contents

- [What is Trending Giphy Bot?](https://github.com/mosentok/TrendingGiphyBot#what-is-trending-giphy-bot)
- [Invite](https://github.com/mosentok/TrendingGiphyBot#invite)
- [Quick Start](https://github.com/mosentok/TrendingGiphyBot#quick-start)
- [Commands](https://github.com/mosentok/TrendingGiphyBot#commands)
- [Special Thanks](https://github.com/mosentok/TrendingGiphyBot#special-thanks)

## What is Trending Giphy Bot?

Get your gif fix! Surf the meme curve!

Trending Giphy Bot finds trending gifs for you and posts them to your Discord at your own pace. Want all the gifs all the time? Tell it to post every 10 minutes! Got a Discord chat channel with slow conversations? Tell it to post a gif there every 2 hours to spice it up!

Trending Giphy Bot does all the heavy lifting. Tell it how often you want gifs, and it will give them to you that often, easy as that.

## Invite

Invite the bot [here](https://discordapp.com/oauth2/authorize?client_id=333392663061463040&scope=bot)!

## Quick Start

Invite the bot, then go to a channel and type `!trend every 30 minutes`, then type `!trend between 8 and 22`. These two commands set the bot up to post gifs to that channel every 30 minutes, and to do it only between the hours of 8 and 22.

## Commands

### !Trend

All of the bot's commands start with `!trend`. If you go ahead and just type `!trend`, the bot will show you its setup for the channel you just typed in. If the bot's not setup in that channel, it'll give you some examples of the commands you can use to set it up.

### !Trend Every x Minutes

This command tells the bot how often to post in the channel. Use this command like `!trend every 10 minutes` or `!trend every 2 hours`.

There are many acceptable times that you can tell the bot how often to post. You can do 10, 15, 20, or 30 for minutes, and you can do 1, 2, 3, 4, 6, 8, 12, or 24 for hours.

If you go to a channel and type `!trend every 10 minutes`, then every 10 minutes, the bot will post a gif to that channel.

If you go to another channel and type `!trend every 2 hours`, then every 2 hours, the bot will post a gif to that channel, too.

### !Trend Random Cats

If you pick a fast time, like 10 or 15 minutes, there might not be new gifs yet. If you want, you can tell the bot to get a random gif when this happens. Use this command like `!trend random cats` or `!trend random legend of zelda`.

Remember, this option is really only useful for fast times like 10 or 15 minutes. If you set a chill pace like 2 hours, there will almost always be a new trending gif, so the bot won't try to find a random one for you.

For example, at 2:00PM the bot might find a trending gif for you, then another one at 2:10PM, but then maybe that's the last trending gif that's available for now, so then at 2:20PM, the bot won't find any new trending gifs for you. If you told it `!trend random cats`, then in this situation, at 2:20PM it will find you a random cat gif instead.

### !Trend Between 8 and 22

This command tells the bot to only post between certain hours of the day. Use this command like `!trend between 8 and 22`.

This option is here so that you can tell the bot not to post overnight, because posting overnight might send you a bunch of notifications, or fill your channel up so you can't see the last conversation you had yesterday.

The hours are 24 hour format, so you can use 0-23. The timezone is Central Standard Time (CST).

### !Trend Prefix ^

This is an advanced command so that you can customize this bot's prefix, which is defaulted to `!`. Use it like `!trend prefix ^`. That would make `^` the new prefix, so you could then do `^trend every 10 minutes`.

Most likely best to just leave it as `!`.

### !Trend Examples

This command will show you some examples of the bot's commands. Use it as a quick reference if you don't want to come all the way back here.

## Special Thanks

- [.NET](https://dotnet.microsoft.com)
- [Azure](https://azure.microsoft.com/en-us)
- [Azure DevOps](https://azure.microsoft.com/en-us/services/devops/)
- [Visual Studio](https://www.visualstudio.com)
- [Discord](https://discordapp.com)
- [Discord.Net](https://github.com/RogueException/Discord.Net)
- [Giphy](https://giphy.com)
- [Shields.io](https://shields.io/)
