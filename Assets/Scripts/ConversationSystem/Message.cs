using System;
using UnityEngine;

namespace ConversationSystem
{
    /// <summary>
    /// Represents a single message in a conversation
    /// </summary>
    [Serializable]
    public class Message
    {
        public string id;
        public string sender;
        public string content;
        public string timestamp; // ISO 8601 format
        
        public Message(string sender, string content)
        {
            this.id = Guid.NewGuid().ToString();
            this.sender = sender;
            this.content = content;
            this.timestamp = DateTime.UtcNow.ToString("o");
        }
        
        /// <summary>
        /// Gets the DateTime of when this message was sent
        /// </summary>
        public DateTime GetDateTime()
        {
            return DateTime.Parse(timestamp);
        }
        
        /// <summary>
        /// Checks if this message was sent yesterday
        /// </summary>
        public bool IsFromYesterday()
        {
            DateTime messageDate = GetDateTime().Date;
            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);
            return messageDate == yesterday;
        }
    }
}
