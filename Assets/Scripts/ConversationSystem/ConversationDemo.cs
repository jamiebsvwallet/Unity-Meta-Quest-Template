using System;
using UnityEngine;

namespace ConversationSystem
{
    /// <summary>
    /// Demo script to populate conversation system with sample data for testing
    /// </summary>
    public class ConversationDemo : MonoBehaviour
    {
        [Header("Demo Settings")]
        [SerializeField] private bool populateOnStart = true;
        [SerializeField] private bool clearExistingData = false;
        
        void Start()
        {
            if (populateOnStart)
            {
                PopulateSampleData();
            }
        }
        
        /// <summary>
        /// Populates the conversation system with sample data including messages from different days
        /// </summary>
        public void PopulateSampleData()
        {
            if (clearExistingData)
            {
                ConversationManager.Instance.ClearAllConversations();
                Debug.Log("Cleared existing conversation data");
            }
            
            // Create conversations with messages from different times
            CreateSampleConversation1();
            CreateSampleConversation2();
            CreateSampleConversation3();
            
            Debug.Log("Sample conversation data populated successfully");
        }
        
        private void CreateSampleConversation1()
        {
            Conversation conv = ConversationManager.Instance.CreateConversation("VR Meeting with Team");
            
            // Add messages from today
            conv.AddMessage("Alice", "Good morning everyone!");
            conv.AddMessage("Bob", "Hi Alice, ready for today's demo?");
            
            // Add messages from yesterday by manually creating them with modified timestamps
            AddHistoricalMessage(conv, "Alice", "Did you see the new XR features?", -1);
            AddHistoricalMessage(conv, "Bob", "Yes! The hand tracking is amazing!", -1);
            AddHistoricalMessage(conv, "Charlie", "We should test it with Quest 3", -1);
        }
        
        private void CreateSampleConversation2()
        {
            Conversation conv = ConversationManager.Instance.CreateConversation("Project Discussion");
            
            // Messages from 2 days ago
            AddHistoricalMessage(conv, "Alice", "Let's plan the next sprint", -2);
            AddHistoricalMessage(conv, "Bob", "Sounds good. What's the priority?", -2);
            
            // Messages from yesterday
            AddHistoricalMessage(conv, "Alice", "I think we should focus on UI improvements", -1);
            AddHistoricalMessage(conv, "Charlie", "Agreed. The conversation history feature is important", -1);
            
            // Message from today
            conv.AddMessage("Bob", "I'll start working on it today");
        }
        
        private void CreateSampleConversation3()
        {
            Conversation conv = ConversationManager.Instance.CreateConversation("Daily Standup");
            
            // Yesterday's standup
            AddHistoricalMessage(conv, "Alice", "Yesterday I worked on the VR controls", -1);
            AddHistoricalMessage(conv, "Bob", "I fixed the performance issues", -1);
            AddHistoricalMessage(conv, "Charlie", "I updated the documentation", -1);
            
            // Today's standup
            conv.AddMessage("Alice", "Today I'll implement the conversation filter");
            conv.AddMessage("Bob", "I'll review the pull requests");
            conv.AddMessage("Charlie", "I'll test the new features");
        }
        
        /// <summary>
        /// Adds a message with a historical timestamp
        /// </summary>
        private void AddHistoricalMessage(Conversation conversation, string sender, string content, int daysAgo)
        {
            DateTime historicalTime = DateTime.UtcNow.AddDays(daysAgo);
            Message message = new Message(sender, content, historicalTime);
            conversation.messages.Add(message);
        }
        
        /// <summary>
        /// Clears all conversation data
        /// </summary>
        public void ClearAllData()
        {
            ConversationManager.Instance.ClearAllConversations();
            Debug.Log("All conversation data cleared");
        }
    }
}
