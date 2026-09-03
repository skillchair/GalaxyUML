using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using GalaxyUML.Client.Shared.Models;
using GalaxyUML.Client.Shared.Services;

namespace GalaxyUML.UI.Avalonia;

public partial class MainWindow : Window
{
    // Shared API & SignalR Services
    private readonly GalaxyApiClient _api = new();
    private readonly DiagramHubClient _hub = new();

    // Observable Collections
    private readonly ObservableCollection<TeamItem> _teams = [];
    private readonly ObservableCollection<TeamMemberItem> _teamMembers = [];
    private readonly ObservableCollection<MeetingItem> _meetings = [];
    private readonly ObservableCollection<MeetingParticipantItem> _meetingParticipants = [];
    private readonly ObservableCollection<ChatMessageItem> _chatMessages = [];
    private readonly ObservableCollection<BoxItem> _boxes = [];

    // Fast Lookups
    private readonly Dictionary<Guid, BoxItem> _boxesById = [];
    private readonly Dictionary<Guid, Border> _boxVisualsById = [];
    private readonly Dictionary<Border, BoxItem> _visualToBox = [];
    private readonly List<LineVisualItem> _lineLinks = [];
    private readonly HashSet<Guid> _pendingLocalMoves = [];

    // Session State
    private Guid? _currentUserId;
    private string? _currentUsername;
    private Guid? _activeMeetingId;
    private Guid? _activeBoardId;
    private bool _currentUserCanDraw;

    // Selection
    private object? _selectedCanvasElement;
    private Border? _selectedBoxVisual;
    private Line? _selectedLineVisual;

    // Tool & Drag state
    private string _selectedTool = "ClassBox";
    private Point _dragStart;
    private Rectangle? _previewRectangle;
    private bool _isDragging;

    // Movement state
    private bool _isMovingBox;
    private Border? _movingBoxVisual;
    private BoxItem? _movingBox;
    private Point _moveStartPoint;
    private double _moveStartLeft;
    private double _moveStartTop;

    public MainWindow()
    {
        InitializeComponent();

        TeamsListBox.ItemsSource = _teams;
        TeamMembersListBox.ItemsSource = _teamMembers;
        MeetingTeamComboBox.ItemsSource = _teams;
        MeetingsListBox.ItemsSource = _meetings;
        MeetingParticipantsListBox.ItemsSource = _meetingParticipants;
        ChatMessagesListBox.ItemsSource = _chatMessages;
        StartBoxComboBox.ItemsSource = _boxes;
        EndBoxComboBox.ItemsSource = _boxes;

        SetupSignalREvents();
        UpdateSessionUI();
        UpdateDrawPermissionUI();
    }

