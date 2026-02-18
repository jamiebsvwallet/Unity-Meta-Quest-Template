# Conversation History System - Usage Guide

This guide explains how to implement and use the conversation history system in your Unity Meta Quest VR application.

## Quick Start

### 1. Basic Setup

The Conversation System automatically initializes when you access it for the first time. No manual setup is required.

```csharp
using ConversationSystem;

// The ConversationManager singleton is ready to use immediately
var allConversations = ConversationManager.Instance.GetAllConversations();
```

### 2. Creating and Managing Conversations

```csharp
// Create a new conversation
Conversation myConversation = ConversationManager.Instance.CreateConversation("Team Meeting");

// Add messages to the conversation
ConversationManager.Instance.AddMessageToConversation(
    myConversation.id, 
    "Alice", 
    "Hello everyone!"
);

ConversationManager.Instance.AddMessageToConversation(
    myConversation.id, 
    "Bob", 
    "Hi Alice! Ready for the demo?"
);
```

### 3. Viewing Yesterday's Conversations

The main feature of this system is the ability to filter conversations by date, particularly to view messages from yesterday.

#### Method 1: Get Conversations with Yesterday's Messages

```csharp
// Get all conversations that contain messages from yesterday
List<Conversation> yesterdayConversations = 
    ConversationManager.Instance.GetConversationsFromYesterday();

foreach (var conversation in yesterdayConversations)
{
    Debug.Log($"Conversation: {conversation.title}");
    var yesterdayMessages = conversation.GetMessagesFromYesterday();
    
    foreach (var message in yesterdayMessages)
    {
        Debug.Log($"  {message.sender}: {message.content}");
        Debug.Log($"  Sent at: {message.GetDateTime()}");
    }
}
```

#### Method 2: Get All Yesterday's Messages Across All Conversations

```csharp
// Get all messages from yesterday with their conversation context
var yesterdayData = ConversationManager.Instance.GetAllMessagesFromYesterday();

foreach (var (conversation, messages) in yesterdayData)
{
    Debug.Log($"=== {conversation.title} ===");
    
    foreach (var message in messages)
    {
        Debug.Log($"{message.sender} ({message.GetDateTime():MMM dd, HH:mm}): {message.content}");
    }
}
```

## UI Setup

### Creating a Conversation History Display

