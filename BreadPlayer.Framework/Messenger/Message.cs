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
        private MessageTypes messageType;
        public MessageTypes MessageType
        {
            get
            {
                return messageType;
            }
        }
        /// <summary>
        /// The payload for the message 
        /// </summary>
        public object Payload
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Message(MessageTypes messageType)
        {
            this.messageType = messageType;
        }
        #endregion
    }
}
