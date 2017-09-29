namespace BreadPlayer.Messengers
{
    public class Message
    {
        #region Public Properties

        /// <summary>
        /// Has the message been handled
        /// </summary>
        public MessageHandledStatus HandledStatus
        {
            get;
            set;
        }

        /// <summary>
        /// What type of message is this
        /// </summary>
        private MessageTypes _messageType;

        public MessageTypes MessageType => _messageType;

        /// <summary>
        /// The payload for the message
        /// </summary>
        public object Payload
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Constructor

        public Message(MessageTypes messageType)
        {
            _messageType = messageType;
        }

        #endregion Constructor
    }
}