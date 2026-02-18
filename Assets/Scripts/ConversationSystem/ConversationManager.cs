using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ConversationSystem
{
    /// <summary>
    /// Manages conversations, including saving, loading, and filtering
    /// </summary>
    public class ConversationManager : MonoBehaviour
    {
        private static ConversationManager instance;
        public static ConversationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("ConversationManager");
                    instance = go.AddComponent<ConversationManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
        
        private List<Conversation> conversations = new List<Conversation>();
        private string saveFilePath;
        
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "conversations.json");
            LoadConversations();
        }
        
        /// <summary>
        /// Creates a new conversation
        /// </summary>
        public Conversation CreateConversation(string title)
        {
            Conversation conversation = new Conversation(title);
            conversations.Add(conversation);
            SaveConversations();
            return conversation;
        }
        
        /// <summary>
        /// Adds a message to an existing conversation
        /// </summary>
        public void AddMessageToConversation(string conversationId, string sender, string content)
        {
            Conversation conversation = conversations.FirstOrDefault(c => c.id == conversationId);
            if (conversation != null)
            {
                conversation.AddMessage(sender, content);
                SaveConversations();
            }
            else
            {
                Debug.LogWarning($"Conversation with id {conversationId} not found");
            }
        }
        
        /// <summary>
        /// Gets all conversations
        /// </summary>
        public List<Conversation> GetAllConversations()
        {
            return new List<Conversation>(conversations);
        }
        
        /// <summary>
        /// Gets all conversations that have messages from yesterday
        /// </summary>
        public List<Conversation> GetConversationsFromYesterday()
        {
            return conversations.Where(c => c.GetMessagesFromYesterday().Count > 0).ToList();
        }
        
        /// <summary>
        /// Gets all messages from yesterday across all conversations
        /// </summary>
        public List<(Conversation conversation, List<Message> messages)> GetAllMessagesFromYesterday()
        {
            var result = new List<(Conversation, List<Message>)>();
            
            foreach (var conversation in conversations)
            {
                var yesterdayMessages = conversation.GetMessagesFromYesterday();
                if (yesterdayMessages.Count > 0)
                {
                    result.Add((conversation, yesterdayMessages));
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Saves all conversations to persistent storage
        /// </summary>
        private void SaveConversations()
        {
            try
            {
                ConversationData data = new ConversationData { conversations = conversations };
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(saveFilePath, json);
                Debug.Log($"Conversations saved to {saveFilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save conversations: {e.Message}");
            }
        }
        
        /// <summary>
        /// Loads conversations from persistent storage
        /// </summary>
        private void LoadConversations()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    ConversationData data = JsonUtility.FromJson<ConversationData>(json);
                    conversations = data.conversations ?? new List<Conversation>();
                    Debug.Log($"Loaded {conversations.Count} conversations from {saveFilePath}");
                }
                else
                {
                    Debug.Log("No saved conversations found, starting fresh");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load conversations: {e.Message}");
                conversations = new List<Conversation>();
            }
        }
        
        /// <summary>
        /// Clears all conversations (useful for testing)
        /// </summary>
        public void ClearAllConversations()
        {
            conversations.Clear();
            SaveConversations();
        }
    }
    
    [Serializable]
    public class ConversationData
    {
        public List<Conversation> conversations;
    }
}
