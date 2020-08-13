using System;

namespace StreamMultiChat.Blazor.Events
{
	public class WhisperMessage
	{
		public WhisperMessage(string message, string userId, string username, string displayName)
		{
			Message = message;
			UserId = userId;
			Username = username;
			DisplayName = displayName;
		}

		public string Message { get; }
		public string UserId { get; }
		public string Username { get; }
		public string DisplayName { get; }

		public override string ToString()
		{
			return $"{DateTime.Now:t}   [Whisper] ({Username}) : {Message}";
		}
	}
	
}