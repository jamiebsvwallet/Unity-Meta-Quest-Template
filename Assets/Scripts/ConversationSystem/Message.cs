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
        /// Constructor with custom timestamp for historical messages
        /// </summary>
        public Message(string sender, string content, DateTime customTimestamp)
        {
            this.id = Guid.NewGuid().ToString();
            this.sender = sender;
            this.content = content;
            this.timestamp = customTimestamp.ToString("o");
        }
        
        /// <summary>
        /// Gets the DateTime of when this message was sent
        /// </summary>
        public DateTime GetDateTime()
        {
            if (DateTime.TryParse(timestamp, out DateTime result))
            {
                return result;
            }
            
            Debug.LogWarning($"Failed to parse timestamp '{timestamp}' for message {id}. Using current time.");
            return DateTime.UtcNow;
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
