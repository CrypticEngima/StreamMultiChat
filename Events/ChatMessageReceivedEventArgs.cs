namespace StreamMultiChat.Blazor.Events
{
	public class ChatMessageReceivedEventArgs
	{
		public ChatMessageReceivedEventArgs(ChatMessage chatMessage)
		{
			ChatMessage = chatMessage;
		}

		public ChatMessage ChatMessage { get; }
	}
}