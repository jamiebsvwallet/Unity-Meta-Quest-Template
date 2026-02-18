using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConversationSystem
{
    /// <summary>
    /// Represents a conversation containing multiple messages
    /// </summary>
    [Serializable]
    public class Conversation
    {
        public string id;
        public string title;
        public List<Message> messages;
        
        public Conversation(string title)
        {
            this.id = Guid.NewGuid().ToString();
            this.title = title;
            this.messages = new List<Message>();
        }
        
        /// <summary>
        /// Adds a new message to the conversation
        /// </summary>
        public void AddMessage(string sender, string content)
        {
            messages.Add(new Message(sender, content));
        }
        
        /// <summary>
        /// Gets all messages from yesterday
        /// </summary>
        public List<Message> GetMessagesFromYesterday()
        {
            return messages.Where(m => m.IsFromYesterday()).ToList();
        }
        
        /// <summary>
        /// Gets the most recent message timestamp
        /// </summary>
        public DateTime GetLastMessageTime()
        {
            if (messages.Count == 0)
                return DateTime.MinValue;
            
            return messages.Max(m => m.GetDateTime());
        }
    }
}