    private void SetupSignalREvents()
    {
        // 1. ClassBox Added
        _hub.OnClassBoxAdded += (id, x1, y1, x2, y2, attrs, methods) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (_boxesById.ContainsKey(id)) return;

                var box = new BoxItem(id, x1, y1, x2, y2, "ClassBox", attrs ?? [], methods ?? []);
                _boxes.Add(box);
                _boxesById[id] = box;

                RenderClassBoxVisual(box);
            });
        };

        // 2. Element Moved
        _hub.OnElementMoved += (elementId, dx, dy) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (_pendingLocalMoves.Remove(elementId)) return;

                if (_boxesById.TryGetValue(elementId, out var box))
                {
                    var newLeft = box.X1 + dx;
                    var newTop = box.Y1 + dy;
                    SetBoxPosition(box, newLeft, newTop);
                    UpdateConnectedLines(elementId);
                }
            });
        };

        // 3. Line Added
        _hub.OnLineAdded += (id, startBoxId, endBoxId, middleText, _, _) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (_lineLinks.Any(l => l.Id == id)) return;
                RenderLineVisual(id, startBoxId, endBoxId, middleText);
            });
        };

        // 4. Element Deleted
        _hub.OnElementDeleted += (elementId) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                RemoveElementLocally(elementId);
            });
        };

        // 5. Board Cleared
        _hub.OnBoardCleared += (boardId) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (_activeBoardId == boardId)
                {
                    ResetLocalBoard();
                }
            });
        };

        // 6. Chat Message Received
        _hub.OnChatMessageReceived += (id, senderId, username, content, sentAt) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (_chatMessages.Any(m => m.Id == id)) return;
                _chatMessages.Add(new ChatMessageItem(id, senderId, username, content, sentAt));
                if (_chatMessages.Count > 0)
                {
                    ChatMessagesListBox.ScrollIntoView(_chatMessages.Last());
                }
            });
        };

        // 7. Draw Permission Changed
        _hub.OnDrawPermissionChanged += (targetUserId, canDraw) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (targetUserId == _currentUserId)
                {
                    _currentUserCanDraw = canDraw;
                    UpdateDrawPermissionUI();
                }

                var participant = _meetingParticipants.FirstOrDefault(p => p.UserId == targetUserId);
                if (participant is not null)
                {
                    participant.CanDraw = canDraw;
                }
            });
        };

        // 8. Participants Updated
        _hub.OnParticipantsUpdated += (mId) =>
        {
            Dispatcher.UIThread.Post(async () =>
            {
                if (_activeMeetingId == mId)
                {
                    await RefreshMeetingParticipantsAsync(mId);
                }
            });
        };

        // 9. Meeting Ended
        _hub.OnMeetingEnded += (endedMeetingId) =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (_activeMeetingId == endedMeetingId)
                {
                    _activeMeetingId = null;
                    _activeBoardId = null;
                    _currentUserCanDraw = false;
                    ResetLocalBoard();
                    UpdateDrawPermissionUI();
                    _meetingParticipants.Clear();
                    _chatMessages.Clear();
                    SignalRStatusText.Text = "SignalR: Isključen";
                }
            });
        };
    }

    #region Auth Operations

    private async void LoginButton_Click(object? sender, RoutedEventArgs e)
    {
        await PerformLoginAsync();
    }

    private async void LoginInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await PerformLoginAsync();
        }
    }

    private async Task PerformLoginAsync()
    {
        var username = LoginUsernameTextBox.Text?.Trim();
        var password = LoginPasswordBox.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return;

        try
        {
            _api.BaseUrl = ApiBaseUrlTextBox.Text?.Trim() ?? "http://localhost:5248";
            var result = await _api.LoginAsync(username, password);

            _currentUserId = result.UserId;
            _currentUsername = result.Username;

            UpdateSessionUI();
            AuthStatusBanner.Text = $"Uspešno ste prijavljeni kao '{_currentUsername}'.";

            await RefreshLoggedInUserTeamsAsync();
            MainTabControl.SelectedIndex = 1;
        }
        catch (Exception ex)
        {
            AuthStatusBanner.Text = $"Greška: {ex.Message}";
        }
    }

    private async void RegisterButton_Click(object? sender, RoutedEventArgs e)
    {
        var firstName = RegisterFirstNameTextBox.Text?.Trim();
        var lastName = RegisterLastNameTextBox.Text?.Trim();
        var username = RegisterUsernameTextBox.Text?.Trim();
        var email = RegisterEmailTextBox.Text?.Trim();
        var password = RegisterPasswordBox.Text;

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        try
        {
            _api.BaseUrl = ApiBaseUrlTextBox.Text?.Trim() ?? "http://localhost:5248";
            await _api.RegisterAsync(firstName, lastName, username, email, password);

            RegisterFirstNameTextBox.Text = "";
            RegisterLastNameTextBox.Text = "";
            RegisterUsernameTextBox.Text = "";
            RegisterEmailTextBox.Text = "";
            RegisterPasswordBox.Text = "";

            LoginUsernameTextBox.Text = username;
            LoginPasswordBox.Text = password;

            AuthStatusBanner.Text = "Registracija uspešna! Možete se prijaviti.";
        }
        catch (Exception ex)
        {
            AuthStatusBanner.Text = $"Greška pri registraciji: {ex.Message}";
        }
    }

    private async void LogoutButton_Click(object? sender, RoutedEventArgs e)
    {
        await _hub.DisconnectAsync();

        _api.JwtToken = null;
        _currentUserId = null;
        _currentUsername = null;
        _activeMeetingId = null;
        _activeBoardId = null;
        _currentUserCanDraw = false;

        _teams.Clear();
        _teamMembers.Clear();
        _meetings.Clear();
        _meetingParticipants.Clear();
        _chatMessages.Clear();
        ResetLocalBoard();

        UpdateSessionUI();
        UpdateDrawPermissionUI();
        MainTabControl.SelectedIndex = 0;
        AuthStatusBanner.Text = "Uspešno ste odjavljeni.";
    }

    private void UpdateSessionUI()
    {
        if (_currentUserId is not null && !string.IsNullOrWhiteSpace(_currentUsername))
        {
            SessionStatusText.Text = $"Korisnik: [{_currentUsername}]";
            SessionStatusText.Foreground = Brushes.Black;
            UserBadgeBorder.Background = Brushes.White;
            UserBadgeBorder.BorderBrush = Brushes.Black;
            LogoutButton.IsVisible = true;
        }
        else
        {
            SessionStatusText.Text = "Niste prijavljeni";
            SessionStatusText.Foreground = Brushes.Black;
            UserBadgeBorder.Background = Brushes.White;
            UserBadgeBorder.BorderBrush = Brushes.Black;
            LogoutButton.IsVisible = false;
        }
    }

    #endregion

    #region Team Operations

    private async void RefreshTeamsButton_Click(object? sender, RoutedEventArgs e)
    {
        await RefreshLoggedInUserTeamsAsync();
    }

    private async Task RefreshLoggedInUserTeamsAsync()
    {
        if (_currentUserId is null) return;

        try
        {
            var teams = await _api.GetUserTeamsAsync();
            _teams.Clear();
            foreach (var t in teams) _teams.Add(t);
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void CreateTeamButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_currentUserId is null) return;

        var teamName = CreateTeamNameTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(teamName)) return;

        try
        {
            var created = await _api.CreateTeamAsync(teamName);
            CreateTeamNameTextBox.Text = "";
            TeamStatusText.Text = $"Kreiran tim '{created.TeamName}'. Kod: {created.TeamCode}";
            await RefreshLoggedInUserTeamsAsync();
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void FindTeamByCodeButton_Click(object? sender, RoutedEventArgs e)
    {
        var code = JoinCodeTextBox.Text?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code) || code.Length != 6) return;

        try
        {
            var found = await _api.FindTeamByCodeAsync(code);
            TeamStatusText.Text = $"Pronađen tim: '{found.TeamName}' (Članova: {found.MemberCount}).";
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void JoinByCodeButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_currentUserId is null) return;

        var code = JoinCodeTextBox.Text?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code) || code.Length != 6) return;

        try
        {
            var joined = await _api.JoinTeamByCodeAsync(code);
            JoinCodeTextBox.Text = "";
            TeamStatusText.Text = $"Učlanjeni ste u tim '{joined.TeamName}'.";
            await RefreshLoggedInUserTeamsAsync();
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void TeamsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamItem selectedTeam)
        {
            _teamMembers.Clear();
            SelectedTeamBannerText.Text = "Selektujte tim sa liste za prikaz članova.";
            return;
        }

        SelectedTeamBannerText.Text = $"Tim: {selectedTeam.TeamName} (Kod: {selectedTeam.TeamCode})";
        await RefreshTeamMembersAsync(selectedTeam.Id);
    }

    private async Task RefreshTeamMembersAsync(Guid teamId)
    {
        try
        {
            var members = await _api.GetTeamMembersAsync(teamId);
            _teamMembers.Clear();
            foreach (var m in members) _teamMembers.Add(m);
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void ChangeRoleButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamItem selectedTeam ||
            TeamMembersListBox.SelectedItem is not TeamMemberItem selectedMember)
        {
            return;
        }

        var newRole = (ChangeRoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Organizer";

        try
        {
            await _api.ChangeRoleAsync(selectedTeam.Id, selectedMember.UserId, newRole);
            TeamStatusText.Text = $"Uloga člana {selectedMember.Username} promenjena u {newRole}.";
            await RefreshTeamMembersAsync(selectedTeam.Id);
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void BanMemberButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamItem selectedTeam ||
            TeamMembersListBox.SelectedItem is not TeamMemberItem selectedMember)
        {
            return;
        }

        var reason = BanReasonTextBox.Text?.Trim();
        try
        {
            await _api.BanMemberAsync(selectedTeam.Id, selectedMember.UserId, string.IsNullOrWhiteSpace(reason) ? null : reason);
            BanReasonTextBox.Text = "";
            TeamStatusText.Text = $"Član {selectedMember.Username} je banovan.";
            await RefreshTeamMembersAsync(selectedTeam.Id);
            await RefreshLoggedInUserTeamsAsync();
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void LeaveTeamButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamItem selectedTeam) return;

        try
        {
            await _api.LeaveTeamAsync(selectedTeam.Id);
            _teamMembers.Clear();
            await RefreshLoggedInUserTeamsAsync();
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void DeleteTeamButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamItem selectedTeam) return;

        try
        {
            await _api.DeleteTeamAsync(selectedTeam.Id);
            _teamMembers.Clear();
            await RefreshLoggedInUserTeamsAsync();
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    #endregion

    #region Meeting Operations

    private async void MeetingTeamComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (MeetingTeamComboBox.SelectedItem is not TeamItem selectedTeam)
        {
            _meetings.Clear();
            return;
        }

        await RefreshActiveMeetingsForTeamAsync(selectedTeam.Id);
    }

    private async void RefreshMeetingsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (MeetingTeamComboBox.SelectedItem is TeamItem selectedTeam)
        {
            await RefreshActiveMeetingsForTeamAsync(selectedTeam.Id);
        }
    }

    private async Task RefreshActiveMeetingsForTeamAsync(Guid teamId)
    {
        try
        {
            var meeting = await _api.GetActiveMeetingForTeamAsync(teamId);
            _meetings.Clear();
            if (meeting is not null)
            {
                _meetings.Add(meeting);
                MeetingsListBox.SelectedItem = meeting;
            }
        }
        catch { }
    }

    private async void StartMeetingButton_Click(object? sender, RoutedEventArgs e)
    {
        if (MeetingTeamComboBox.SelectedItem is not TeamItem selectedTeam) return;

        try
        {
            var created = await _api.CreateMeetingAsync(selectedTeam.Id);

            _activeMeetingId = created.Id;
            _activeBoardId = created.BoardId;
            _currentUserCanDraw = true;

            BoardInfoText.Text = $"Board ID: {created.BoardId}";
            MeetingStatusText.Text = $"Sastanak aktivan: {created.Id}";

            _meetings.Clear();
            _meetings.Add(created);
            MeetingsListBox.SelectedItem = created;

            ResetLocalBoard();
            UpdateDrawPermissionUI();

            await RefreshMeetingParticipantsAsync(created.Id);
            await LoadChatHistoryAsync(created.Id);
            await ConnectSignalRAsync(created.Id);

            MainTabControl.SelectedItem = DiagramTabItem;
        }
        catch (Exception ex)
        {
            MeetingStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void JoinMeetingButton_Click(object? sender, RoutedEventArgs e)
    {
        if (MeetingsListBox.SelectedItem is not MeetingItem selectedMeeting) return;

        try
        {
            await _api.JoinMeetingAsync(selectedMeeting.Id);

            _activeMeetingId = selectedMeeting.Id;
            _activeBoardId = selectedMeeting.BoardId;

            BoardInfoText.Text = $"Board ID: {selectedMeeting.BoardId}";
            MeetingStatusText.Text = $"Pridruženi sastanku: {selectedMeeting.Id}";

            ResetLocalBoard();

            await RefreshMeetingParticipantsAsync(selectedMeeting.Id);
            await LoadChatHistoryAsync(selectedMeeting.Id);
            await ConnectSignalRAsync(selectedMeeting.Id);

            MainTabControl.SelectedItem = DiagramTabItem;
        }
        catch (Exception ex)
        {
            MeetingStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void EndMeetingButton_Click(object? sender, RoutedEventArgs e)
    {
        if (MeetingsListBox.SelectedItem is not MeetingItem selectedMeeting) return;

        try
        {
            await _api.EndMeetingAsync(selectedMeeting.Id);
            _meetings.Remove(selectedMeeting);

            if (_activeMeetingId == selectedMeeting.Id)
            {
                _activeMeetingId = null;
                _activeBoardId = null;
                _currentUserCanDraw = false;
                ResetLocalBoard();
                UpdateDrawPermissionUI();
                _meetingParticipants.Clear();
                _chatMessages.Clear();
                await _hub.DisconnectAsync();
                SignalRStatusText.Text = "SignalR: Isključen";
            }

            MeetingStatusText.Text = "Sastanak završen.";
        }
        catch (Exception ex)
        {
            MeetingStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void MeetingsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (MeetingsListBox.SelectedItem is not MeetingItem selectedMeeting) return;

        BoardInfoText.Text = $"Board ID: {selectedMeeting.BoardId}";
        if (_activeMeetingId == selectedMeeting.Id)
        {
            await RefreshMeetingParticipantsAsync(selectedMeeting.Id);
        }
    }

    private async Task RefreshMeetingParticipantsAsync(Guid meetingId)
    {
        try
        {
            var participants = await _api.GetParticipantsAsync(meetingId);
            _meetingParticipants.Clear();

            var currentCanDraw = false;
            foreach (var p in participants)
            {
                _meetingParticipants.Add(p);
                if (p.UserId == _currentUserId && p.CanDraw)
                {
                    currentCanDraw = true;
                }
            }

            _currentUserCanDraw = currentCanDraw;
            UpdateDrawPermissionUI();
        }
        catch { }
    }

    private async void GrantDrawButton_Click(object? sender, RoutedEventArgs e)
    {
        await ToggleDrawPermissionAsync(true);
    }

    private async void RevokeDrawButton_Click(object? sender, RoutedEventArgs e)
    {
        await ToggleDrawPermissionAsync(false);
    }

    private async Task ToggleDrawPermissionAsync(bool canDraw)
    {
        if (_activeMeetingId is null) return;
        if (MeetingParticipantsListBox.SelectedItem is not MeetingParticipantItem selectedParticipant) return;

        try
        {
            await _api.GrantDrawAsync(_activeMeetingId.Value, selectedParticipant.UserId, canDraw);
            await RefreshMeetingParticipantsAsync(_activeMeetingId.Value);
        }
        catch { }
    }

    private void SwitchToDiagramTabButton_Click(object? sender, RoutedEventArgs e)
    {
        MainTabControl.SelectedItem = DiagramTabItem;
    }

    private void UpdateDrawPermissionUI()
    {
        if (_currentUserCanDraw)
        {
            DrawPermissionBadge.Background = Brushes.White;
            DrawPermissionBadge.BorderBrush = Brushes.Black;
            DrawPermissionStatusText.Text = "Dozvola za crtanje: Aktivna";
            DrawPermissionStatusText.Foreground = Brushes.Black;
        }
        else
        {
            DrawPermissionBadge.Background = new SolidColorBrush(Color.Parse("#F4F4F4"));
            DrawPermissionBadge.BorderBrush = new SolidColorBrush(Color.Parse("#767676"));
            DrawPermissionStatusText.Text = "Samo pregled";
            DrawPermissionStatusText.Foreground = new SolidColorBrush(Color.Parse("#444444"));
        }
    }

    private async Task ConnectSignalRAsync(Guid meetingId)
    {
        try
        {
            await _hub.ConnectAsync(_api.BaseUrl, _api.JwtToken, meetingId);
            SignalRStatusText.Text = "SignalR: [Povezan]";
            SignalRBadge.Background = new SolidColorBrush(Color.Parse("#E6F4EA"));
            SignalRBadge.BorderBrush = new SolidColorBrush(Color.Parse("#137333"));
            SignalRStatusText.Foreground = new SolidColorBrush(Color.Parse("#137333"));

            if (_activeBoardId.HasValue)
            {
                await LoadExistingElementsAsync(_activeBoardId.Value);
            }
        }
        catch
        {
            SignalRStatusText.Text = "SignalR: [Greška]";
            SignalRBadge.Background = new SolidColorBrush(Color.Parse("#FCE8E6"));
            SignalRBadge.BorderBrush = new SolidColorBrush(Color.Parse("#C5221F"));
            SignalRStatusText.Foreground = new SolidColorBrush(Color.Parse("#C5221F"));
        }
    }

    #endregion

    #region Chat Operations

    private async Task LoadChatHistoryAsync(Guid meetingId)
    {
        try
        {
            var msgs = await _api.GetMessagesAsync(meetingId);
            _chatMessages.Clear();
            foreach (var m in msgs) _chatMessages.Add(m);

            if (_chatMessages.Count > 0)
            {
                ChatMessagesListBox.ScrollIntoView(_chatMessages.Last());
            }
        }
        catch { }
    }

    private async void SendChatMessageButton_Click(object? sender, RoutedEventArgs e)
    {
        await SendCurrentChatMessageAsync();
    }

    private async void ChatMessageInputTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await SendCurrentChatMessageAsync();
        }
    }

    private async Task SendCurrentChatMessageAsync()
    {
        if (_activeMeetingId is null) return;
        var content = ChatMessageInputTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(content)) return;

        try
        {
            ChatMessageInputTextBox.Text = "";
            await _api.SendMessageAsync(_activeMeetingId.Value, content);
        }
        catch { }
    }

    #endregion

    #region Canvas UML Diagramming Operations

    private void ToolComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ToolComboBox is null || ClassBoxCreationWrap is null || LineControlsWrap is null) return;
        if (ToolComboBox.SelectedItem is ComboBoxItem item && item.Content is string tool)
        {
            _selectedTool = tool;
            ClassBoxCreationWrap.IsVisible = _selectedTool == "ClassBox";
            LineControlsWrap.IsVisible = _selectedTool == "Line";
        }
    }

    private async void AddClassBoxDirectButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!_currentUserCanDraw || _activeBoardId is null) return;

        var className = NewClassNameTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(className)) className = "NovaKlasa";

        var attributes = ParseList(NewClassAttributesTextBox.Text);
        var methods = ParseList(NewClassMethodsTextBox.Text);

        var width = 160;
        var height = Math.Max(100, 30 + attributes.Count * 18 + methods.Count * 18 + 20);
        var left = 60 + (_boxes.Count * 30) % 400;
        var top = 60 + (_boxes.Count * 30) % 300;

        await CreateClassBoxAsync(left, top, left + width, top + height, className, attributes, methods);
    }

    private async Task CreateClassBoxAsync(int x1, int y1, int x2, int y2, string className, List<string> attributes, List<string> methods)
    {
        if (_activeBoardId is null) return;

        try
        {
            var id = await _api.AddClassBoxAsync(_activeBoardId.Value, x1, y1, x2, y2, attributes, methods);
            var box = new BoxItem(id, x1, y1, x2, y2, className, attributes, methods);
            if (!_boxesById.ContainsKey(box.Id))
            {
                _boxes.Add(box);
                _boxesById[box.Id] = box;
                RenderClassBoxVisual(box);
            }
        }
        catch { }
    }

    private void RenderClassBoxVisual(BoxItem box)
    {
        if (_boxVisualsById.ContainsKey(box.Id)) return;

        var width = Math.Max(140, box.X2 - box.X1);
        var height = Math.Max(90, box.Y2 - box.Y1);

        var border = new Border
        {
            Width = width,
            Height = height,
            Background = Brushes.White,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1.5),
            CornerRadius = new CornerRadius(0),
            Cursor = new Cursor(StandardCursorType.Hand)
        };

        var mainPanel = new StackPanel();

        // 1. Header (Class Name)
        var headerBorder = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#F0F0F0")),
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(6, 4, 6, 4)
        };
        var headerText = new TextBlock
        {
            Text = box.ClassName,
            FontWeight = FontWeight.Bold,
            FontSize = 13,
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = Brushes.Black
        };
        headerBorder.Child = headerText;
        mainPanel.Children.Add(headerBorder);

        // 2. Attributes
        var attributesPanel = new StackPanel { Margin = new Thickness(6, 3, 6, 3) };
        if (box.Attributes.Count > 0)
        {
            foreach (var attr in box.Attributes)
            {
                attributesPanel.Children.Add(new TextBlock
                {
                    Text = attr,
                    FontSize = 11,
                    FontFamily = new FontFamily("Consolas, Courier New, monospace"),
                    Foreground = Brushes.Black
                });
            }
        }
        mainPanel.Children.Add(attributesPanel);

        // Separator
        mainPanel.Children.Add(new Separator { Margin = new Thickness(0), Background = Brushes.Black });

        // 3. Methods
        var methodsPanel = new StackPanel { Margin = new Thickness(6, 3, 6, 3) };
        if (box.Methods.Count > 0)
        {
            foreach (var m in box.Methods)
            {
                methodsPanel.Children.Add(new TextBlock
                {
                    Text = m,
                    FontSize = 11,
                    FontFamily = new FontFamily("Consolas, Courier New, monospace"),
                    Foreground = Brushes.Black
                });
            }
        }
        mainPanel.Children.Add(methodsPanel);

        border.Child = mainPanel;

        Canvas.SetLeft(border, box.X1);
        Canvas.SetTop(border, box.Y1);

        BoardCanvas.Children.Add(border);

        _boxVisualsById[box.Id] = border;
        _visualToBox[border] = box;
    }

    private async void CreateLineButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!_currentUserCanDraw || _activeBoardId is null) return;
        if (StartBoxComboBox.SelectedItem is not BoxItem start || EndBoxComboBox.SelectedItem is not BoxItem end) return;
        if (start.Id == end.Id) return;

        var middleText = LineMiddleTextTextBox.Text?.Trim();

        try
        {
            var id = await _api.AddLineAsync(_activeBoardId.Value, start.Id, end.Id, middleText);
            RenderLineVisual(id, start.Id, end.Id, middleText);
            LineMiddleTextTextBox.Text = "";
        }
        catch { }
    }

    private void RenderLineVisual(Guid id, Guid startBoxId, Guid endBoxId, string? middleText)
    {
        if (_lineLinks.Any(l => l.Id == id)) return;
        if (!_boxesById.TryGetValue(startBoxId, out var start) || !_boxesById.TryGetValue(endBoxId, out var end)) return;

        var (startPt, endPt) = CalculateEdgeConnectionPoints(start, end);

        var line = new Line
        {
            Stroke = Brushes.Black,
            StrokeThickness = 2,
            StrokeLineCap = PenLineCap.Flat,
            StartPoint = startPt,
            EndPoint = endPt,
            Cursor = new Cursor(StandardCursorType.Hand)
        };
        BoardCanvas.Children.Add(line);

        TextBlock? label = null;
        if (!string.IsNullOrWhiteSpace(middleText))
        {
            label = new TextBlock
            {
                Text = middleText,
                FontWeight = FontWeight.Bold,
                FontSize = 11,
                Foreground = Brushes.Black,
                Background = Brushes.White
            };
            Canvas.SetLeft(label, (startPt.X + endPt.X) / 2d - 10);
            Canvas.SetTop(label, (startPt.Y + endPt.Y) / 2d - 8);
            BoardCanvas.Children.Add(label);
        }

        var linkItem = new LineVisualItem(id, startBoxId, endBoxId, line, label);
        _lineLinks.Add(linkItem);
    }

    #endregion

    #region Pointer & Interaction Events

    private void BoardCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(BoardCanvas);
        if (!point.Properties.IsLeftButtonPressed) return;

        var clickedVisual = FindAncestorBorder(e.Source as Visual);

        if (clickedVisual is not null && _visualToBox.TryGetValue(clickedVisual, out var clickedBox))
        {
            SelectCanvasElement(clickedBox, clickedVisual);

            if (_currentUserCanDraw)
            {
                _isMovingBox = true;
                _movingBoxVisual = clickedVisual;
                _movingBox = clickedBox;
                _moveStartPoint = e.GetPosition(BoardCanvas);
                _moveStartLeft = Canvas.GetLeft(clickedVisual);
                _moveStartTop = Canvas.GetTop(clickedVisual);
                e.Pointer.Capture(BoardCanvas);
                e.Handled = true;
            }
            return;
        }

        if (e.Source is Line clickedLine)
        {
            var lineItem = _lineLinks.FirstOrDefault(l => l.Shape == clickedLine);
            if (lineItem is not null)
            {
                SelectCanvasElement(lineItem, clickedLine);
                e.Handled = true;
                return;
            }
        }

        ClearSelection();

        if (!_currentUserCanDraw) return;

        if (_selectedTool == "ClassBox")
        {
            _dragStart = e.GetPosition(BoardCanvas);
            _isDragging = true;
            e.Pointer.Capture(BoardCanvas);

            _previewRectangle = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeDashArray = [3, 3],
                Fill = new SolidColorBrush(Color.Parse("#15000000")),
                StrokeThickness = 1
            };
            BoardCanvas.Children.Add(_previewRectangle);
        }
    }

    private void BoardCanvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isMovingBox && _movingBoxVisual is not null && _movingBox is not null)
        {
            var movePoint = e.GetPosition(BoardCanvas);
            var newLeft = _moveStartLeft + (movePoint.X - _moveStartPoint.X);
            var newTop = _moveStartTop + (movePoint.Y - _moveStartPoint.Y);
            SetBoxPosition(_movingBox, newLeft, newTop);
            UpdateConnectedLines(_movingBox.Id);
            return;
        }

        if (!_isDragging || _previewRectangle is null) return;

        var current = e.GetPosition(BoardCanvas);
        var dx = current.X - _dragStart.X;
        var dy = current.Y - _dragStart.Y;
        var width = Math.Max(10, Math.Abs(dx));
        var height = Math.Max(10, Math.Abs(dy));
        var left = dx >= 0 ? _dragStart.X : _dragStart.X - width;
        var top = dy >= 0 ? _dragStart.Y : _dragStart.Y - height;

        _previewRectangle.Width = width;
        _previewRectangle.Height = height;
        Canvas.SetLeft(_previewRectangle, left);
        Canvas.SetTop(_previewRectangle, top);
    }

    private async void BoardCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isMovingBox && _movingBoxVisual is not null && _movingBox is not null)
        {
            _isMovingBox = false;
            e.Pointer.Capture(null);

            var currentLeft = Canvas.GetLeft(_movingBoxVisual);
            var currentTop = Canvas.GetTop(_movingBoxVisual);
            var dx = (int)Math.Round(currentLeft - _moveStartLeft);
            var dy = (int)Math.Round(currentTop - _moveStartTop);
            var movingId = _movingBox.Id;

            try
            {
                if (dx != 0 || dy != 0)
                {
                    _pendingLocalMoves.Add(movingId);
                    await _api.MoveElementAsync(movingId, dx, dy);
                }
            }
            catch
            {
                _pendingLocalMoves.Remove(movingId);
                SetBoxPosition(_movingBox, _moveStartLeft, _moveStartTop);
                UpdateConnectedLines(movingId);
            }
            finally
            {
                _movingBoxVisual = null;
                _movingBox = null;
            }
            return;
        }

        if (!_isDragging || _previewRectangle is null) return;

        _isDragging = false;
        e.Pointer.Capture(null);

        var pLeft = (int)Canvas.GetLeft(_previewRectangle);
        var pTop = (int)Canvas.GetTop(_previewRectangle);
        var pWidth = (int)Math.Round(_previewRectangle.Width);
        var pHeight = (int)Math.Round(_previewRectangle.Height);

        BoardCanvas.Children.Remove(_previewRectangle);
        _previewRectangle = null;

        if (pWidth < 40 || pHeight < 40) return;

        var className = NewClassNameTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(className)) className = "NovaKlasa";

        var attributes = ParseList(NewClassAttributesTextBox.Text);
        var methods = ParseList(NewClassMethodsTextBox.Text);

        await CreateClassBoxAsync(pLeft, pTop, pLeft + pWidth, pTop + pHeight, className, attributes, methods);
    }

    private void SelectCanvasElement(object element, Visual visual)
    {
        ClearSelection();

        _selectedCanvasElement = element;
        if (element is BoxItem box && visual is Border border)
        {
            _selectedBoxVisual = border;
            border.BorderBrush = new SolidColorBrush(Color.Parse("#0000EE"));
            border.BorderThickness = new Thickness(2.5);
            StartBoxComboBox.SelectedItem = box;
        }
        else if (element is LineVisualItem lineLink && visual is Line line)
        {
            _selectedLineVisual = line;
            line.Stroke = new SolidColorBrush(Color.Parse("#0000EE"));
            line.StrokeThickness = 3.5;
        }
    }

    private void ClearSelection()
    {
        if (_selectedBoxVisual is not null)
        {
            _selectedBoxVisual.BorderBrush = Brushes.Black;
            _selectedBoxVisual.BorderThickness = new Thickness(1.5);
            _selectedBoxVisual = null;
        }

        if (_selectedLineVisual is not null)
        {
            _selectedLineVisual.Stroke = Brushes.Black;
            _selectedLineVisual.StrokeThickness = 2;
            _selectedLineVisual = null;
        }

        _selectedCanvasElement = null;
    }

    private Border? FindAncestorBorder(Visual? visual)
    {
        while (visual is not null && visual != BoardCanvas)
        {
            if (visual is Border b && _visualToBox.ContainsKey(b))
            {
                return b;
            }
            visual = visual.GetVisualParent();
        }
        return null;
    }

    private async void DeleteSelectedElementButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!_currentUserCanDraw || _selectedCanvasElement is null) return;

        Guid elementId;
        if (_selectedCanvasElement is BoxItem box) elementId = box.Id;
        else if (_selectedCanvasElement is LineVisualItem line) elementId = line.Id;
        else return;

        try
        {
            await _api.DeleteElementAsync(elementId);
            RemoveElementLocally(elementId);
        }
        catch { }
    }

    private void RemoveElementLocally(Guid elementId)
    {
        if (_boxesById.TryGetValue(elementId, out var box))
        {
            if (_boxVisualsById.TryGetValue(elementId, out var visual))
            {
                BoardCanvas.Children.Remove(visual);
                _boxVisualsById.Remove(elementId);
                _visualToBox.Remove(visual);
            }
            _boxes.Remove(box);
            _boxesById.Remove(elementId);

            var connected = _lineLinks.Where(l => l.StartBoxId == elementId || l.EndBoxId == elementId).ToList();
            foreach (var l in connected)
            {
                BoardCanvas.Children.Remove(l.Shape);
                if (l.Label is not null) BoardCanvas.Children.Remove(l.Label);
                _lineLinks.Remove(l);
            }
        }

        var lineItem = _lineLinks.FirstOrDefault(l => l.Id == elementId);
        if (lineItem is not null)
        {
            BoardCanvas.Children.Remove(lineItem.Shape);
            if (lineItem.Label is not null) BoardCanvas.Children.Remove(lineItem.Label);
            _lineLinks.Remove(lineItem);
        }

        ClearSelection();
    }

    private async void ClearBoardButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!_currentUserCanDraw || _activeBoardId is null) return;

        try
        {
            await _api.ClearBoardAsync(_activeBoardId.Value);
            ResetLocalBoard();
        }
        catch { }
    }

    private async void RefreshBoardButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_activeBoardId is null) return;
        ResetLocalBoard();
        await LoadExistingElementsAsync(_activeBoardId.Value);
    }

    private async Task LoadExistingElementsAsync(Guid boardId)
    {
        try
        {
            var res = await _api.GetElementsAsync(boardId);

            foreach (var b in res.Boxes)
            {
                if (_boxesById.ContainsKey(b.Id)) continue;
                _boxes.Add(b);
                _boxesById[b.Id] = b;
                RenderClassBoxVisual(b);
            }

            foreach (var l in res.Lines)
            {
                RenderLineVisual(l.Id, l.StartBoxId, l.EndBoxId, l.MiddleText);
            }
        }
        catch { }
    }

    private void ResetLocalBoard()
    {
        BoardCanvas.Children.Clear();
        _boxes.Clear();
        _boxesById.Clear();
        _boxVisualsById.Clear();
        _visualToBox.Clear();
        _lineLinks.Clear();
        _pendingLocalMoves.Clear();
        ClearSelection();
    }

    private void SetBoxPosition(BoxItem box, double left, double top)
    {
        if (!_boxVisualsById.TryGetValue(box.Id, out var shape)) return;

        Canvas.SetLeft(shape, left);
        Canvas.SetTop(shape, top);

        var width = box.X2 - box.X1;
        var height = box.Y2 - box.Y1;
        box.X1 = (int)Math.Round(left);
        box.Y1 = (int)Math.Round(top);
        box.X2 = box.X1 + width;
        box.Y2 = box.Y1 + height;
    }

    private void UpdateConnectedLines(Guid boxId)
    {
        foreach (var link in _lineLinks.Where(l => l.StartBoxId == boxId || l.EndBoxId == boxId))
        {
            UpdateLineGeometry(link);
        }
    }

    private void UpdateLineGeometry(LineVisualItem link)
    {
        if (!_boxesById.TryGetValue(link.StartBoxId, out var start) ||
            !_boxesById.TryGetValue(link.EndBoxId, out var end)) return;

        var (startPt, endPt) = CalculateEdgeConnectionPoints(start, end);

        link.Shape.StartPoint = startPt;
        link.Shape.EndPoint = endPt;

        if (link.Label is not null)
        {
            Canvas.SetLeft(link.Label, (startPt.X + endPt.X) / 2d - 10);
            Canvas.SetTop(link.Label, (startPt.Y + endPt.Y) / 2d - 8);
        }
    }

    private static (Point Start, Point End) CalculateEdgeConnectionPoints(BoxItem start, BoxItem end)
    {
        var startW = Math.Max(140, start.X2 - start.X1);
        var startH = Math.Max(90, start.Y2 - start.Y1);
        var startCx = start.X1 + startW / 2.0;
        var startCy = start.Y1 + startH / 2.0;

        var endW = Math.Max(140, end.X2 - end.X1);
        var endH = Math.Max(90, end.Y2 - end.Y1);
        var endCx = end.X1 + endW / 2.0;
        var endCy = end.Y1 + endH / 2.0;

        var dx = endCx - startCx;
        var dy = endCy - startCy;

        var startPt = GetBoxEdgeIntersection(startCx, startCy, startW / 2.0, startH / 2.0, dx, dy);
        var endPt = GetBoxEdgeIntersection(endCx, endCy, endW / 2.0, endH / 2.0, -dx, -dy);

        return (startPt, endPt);
    }

    private static Point GetBoxEdgeIntersection(double cx, double cy, double halfW, double halfH, double dx, double dy)
    {
        if (Math.Abs(dx) < 0.0001 && Math.Abs(dy) < 0.0001)
        {
            return new Point(cx, cy);
        }

        var absDx = Math.Abs(dx);
        var absDy = Math.Abs(dy);

        var tx = absDx > 0.0001 ? halfW / absDx : double.MaxValue;
        var ty = absDy > 0.0001 ? halfH / absDy : double.MaxValue;

        var t = Math.Min(tx, ty);

        return new Point(cx + t * dx, cy + t * dy);
    }

    private static List<string> ParseList(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return [];
        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }

    #endregion

    private sealed class LineVisualItem(Guid id, Guid startBoxId, Guid endBoxId, Line shape, TextBlock? label)
    {
        public Guid Id { get; init; } = id;
        public Guid StartBoxId { get; init; } = startBoxId;
        public Guid EndBoxId { get; init; } = endBoxId;
        public Line Shape { get; init; } = shape;
        public TextBlock? Label { get; init; } = label;
    }
}
