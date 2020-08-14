# StreamMultiChat

## Idea
This application is designed to allow stream Broadcasters, Moderators and others to have multiple stream chats open in one cohesive stream of information allowing them to be more productive.

##### Feature List:
  * All messages in a single view including whispers. 
  * Allows for the user to select a single channel to respond or all channels.
  * Has a macro system allowing for quick responses and pre-determind messages.
  * Allows Broadcasters and Moderators the standard tools such as:
    * Delete message
    * Timeout
    * Ban/Unban

 The app is a still a Work In Progress and will be gaining more features as time moves on. If you wish to request a feature or report a bug please use the Isse Tracker on the [github repository page.](https://github.com/CrypticEngima/StreamMultiChat/issues)
	
##### Current Required Scopes for twtich API
  *	chat:read
  *	chat:edit
  *	channel:moderate
  *	whispers:read
  *	user:read:email

## Setup
To run this application you need to have registered an application on the Twitch Dev Portal. The ClientID and ClientSecret are required in the appsettings.json file.


##### App Registration Process on Twitch Dev Portal
Full instructions are [here.](https://dev.twitch.tv/docs/authentication/#registration)

To make an application that uses the Twitch API, you first need to [register your application on the Twitch developer site](https://dev.twitch.tv/dashboard/apps/create). 

When creating this app, enter your redirect URI, which is where your users are redirected after being authorized.For this application the callback URL will be the index page (the root of your site).

Once you create a developer application, you are assigned a client ID. Please generate the client secret, which you can generate on the same page as the client ID. The client secreet will only be shown once so please take a copy and keep it somewhere safe.




## Usage
Login to twitch using button in top right corner of screen on the main page. Type in the channels you wish to connect to as comma seperated values and then click chat button.
Once in the chat page use the select box to set the channel you wish to chat in.