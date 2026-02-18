# Unity-Meta-Quest-Template
Unity template project optimised for Meta Quest devices.
Current Project Version Unity 2022.3.2f

Changes from default Unity project with Android build target:  

Packages:  
Installed XR Plug-in Management. Targets Quest, Quest 2, Quest Pro  
Installed OpenXR Plugin  
Installed Oculus XR Plugin  
Installed XR Interaction Toolkit version 2.4.0
Installed XR Hands 1.2.1
Installed Meta OpenXR Feature 0.1.1
URP Samples imported (includes useful blob shadow shader)  

Quality Settings:  
Custom Quality profiles  for Quest, Quest 2, Quest Pro, Quest 3. (Default Quest 2)  
Vsync disabled  
Anisotropic Textures set to Per Texture.  
Shadowmask Mode set to Shadowmask  
LOD Bias set to 0.7  
Skin Weights set to 2 Bones  

Player Settings:  
Auto Graphics API disabled, set to OpenGL ES 3.0  
Texture Compression format set to ATSC  
Minimum API Level set to Android 10.0 (API Level 29)  
Lightmap encoding set to High Quality  
HDR Cubemap encoding set to High Quality  
Use Incremental GC enabled 
Scripting Backend set to IL2CPP  
IL2CPP Code generation set to Faster (smaller) builds *Change this to Faster runtime for release build  
Target Architecture set to Arm64  
Active Input Handling set to Both  
Optimize Mesh Data enabled   


Physics Settings:  
Reuse Collision Callbacks enabled  
Default Max Angular Speed set to 7 
Enabled Improved Patch Friction 

Time Settings:  
Maximum Allowed Timestep set to 0.0138 (for 72 Hz)  

URP Renderer Settings:  
Shadows – Transparent Receive Shadows disabled   

URP Pipeline asset settings for Quest 2: (minor differences for Quest 1 and Quest Pro)  
Disable Terrain Holes  
Main Light – Cast Shadows disabled  
Additional Lights set to Per Pixel  

Notes:  
The project is set up to have to realtime shadows and no additional lights.
Adjust URP shadow settings according to the needs of your game/app.   
For release builds enable Low Overhead Mode under Oculus XR Plug-in Management options.  

Cornell Box model taken form Sketchfab - Cornell Box- Original - Download Free 3D model by t-ly (@t-ly) https://sketchfab.com/3d-models/cornell-box-original-0d18de8d108c4c9cab1a4405698cc6b6

## Conversation History System

This template includes a conversation history system that allows you to store, retrieve, and filter conversations by date.

### Features
- **Message Storage**: Save conversations with timestamps
- **Date Filtering**: View messages from specific dates (e.g., yesterday)
- **Persistent Storage**: Conversations are saved to device storage and persist between sessions
- **VR-Ready UI**: UI components designed for VR interaction

### Usage

#### Basic Setup

1. **Add ConversationManager to your scene**: The ConversationManager automatically initializes as a singleton. No manual setup required.

2. **Create a conversation**:
```csharp
using ConversationSystem;

// Create a new conversation
Conversation conv = ConversationManager.Instance.CreateConversation("My Conversation");

// Add messages to the conversation
ConversationManager.Instance.AddMessageToConversation(conv.id, "Alice", "Hello!");
ConversationManager.Instance.AddMessageToConversation(conv.id, "Bob", "Hi Alice!");
```

3. **Retrieve conversations**:
```csharp
// Get all conversations
List<Conversation> allConversations = ConversationManager.Instance.GetAllConversations();

// Get only conversations with messages from yesterday
List<Conversation> yesterdayConvs = ConversationManager.Instance.GetConversationsFromYesterday();

// Get all messages from yesterday across all conversations
var yesterdayMessages = ConversationManager.Instance.GetAllMessagesFromYesterday();
```

#### UI Integration

1. **Add ConversationHistoryUI component** to a Canvas in your scene
2. **Assign references** in the Inspector:
   - Message Container (Transform): Container where message items will be displayed
   - Message Item Prefab (GameObject): Prefab for each message item
   - Show All Button (Button): Button to show all conversations
   - Show Yesterday Button (Button): Button to filter yesterday's conversations
   - Header Text (TextMeshProUGUI): Text to display current filter
   - Scroll Rect (ScrollRect): Scroll view component

3. The UI will automatically display conversations when buttons are clicked.

#### Demo / Testing

The `ConversationDemo` script can populate the system with sample data for testing:

1. Add the `ConversationDemo` component to any GameObject in your scene
2. Check "Populate On Start" to automatically create sample data
3. Check "Clear Existing Data" to reset before populating
4. Or call `PopulateSampleData()` manually from code

### How to View Yesterday's Conversations

To view conversations from yesterday:

**Via Code**:
```csharp
var yesterdayMessages = ConversationManager.Instance.GetAllMessagesFromYesterday();
foreach (var (conversation, messages) in yesterdayMessages)
{
    Debug.Log($"Conversation: {conversation.title}");
    foreach (var message in messages)
    {
        Debug.Log($"{message.sender}: {message.content} at {message.GetDateTime()}");
    }
}
```

**Via UI**:
1. Add the `ConversationHistoryUI` component to your scene
2. Click the "Show Yesterday" button to filter messages from yesterday
3. Click the "Show All" button to return to viewing all messages

### Storage Location
Conversations are automatically saved to: `Application.persistentDataPath/conversations.json`

On Quest devices, this is typically: `/data/data/com.YourCompany.YourApp/files/conversations.json`
