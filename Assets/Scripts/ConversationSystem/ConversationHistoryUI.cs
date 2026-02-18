using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ConversationSystem
{
    /// <summary>
    /// UI component for displaying conversation history
    /// </summary>
    public class ConversationHistoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform messageContainer;
        [SerializeField] private GameObject messageItemPrefab;
        [SerializeField] private Button showAllButton;
        [SerializeField] private Button showYesterdayButton;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private ScrollRect scrollRect;
        
        private List<GameObject> currentMessageItems = new List<GameObject>();
        
        void Start()
        {
            if (showAllButton != null)
                showAllButton.onClick.AddListener(ShowAllConversations);
            
            if (showYesterdayButton != null)
                showYesterdayButton.onClick.AddListener(ShowYesterdayConversations);
            
            // Show all conversations by default
            ShowAllConversations();
        }
        
        /// <summary>
        /// Displays all conversations
        /// </summary>
        public void ShowAllConversations()
        {
            ClearMessages();
            
            if (headerText != null)
                headerText.text = "All Conversations";
            
            List<Conversation> conversations = ConversationManager.Instance.GetAllConversations();
            
            foreach (var conversation in conversations)
            {
                foreach (var message in conversation.messages)
                {
                    CreateMessageItem(conversation.title, message);
                }
            }
            
            Debug.Log($"Displaying {conversations.Count} conversations");
        }
        
        /// <summary>
        /// Displays only conversations from yesterday
        /// </summary>
        public void ShowYesterdayConversations()
        {
            ClearMessages();
            
            if (headerText != null)
                headerText.text = "Yesterday's Conversations";
            
            var yesterdayData = ConversationManager.Instance.GetAllMessagesFromYesterday();
            
            if (yesterdayData.Count == 0)
            {
                CreateInfoMessage("No conversations from yesterday");
                Debug.Log("No conversations from yesterday found");
                return;
            }
            
            foreach (var (conversation, messages) in yesterdayData)
            {
                foreach (var message in messages)
                {
                    CreateMessageItem(conversation.title, message);
                }
            }
            
            Debug.Log($"Displaying {yesterdayData.Count} conversations from yesterday");
        }
        
        /// <summary>
        /// Creates a message item in the UI
        /// </summary>
        private void CreateMessageItem(string conversationTitle, Message message)
        {
            if (messageItemPrefab == null || messageContainer == null)
            {
                Debug.LogWarning("Message prefab or container not assigned");
                return;
            }
            
            GameObject item = Instantiate(messageItemPrefab, messageContainer);
            
            // Set the text content
            TextMeshProUGUI[] textFields = item.GetComponentsInChildren<TextMeshProUGUI>();
            if (textFields.Length >= 3)
            {
                textFields[0].text = conversationTitle;
                textFields[1].text = $"{message.sender}: {message.content}";
                textFields[2].text = FormatTimestamp(message.GetDateTime());
            }
            else if (textFields.Length > 0)
            {
                // Fallback if prefab structure is different
                textFields[0].text = $"[{conversationTitle}] {message.sender}: {message.content}\n{FormatTimestamp(message.GetDateTime())}";
            }
            
            currentMessageItems.Add(item);
        }
        
        /// <summary>
        /// Creates an info message (e.g., "No messages found")
        /// </summary>
        private void CreateInfoMessage(string infoText)
        {
            if (messageItemPrefab == null || messageContainer == null)
                return;
            
            GameObject item = Instantiate(messageItemPrefab, messageContainer);
            
            TextMeshProUGUI[] textFields = item.GetComponentsInChildren<TextMeshProUGUI>();
            if (textFields.Length > 0)
            {
                textFields[0].text = infoText;
            }
            
            currentMessageItems.Add(item);
        }
        
        /// <summary>
        /// Clears all displayed messages
        /// </summary>
        private void ClearMessages()
        {
            foreach (var item in currentMessageItems)
            {
                if (item != null)
                    Destroy(item);
            }
            currentMessageItems.Clear();
        }
        
        /// <summary>
        /// Formats a timestamp for display (uses 24-hour format for consistency)
        /// </summary>
        private string FormatTimestamp(System.DateTime dateTime)
        {
            System.DateTime localTime = dateTime.ToLocalTime();
            return localTime.ToString("MMM dd, yyyy HH:mm");
        }
        
        void OnDestroy()
        {
            if (showAllButton != null)
                showAllButton.onClick.RemoveListener(ShowAllConversations);
            
            if (showYesterdayButton != null)
                showYesterdayButton.onClick.RemoveListener(ShowYesterdayConversations);
        }
    }
}