1. **Create a Canvas** in your scene (if you don't have one already):
   - GameObject → UI → Canvas
   - Set Canvas Scaler to "Scale With Screen Size" for VR compatibility
   - For VR, you may want to set the Canvas to "World Space" and position it in front of the user

2. **Create UI Structure**:
   ```
   Canvas
   ├── ConversationHistoryPanel
   │   ├── HeaderText (TextMeshPro)
   │   ├── ButtonContainer
   │   │   ├── ShowAllButton
   │   │   └── ShowYesterdayButton
   │   └── ScrollView
   │       └── Content (this is your messageContainer)
   ```

3. **Create Message Item Prefab**:
   - Create a new UI GameObject
   - Add TextMeshProUGUI components for:
     - Conversation Title (textFields[0])
     - Message Content (textFields[1])
     - Timestamp (textFields[2])
   - Save as a prefab (e.g., `MessageItemPrefab`)

4. **Attach ConversationHistoryUI Component**:
   - Add the `ConversationHistoryUI` script to your ConversationHistoryPanel
   - Assign references in the Inspector:
     - **Message Container**: The Content GameObject inside ScrollView
     - **Message Item Prefab**: Your MessageItemPrefab
     - **Show All Button**: Button to show all conversations
     - **Show Yesterday Button**: Button to filter yesterday's conversations
     - **Header Text**: TextMeshProUGUI for the header
     - **Scroll Rect**: The ScrollView's ScrollRect component

### VR-Specific Considerations

For Meta Quest applications, consider these UX improvements:

1. **World Space Canvas**: Position the UI in the user's view
   ```csharp
   Canvas canvas = GetComponent<Canvas>();
   canvas.renderMode = RenderMode.WorldSpace;
   canvas.worldCamera = Camera.main;
   transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
   transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
   ```

2. **Use XR Interaction**: Make buttons interactable with XR ray interactors
   - Add `XRSimpleInteractable` to buttons
   - Ensure buttons have colliders for ray interaction

3. **Readable Text**: Use appropriate text sizes for VR (larger than desktop UI)
   - Recommended: 24-36 point size for TextMeshPro
   - High contrast colors for readability

## Testing with Demo Data

The `ConversationDemo` component helps you test the system with sample data:

```csharp
// In Unity Editor or at runtime:
GameObject demoObject = new GameObject("ConversationDemo");
ConversationDemo demo = demoObject.AddComponent<ConversationDemo>();

// Populate sample conversations (includes messages from different days)
demo.PopulateSampleData();

// Clear all data when needed
demo.ClearAllData();
```

Or attach the component to a GameObject in your scene and configure it in the Inspector:
- **Populate On Start**: Automatically create sample data when the scene starts
- **Clear Existing Data**: Clear previous data before populating

## Advanced Usage

### Custom Date Filtering

You can extend the system to filter by custom date ranges:

```csharp
// Extension method to filter by custom date
public static class ConversationExtensions
{
    public static List<Message> GetMessagesByDate(this Conversation conversation, DateTime date)
    {
        return conversation.messages
            .Where(m => m.GetDateTime().Date == date.Date)
            .ToList();
    }
    
    public static List<Message> GetMessagesInDateRange(
        this Conversation conversation, 
        DateTime startDate, 
        DateTime endDate)
    {
        return conversation.messages
            .Where(m => {
                var msgDate = m.GetDateTime();
                return msgDate >= startDate && msgDate <= endDate;
            })
            .ToList();
    }
}

// Usage:
DateTime yesterday = DateTime.UtcNow.AddDays(-1);
var yesterdayMessages = myConversation.GetMessagesByDate(yesterday);
```

### Persistent Storage

Conversations are automatically saved to persistent storage:
- **Location**: `Application.persistentDataPath/conversations.json`
- **On Quest**: `/data/data/com.YourCompany.YourApp/files/conversations.json`
- **Format**: JSON (human-readable)

To manually save or load:
```csharp
// Data is automatically saved when:
// - Creating a new conversation
// - Adding a message to a conversation
// - Clearing all conversations

// Data is automatically loaded when ConversationManager initializes
```

### Error Handling

The system includes built-in error handling and logging:

```csharp
// Adding a message to a non-existent conversation
ConversationManager.Instance.AddMessageToConversation("invalid-id", "User", "Message");
// Logs: "Conversation with id invalid-id not found"

// If save fails (e.g., no write permissions)
// Logs: "Failed to save conversations: [error details]"

// If load fails (e.g., corrupted JSON)
// Logs: "Failed to load conversations: [error details]"
// System starts fresh with empty conversation list
```

## Integration Examples

### Example 1: Voice Chat History

```csharp
public class VoiceChatManager : MonoBehaviour
{
    private Conversation currentConversation;
    
    void StartVoiceChat(string roomName)
    {
        currentConversation = ConversationManager.Instance.CreateConversation(roomName);
    }
    
    void OnVoiceMessageReceived(string speaker, string transcription)
    {
        ConversationManager.Instance.AddMessageToConversation(
            currentConversation.id,
            speaker,
            transcription
        );
    }
    
    void ShowYesterdaysChatHistory()
    {
        var yesterdayMessages = currentConversation.GetMessagesFromYesterday();
        // Display in UI
    }
}
```

### Example 2: Multiplayer Text Chat

```csharp
public class MultiplayerChat : MonoBehaviour
{
    private Conversation sessionConversation;
    
    void OnSessionStart(string sessionId)
    {
        sessionConversation = ConversationManager.Instance.CreateConversation($"Session {sessionId}");
    }
    
    void SendChatMessage(string message)
    {
        string senderName = GetLocalPlayerName();
        ConversationManager.Instance.AddMessageToConversation(
            sessionConversation.id,
            senderName,
            message
        );
        
        // Also send to network...
    }
    
    void OnRemoteMessage(string senderName, string message)
    {
        ConversationManager.Instance.AddMessageToConversation(
            sessionConversation.id,
            senderName,
            message
        );
    }
}
```

## Troubleshooting

### Issue: No conversations appearing in UI

**Solution**: 
1. Check that ConversationManager is initialized: `ConversationManager.Instance`
2. Verify UI references are assigned in Inspector
3. Check console for error messages
4. Try populating with demo data first: `ConversationDemo.PopulateSampleData()`

### Issue: "Yesterday" filter shows no messages

**Solution**:
1. Ensure messages actually exist from yesterday (check timestamps)
2. Use ConversationDemo to create test data with historical timestamps
3. Check system date/time is correct
4. Note: "Yesterday" uses UTC time for consistency across time zones

### Issue: Conversations not persisting between sessions

**Solution**:
1. Check console for save/load error messages
2. Verify write permissions to `Application.persistentDataPath`
3. On Quest, check the app has storage permissions
4. Check that the JSON file exists and is valid

## API Reference

### ConversationManager

- `CreateConversation(string title)` - Creates a new conversation
- `AddMessageToConversation(string conversationId, string sender, string content)` - Adds a message
- `GetAllConversations()` - Returns all conversations
- `GetConversationsFromYesterday()` - Returns conversations with yesterday's messages
- `GetAllMessagesFromYesterday()` - Returns tuple of (Conversation, Messages) for yesterday
- `ClearAllConversations()` - Clears all data (for testing)

### Message

- `GetDateTime()` - Returns DateTime of message
- `IsFromYesterday()` - Returns true if message is from yesterday

### Conversation

- `AddMessage(string sender, string content)` - Adds a message
- `GetMessagesFromYesterday()` - Returns messages from yesterday
- `GetLastMessageTime()` - Returns DateTime of most recent message

### ConversationHistoryUI

- `ShowAllConversations()` - Displays all conversations in UI
- `ShowYesterdayConversations()` - Displays only yesterday's conversations in UI

## Performance Considerations

- Messages are stored in memory while the app runs
- Conversations are saved to disk whenever modified (not on every frame)
- For large conversation histories (1000+ messages), consider pagination
- JSON serialization is synchronous - for very large datasets, consider async saving

## Future Enhancements

Consider implementing:
- Search/filter by sender or keyword
- Message editing/deletion
- Conversation archiving
- Export to external formats
- Audio/image attachments
- Read/unread status tracking
