namespace StreamMultiChat.Blazor.Events
{
	public class WhisperReceivedEventArgs
	{
		public WhisperReceivedEventArgs(WhisperMessage chatMessage)
		{
			WhisperMessage = chatMessage;
		}

		public WhisperMessage WhisperMessage { get; }
	}
}