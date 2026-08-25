using Microsoft.AspNetCore.SignalR.Client;

namespace GalaxyUML.Client.Shared.Services;

public class DiagramHubClient
{
    private HubConnection? _hubConnection;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    // Events
    public event Action<Guid, int, int, int, int, IReadOnlyCollection<string>, IReadOnlyCollection<string>>? OnClassBoxAdded;
    public event Action<Guid, int, int>? OnElementMoved;
    public event Action<Guid, Guid, Guid, string?>? OnLineAdded;
    public event Action<Guid>? OnElementDeleted;
    public event Action<Guid>? OnBoardCleared;
    public event Action<Guid, Guid, string, string, DateTime>? OnChatMessageReceived;
    public event Action<Guid, bool>? OnDrawPermissionChanged;
    public event Action<Guid>? OnParticipantsUpdated;
    public event Action<Guid>? OnMeetingEnded;

    public async Task ConnectAsync(string baseUrl, string? jwtToken, Guid meetingId)
    {
        await DisconnectAsync();

        var hubUrl = $"{baseUrl.Trim().TrimEnd('/')}/diagramHub";
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                if (!string.IsNullOrWhiteSpace(jwtToken))
                {
                    options.AccessTokenProvider = () => Task.FromResult(jwtToken)!;
                }
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<Guid, int, int, int, int, IReadOnlyCollection<string>, IReadOnlyCollection<string>>(
            "ClassBoxAdded", (id, x1, y1, x2, y2, attrs, methods) =>
            {
                OnClassBoxAdded?.Invoke(id, x1, y1, x2, y2, attrs ?? [], methods ?? []);
            });

        _hubConnection.On<Guid, int, int>("ElementMoved", (elementId, dx, dy) =>
        {
            OnElementMoved?.Invoke(elementId, dx, dy);
        });

        _hubConnection.On<Guid, Guid, Guid, string?>("LineAdded", (id, startBoxId, endBoxId, middleText) =>
        {
            OnLineAdded?.Invoke(id, startBoxId, endBoxId, middleText);
        });

        _hubConnection.On<Guid>("ElementDeleted", (elementId) =>
        {
            OnElementDeleted?.Invoke(elementId);
        });

        _hubConnection.On<Guid>("BoardCleared", (boardId) =>
        {
            OnBoardCleared?.Invoke(boardId);
        });

        _hubConnection.On<Guid, Guid, string, string, DateTime>("ChatMessageReceived", (id, senderId, username, content, sentAt) =>
        {
            OnChatMessageReceived?.Invoke(id, senderId, username, content, sentAt);
        });

        _hubConnection.On<Guid, bool>("DrawPermissionChanged", (targetUserId, canDraw) =>
        {
            OnDrawPermissionChanged?.Invoke(targetUserId, canDraw);
        });

        _hubConnection.On<Guid>("ParticipantsUpdated", (mId) =>
        {
            OnParticipantsUpdated?.Invoke(mId);
        });

        _hubConnection.On<Guid>("MeetingEnded", (mId) =>
        {
            OnMeetingEnded?.Invoke(mId);
        });

        await _hubConnection.StartAsync();
        await _hubConnection.InvokeAsync("JoinMeeting", meetingId);
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection is not null)
        {
            try
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
            catch { }
            _hubConnection = null;
        }
    }
}
