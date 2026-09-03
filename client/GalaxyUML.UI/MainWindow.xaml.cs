using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.AspNetCore.SignalR.Client;

namespace GalaxyUML.UI;

public partial class MainWindow : Window
{
    // Collections
    private readonly ObservableCollection<TeamViewItem> _teams = [];
    private readonly ObservableCollection<TeamMemberViewItem> _teamMembers = [];
    private readonly ObservableCollection<MeetingViewItem> _meetings = [];
    private readonly ObservableCollection<MeetingParticipantViewItem> _meetingParticipants = [];
    private readonly ObservableCollection<ChatMessageViewItem> _chatMessages = [];
    private readonly ObservableCollection<BoxViewItem> _boxes = [];

    // Fast lookups
    private readonly Dictionary<Guid, BoxViewItem> _boxesById = [];
    private readonly Dictionary<Guid, Border> _boxVisualsById = [];
    private readonly Dictionary<Border, BoxViewItem> _visualToBox = [];
    private readonly List<LineViewItem> _lineLinks = [];
    private readonly HashSet<Guid> _pendingLocalMoves = [];

    // Session state
    private Guid? _currentUserId;
    private string? _currentUsername;
    private string? _jwtToken;
    private Guid? _activeMeetingId;
    private Guid? _activeBoardId;
    private bool _currentUserCanDraw;
    private HubConnection? _hubConnection;

    // Selected element on canvas
    private object? _selectedCanvasElement; // BoxViewItem or LineViewItem
    private Border? _selectedBoxVisual;
    private Line? _selectedLineVisual;

    // Tool & Drag state
    private string _selectedTool = "ClassBox";
    private Point _dragStart;
    private Rectangle? _previewRectangle;
    private bool _isDragging;

    // Box movement state
    private bool _isMovingBox;
    private Border? _movingBoxVisual;
    private BoxViewItem? _movingBox;
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

        UpdateSessionUI();
        UpdateDrawPermissionUI();
        SetToolControlsVisibility();
    }

    #region Auth Operations

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        await PerformLoginAsync();
    }

    private async void LoginInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await PerformLoginAsync();
        }
    }

    private async Task PerformLoginAsync()
    {
        var username = LoginUsernameTextBox.Text.Trim();
        var password = LoginPasswordBox.Password;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Unesite korisničko ime i lozinku.", "Prijava", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var payload = new { username, password };
            var response = await PostAndReadAsync<LoginResponse>("/api/auth/login", payload);

            _jwtToken = response.Token;
            _currentUserId = response.User.IdUser;
            _currentUsername = response.User.Username;

            UpdateSessionUI();
            AuthStatusBanner.Text = $"Uspešno ste prijavljeni kao '{_currentUsername}'.";

            await RefreshLoggedInUserTeamsAsync();

            // Prebaci na tab sa timovima
            MainTabControl.SelectedIndex = 1;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri prijavi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var firstName = RegisterFirstNameTextBox.Text.Trim();
        var lastName = RegisterLastNameTextBox.Text.Trim();
        var username = RegisterUsernameTextBox.Text.Trim();
        var email = RegisterEmailTextBox.Text.Trim();
        var password = RegisterPasswordBox.Password;

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Sva polja su obavezna za registraciju.", "Registracija", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var payload = new { firstName, lastName, username, email, password };
            await PostAsync("/api/auth/register", payload);

            RegisterFirstNameTextBox.Clear();
            RegisterLastNameTextBox.Clear();
            RegisterUsernameTextBox.Clear();
            RegisterEmailTextBox.Clear();
            RegisterPasswordBox.Clear();

            // Popuni login formu radi lakše prijave
            LoginUsernameTextBox.Text = username;
            LoginPasswordBox.Password = password;

            MessageBox.Show("Registracija je uspešna! Sada se možete prijaviti.", "Registracija", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri registraciji", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void LogoutButton_Click(object sender, RoutedEventArgs e)
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

        _jwtToken = null;
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
            LogoutButton.Visibility = Visibility.Visible;
        }
        else
        {
            SessionStatusText.Text = "Niste prijavljeni";
            SessionStatusText.Foreground = Brushes.Black;
            UserBadgeBorder.Background = Brushes.White;
            UserBadgeBorder.BorderBrush = Brushes.Black;
            LogoutButton.Visibility = Visibility.Collapsed;
        }
    }

    #endregion

    #region Team Operations

    private async void RefreshTeamsButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshLoggedInUserTeamsAsync();
    }

    private async Task RefreshLoggedInUserTeamsAsync()
    {
        if (_currentUserId is null) return;

        try
        {
            var teams = await GetAsync<IReadOnlyCollection<TeamResponse>>("/api/teams/me");
            _teams.Clear();
            foreach (var team in teams)
            {
                _teams.Add(new TeamViewItem(team.Id, team.TeamName, team.TeamCode, team.OwnerId, team.MemberCount));
            }

            MeetingTeamComboBox.Items.Refresh();
            TeamsListBox.Items.Refresh();
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška pri osvežavanju timova: {ex.Message}";
        }
    }

    private async void CreateTeamButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentUserId is null)
        {
            MessageBox.Show("Prvo se prijavite.", "Tim", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var teamName = CreateTeamNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(teamName))
        {
            MessageBox.Show("Unesite naziv tima.", "Tim", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var created = await PostAndReadAsync<TeamResponse>("/api/teams", new { teamName });
            CreateTeamNameTextBox.Clear();
            TeamStatusText.Text = $"Kreiran tim '{created.TeamName}'. Kod tima: {created.TeamCode}";
            await RefreshLoggedInUserTeamsAsync();

            var createdItem = _teams.FirstOrDefault(t => t.Id == created.Id);
            if (createdItem is not null)
            {
                TeamsListBox.SelectedItem = createdItem;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri kreiranju tima", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void FindTeamByCodeButton_Click(object sender, RoutedEventArgs e)
    {
        var code = JoinCodeTextBox.Text.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code) || code.Length != 6)
        {
            MessageBox.Show("Unesite tačan kod tima od 6 karaktera.", "Pronalaženje tima", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var found = await GetAsync<TeamResponse>($"/api/teams/by-code/{code}");
            TeamStatusText.Text = $"Pronađen tim: '{found.TeamName}' (Broj članova: {found.MemberCount}).";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Tim nije pronađen", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void JoinByCodeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentUserId is null)
        {
            MessageBox.Show("Prvo se prijavite.", "Učlanjenje", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var code = JoinCodeTextBox.Text.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code) || code.Length != 6)
        {
            MessageBox.Show("Unesite tačan kod tima od 6 karaktera.", "Učlanjenje", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var joined = await PostAndReadAsync<TeamResponse>("/api/teams/join-by-code", new { joinCode = code });
            JoinCodeTextBox.Clear();
            TeamStatusText.Text = $"Uspešno ste se učlanili u tim '{joined.TeamName}'.";
            await RefreshLoggedInUserTeamsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri učlanjenju", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void TeamsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamViewItem selectedTeam)
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
            var members = await GetAsync<IReadOnlyCollection<TeamMemberResponse>>($"/api/teams/{teamId}/members");
            _teamMembers.Clear();
            foreach (var m in members)
            {
                _teamMembers.Add(new TeamMemberViewItem(m.UserId, m.Username, m.Email, m.FirstName, m.LastName, m.Role, m.JoinedAt));
            }
        }
        catch (Exception ex)
        {
            TeamStatusText.Text = $"Greška pri učitavanju članova: {ex.Message}";
        }
    }

    private async void ChangeRoleButton_Click(object sender, RoutedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamViewItem selectedTeam ||
            TeamMembersListBox.SelectedItem is not TeamMemberViewItem selectedMember)
        {
            MessageBox.Show("Selektujte tim i člana čiju ulogu menjate.", "Promena uloge", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (ChangeRoleComboBox.SelectedItem is not ComboBoxItem item || item.Content is not string newRole)
        {
            return;
        }

        try
        {
            await PostAsync($"/api/teams/{selectedTeam.Id}/role", new
            {
                targetUserId = selectedMember.UserId,
                role = newRole
            });

            MessageBox.Show($"Uloga člana {selectedMember.Username} uspešno promenjena u {newRole}.", "Uloga promenjena", MessageBoxButton.OK, MessageBoxImage.Information);
            await RefreshTeamMembersAsync(selectedTeam.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri promeni uloge", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BanMemberButton_Click(object sender, RoutedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamViewItem selectedTeam ||
            TeamMembersListBox.SelectedItem is not TeamMemberViewItem selectedMember)
        {
            MessageBox.Show("Selektujte tim i člana koga želite da banujete.", "Banovanje člana", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show($"Da li ste sigurni da želite da banujete člana '{selectedMember.Username}' iz tima '{selectedTeam.TeamName}'?",
            "Potvrda bana", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
        {
            return;
        }

        var reason = BanReasonTextBox.Text.Trim();
        try
        {
            await PostAsync($"/api/teams/{selectedTeam.Id}/ban", new
            {
                targetUserId = selectedMember.UserId,
                reason = string.IsNullOrWhiteSpace(reason) ? null : reason
            });

            BanReasonTextBox.Clear();
            MessageBox.Show($"Član {selectedMember.Username} je uspešno banovan.", "Ban", MessageBoxButton.OK, MessageBoxImage.Information);
            await RefreshTeamMembersAsync(selectedTeam.Id);
            await RefreshLoggedInUserTeamsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri banovanju", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void LeaveTeamButton_Click(object sender, RoutedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamViewItem selectedTeam)
        {
            MessageBox.Show("Selektujte tim koji želite da napustite.", "Napuštanje tima", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show($"Da li ste sigurni da želite da napustite tim '{selectedTeam.TeamName}'?",
            "Potvrda napuštanja", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await PostAsync($"/api/teams/{selectedTeam.Id}/leave", new { });
            MessageBox.Show($"Napustili ste tim '{selectedTeam.TeamName}'.", "Tim napušten", MessageBoxButton.OK, MessageBoxImage.Information);
            _teamMembers.Clear();
            await RefreshLoggedInUserTeamsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri napuštanju tima", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void DeleteTeamButton_Click(object sender, RoutedEventArgs e)
    {
        if (TeamsListBox.SelectedItem is not TeamViewItem selectedTeam)
        {
            MessageBox.Show("Selektujte tim koji želite da obrišete.", "Brisanje tima", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show($"Da li ste sigurni da želite trajno da OBRIŠETE tim '{selectedTeam.TeamName}'?",
            "Potvrda brisanja tima", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            using var client = CreateClient();
            var response = await client.DeleteAsync($"/api/teams/{selectedTeam.Id}");
            await EnsureSuccess(response);

            MessageBox.Show($"Tim '{selectedTeam.TeamName}' je uspešno obrisan.", "Tim obrisan", MessageBoxButton.OK, MessageBoxImage.Information);
            _teamMembers.Clear();
            await RefreshLoggedInUserTeamsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri brisanju tima", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #endregion

    #region Meeting Operations

    private async void MeetingTeamComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MeetingTeamComboBox.SelectedItem is not TeamViewItem selectedTeam)
        {
            _meetings.Clear();
            return;
        }

        await RefreshActiveMeetingsForTeamAsync(selectedTeam.Id);
    }

    private async void RefreshMeetingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (MeetingTeamComboBox.SelectedItem is TeamViewItem selectedTeam)
        {
            await RefreshActiveMeetingsForTeamAsync(selectedTeam.Id);
        }
    }

    private async Task RefreshActiveMeetingsForTeamAsync(Guid teamId)
    {
        try
        {
            var meeting = await GetAsyncNullable<MeetingStartedResponse>($"/api/meetings/by-team/{teamId}/active");
            _meetings.Clear();

            if (meeting is not null)
            {
                var item = new MeetingViewItem(meeting.MeetingId, meeting.TeamId, meeting.BoardId, meeting.StartedAtUtc);
                _meetings.Add(item);
                MeetingsListBox.SelectedItem = item;
            }
        }
        catch (Exception ex)
        {
            MeetingStatusText.Text = $"Greška: {ex.Message}";
        }
    }

    private async void StartMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentUserId is null)
        {
            MessageBox.Show("Prvo se prijavite.", "Sastanak", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MeetingTeamComboBox.SelectedItem is not TeamViewItem selectedTeam)
        {
            MessageBox.Show("Izaberite tim za pokretanje sastanka.", "Sastanak", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var created = await PostAndReadAsync<MeetingStartedResponse>("/api/meetings", new
            {
                teamId = selectedTeam.Id
            });

            _activeMeetingId = created.MeetingId;
            _activeBoardId = created.BoardId;
            _currentUserCanDraw = true; // Organizer starts with draw rights

            BoardInfoText.Text = $"Board ID: {created.BoardId}";
            MeetingStatusText.Text = $"Sastanak aktivan: {created.MeetingId}";

            var meetingItem = new MeetingViewItem(created.MeetingId, created.TeamId, created.BoardId, created.StartedAtUtc);
            _meetings.Clear();
            _meetings.Add(meetingItem);
            MeetingsListBox.SelectedItem = meetingItem;

            ResetLocalBoard();
            UpdateDrawPermissionUI();

            await RefreshMeetingParticipantsAsync(created.MeetingId);
            await LoadChatHistoryAsync(created.MeetingId);
            await ConnectToSignalRAsync(created.MeetingId);

            MainTabControl.SelectedItem = DiagramTabItem;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri startovanju sastanka", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void JoinMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentUserId is null)
        {
            MessageBox.Show("Prvo se prijavite.", "Sastanak", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MeetingsListBox.SelectedItem is not MeetingViewItem selectedMeeting)
        {
            MessageBox.Show("Izaberite aktivan sastanak sa liste.", "Sastanak", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await PostAsync($"/api/meetings/{selectedMeeting.Id}/join", new { });

            _activeMeetingId = selectedMeeting.Id;
            _activeBoardId = selectedMeeting.BoardId;

            BoardInfoText.Text = $"Board ID: {selectedMeeting.BoardId}";
            MeetingStatusText.Text = $"Pridruženi ste sastanku: {selectedMeeting.Id}";

            ResetLocalBoard();

            await RefreshMeetingParticipantsAsync(selectedMeeting.Id);
            await LoadChatHistoryAsync(selectedMeeting.Id);
            await ConnectToSignalRAsync(selectedMeeting.Id);

            MainTabControl.SelectedItem = DiagramTabItem;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri pridruživanju", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void EndMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        if (MeetingsListBox.SelectedItem is not MeetingViewItem selectedMeeting)
        {
            MessageBox.Show("Izaberite sastanak koji želite da završite.", "Završetak sastanka", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show("Da li ste sigurni da želite da završite ovaj sastanak za sve učesnike?",
            "Završi sastanak", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await PostAsync($"/api/meetings/{selectedMeeting.Id}/end", new { });

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

                if (_hubConnection is not null)
                {
                    try { await _hubConnection.StopAsync(); } catch { }
                    _hubConnection = null;
                    SignalRStatusText.Text = "SignalR: Isključen";
                }
            }

            MeetingStatusText.Text = "Sastanak je završen.";
            BoardInfoText.Text = "Board: Nije aktivan";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri završetku sastanka", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void MeetingsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MeetingsListBox.SelectedItem is not MeetingViewItem selectedMeeting) return;

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
            var participants = await GetAsync<IReadOnlyCollection<MeetingParticipantResponse>>($"/api/meetings/{meetingId}/participants");
            _meetingParticipants.Clear();

            var currentCanDraw = false;
            foreach (var p in participants)
            {
                _meetingParticipants.Add(new MeetingParticipantViewItem(p.UserId, p.Username, p.Role, p.CanDraw, p.JoinedAtUtc));
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

    private async void GrantDrawButton_Click(object sender, RoutedEventArgs e)
    {
        await ToggleDrawPermissionAsync(true);
    }

    private async void RevokeDrawButton_Click(object sender, RoutedEventArgs e)
    {
        await ToggleDrawPermissionAsync(false);
    }

    private async Task ToggleDrawPermissionAsync(bool canDraw)
    {
        if (_activeMeetingId is null)
        {
            MessageBox.Show("Nema aktivnog sastanka.", "Prava crtanja", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MeetingParticipantsListBox.SelectedItem is not MeetingParticipantViewItem selectedParticipant)
        {
            MessageBox.Show("Selektujte učesnika sa liste za promenu prava crtanja.", "Prava crtanja", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await PostAsync($"/api/meetings/{_activeMeetingId}/grant-draw", new
            {
                targetId = selectedParticipant.UserId,
                canDraw
            });

            await RefreshMeetingParticipantsAsync(_activeMeetingId.Value);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri promeni prava crtanja", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SwitchToDiagramTabButton_Click(object sender, RoutedEventArgs e)
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
            DrawPermissionBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F4F4"));
            DrawPermissionBadge.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#767676"));
            DrawPermissionStatusText.Text = "Samo pregled";
            DrawPermissionStatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"));
        }
    }

    #endregion

    #region Real-time SignalR Integration

    private async Task ConnectToSignalRAsync(Guid meetingId)
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

        var baseUrl = ApiBaseUrlTextBox.Text.Trim().TrimEnd('/');
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{baseUrl}/diagramHub", options =>
            {
                if (!string.IsNullOrWhiteSpace(_jwtToken))
                {
                    options.AccessTokenProvider = () => Task.FromResult(_jwtToken)!;
                }
            })
            .WithAutomaticReconnect()
            .Build();

        // 1. ClassBox Added
        _hubConnection.On<Guid, int, int, int, int, IReadOnlyCollection<string>, IReadOnlyCollection<string>>(
            "ClassBoxAdded", (id, x1, y1, x2, y2, attributes, methods) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (_boxesById.ContainsKey(id)) return;

                var box = new BoxViewItem(id, x1, y1, x2, y2, "ClassBox", attributes ?? [], methods ?? []);
                _boxes.Add(box);
                _boxesById[id] = box;

                RenderClassBoxVisual(box);
                StartBoxComboBox.Items.Refresh();
                EndBoxComboBox.Items.Refresh();
            });
        });

        // 2. Element Moved
        _hubConnection.On<Guid, int, int>("ElementMoved", (elementId, dx, dy) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (_pendingLocalMoves.Remove(elementId))
                {
                    // This was triggered by our own move, position is already accurate!
                    return;
                }

                if (_boxesById.TryGetValue(elementId, out var box))
                {
                    var newLeft = box.X1 + dx;
                    var newTop = box.Y1 + dy;
                    SetBoxPosition(box, newLeft, newTop);
                    UpdateConnectedLines(elementId);
                }
            });
        });

        // 3. Line Added
        _hubConnection.On<Guid, Guid, Guid, string?, string?, string?>(
            "LineAdded", (id, startBoxId, endBoxId, middleText, _, _) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (_lineLinks.Any(l => l.Id == id)) return;
                RenderLineVisual(id, startBoxId, endBoxId, middleText);
            });
        });

        // 4. Element Deleted
        _hubConnection.On<Guid>("ElementDeleted", (elementId) =>
        {
            Dispatcher.Invoke(() =>
            {
                RemoveElementLocally(elementId);
            });
        });

        // 5. Board Cleared
        _hubConnection.On<Guid>("BoardCleared", (boardId) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (_activeBoardId == boardId)
                {
                    ResetLocalBoard();
                }
            });
        });

        // 6. Real-time Chat Message
        _hubConnection.On<Guid, Guid, string, string, DateTime>(
            "ChatMessageReceived", (id, senderId, senderUsername, content, sentAtUtc) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (_chatMessages.Any(m => m.Id == id)) return;
                _chatMessages.Add(new ChatMessageViewItem(id, senderId, senderUsername, content, sentAtUtc));
                ChatMessagesListBox.ScrollIntoView(_chatMessages.Last());
            });
        });

        // 7. Draw Permission Changed
        _hubConnection.On<Guid, bool>("DrawPermissionChanged", (targetUserId, canDraw) =>
        {
            Dispatcher.Invoke(() =>
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
                    MeetingParticipantsListBox.Items.Refresh();
                }
            });
        });

        // 8. Participants Updated
        _hubConnection.On<Guid>("ParticipantsUpdated", (updatedMeetingId) =>
        {
            Dispatcher.Invoke(async () =>
            {
                if (_activeMeetingId == updatedMeetingId)
                {
                    await RefreshMeetingParticipantsAsync(updatedMeetingId);
                }
            });
        });

        // 9. Meeting Ended
        _hubConnection.On<Guid>("MeetingEnded", (endedMeetingId) =>
        {
            Dispatcher.Invoke(() =>
            {
                if (_activeMeetingId == endedMeetingId)
                {
                    MessageBox.Show("Organizator je završio ovaj sastanak.", "Sastanak završen", MessageBoxButton.OK, MessageBoxImage.Information);
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
        });

        try
        {
            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("JoinMeeting", meetingId);

            SignalRStatusText.Text = "SignalR: [Povezan]";
            SignalRBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6F4EA"));
            SignalRBadge.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#137333"));
            SignalRStatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#137333"));

            if (_activeBoardId.HasValue)
            {
                await LoadExistingElementsAsync(_activeBoardId.Value);
            }
        }
        catch (Exception)
        {
            SignalRStatusText.Text = "SignalR: [Greška]";
            SignalRBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCE8E6"));
            SignalRBadge.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C5221F"));
            SignalRStatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C5221F"));
        }
    }

    #endregion

    #region Chat Operations

    private async Task LoadChatHistoryAsync(Guid meetingId)
    {
        try
        {
            var messages = await GetAsync<IReadOnlyCollection<ChatMessageResponse>>($"/api/meetings/{meetingId}/messages");
            _chatMessages.Clear();
            foreach (var m in messages)
            {
                _chatMessages.Add(new ChatMessageViewItem(m.Id, m.SenderId, m.SenderUsername, m.Content, m.SentAtUtc));
            }

            if (_chatMessages.Count > 0)
            {
                ChatMessagesListBox.ScrollIntoView(_chatMessages.Last());
            }
        }
        catch { }
    }

    private async void SendChatMessageButton_Click(object sender, RoutedEventArgs e)
    {
        await SendCurrentChatMessageAsync();
    }

    private async void ChatMessageInputTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await SendCurrentChatMessageAsync();
        }
    }

    private async Task SendCurrentChatMessageAsync()
    {
        if (_activeMeetingId is null)
        {
            MessageBox.Show("Niste u aktivnom sastanku.", "Chat", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var content = ChatMessageInputTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(content)) return;

        try
        {
            ChatMessageInputTextBox.Clear();
            await PostAsync($"/api/meetings/{_activeMeetingId}/message", new { content });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri slanju poruke", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #endregion

    #region Canvas UML Diagramming Operations

    private void ToolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ToolComboBox.SelectedItem is ComboBoxItem item && item.Content is string tool)
        {
            _selectedTool = tool;
            SetToolControlsVisibility();
        }
    }

    private void SetToolControlsVisibility()
    {
        if (ClassBoxCreationWrap is null || LineControlsWrap is null) return;

        ClassBoxCreationWrap.Visibility = _selectedTool == "ClassBox" ? Visibility.Visible : Visibility.Collapsed;
        LineControlsWrap.Visibility = _selectedTool == "Line" ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void AddClassBoxDirectButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_currentUserCanDraw)
        {
            MessageBox.Show("Nemate dozvolu za crtanje.", "ClassBox", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_activeBoardId is null)
        {
            MessageBox.Show("Nema aktivnog board-a. Prvo pokrenite ili se pridružite sastanku.", "ClassBox", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var className = NewClassNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(className)) className = "NovaKlasa";

        var attributes = ParseList(NewClassAttributesTextBox.Text);
        var methods = ParseList(NewClassMethodsTextBox.Text);

        // Standard size and position in center view
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
            var created = await PostAndReadAsync<ElementCreateResponse>($"/api/diagram/{_activeBoardId}/class-box", new
            {
                x1,
                y1,
                x2,
                y2,
                attributes,
                methods
            });

            var box = new BoxViewItem(created.Id, x1, y1, x2, y2, className, attributes, methods);
            if (!_boxesById.ContainsKey(box.Id))
            {
                _boxes.Add(box);
                _boxesById[box.Id] = box;
                RenderClassBoxVisual(box);
                StartBoxComboBox.Items.Refresh();
                EndBoxComboBox.Items.Refresh();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri kreiranju ClassBox-a", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RenderClassBoxVisual(BoxViewItem box)
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
            Cursor = Cursors.Hand
        };

        var mainPanel = new StackPanel();

        // 1. Header (Class Name)
        var headerBorder = new Border
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0")),
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(6, 4, 6, 4)
        };
        var headerText = new TextBlock
        {
            Text = box.ClassName,
            FontWeight = FontWeights.Bold,
            FontSize = 13,
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = Brushes.Black
        };
        headerBorder.Child = headerText;
        mainPanel.Children.Add(headerBorder);

        // 2. Attributes Section
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
        else
        {
            attributesPanel.Children.Add(new TextBlock { Text = " ", FontSize = 8 });
        }
        mainPanel.Children.Add(attributesPanel);

        // Separator
        mainPanel.Children.Add(new Separator { Margin = new Thickness(0), Background = Brushes.Black });

        // 3. Methods Section
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
        else
        {
            methodsPanel.Children.Add(new TextBlock { Text = " ", FontSize = 8 });
        }
        mainPanel.Children.Add(methodsPanel);

        border.Child = mainPanel;

        Panel.SetZIndex(border, 10);
        Canvas.SetLeft(border, box.X1);
        Canvas.SetTop(border, box.Y1);

        BoardCanvas.Children.Add(border);

        _boxVisualsById[box.Id] = border;
        _visualToBox[border] = box;
    }

    private async void CreateLineButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_currentUserCanDraw)
        {
            MessageBox.Show("Nemate dozvolu za crtanje.", "Line", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_activeBoardId is null)
        {
            MessageBox.Show("Nema aktivnog board-a.", "Line", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (StartBoxComboBox.SelectedItem is not BoxViewItem start || EndBoxComboBox.SelectedItem is not BoxViewItem end)
        {
            MessageBox.Show("Izaberite start i end klasu za liniju.", "Line", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (start.Id == end.Id)
        {
            MessageBox.Show("Start i end klasa moraju biti različite.", "Line", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var middleText = LineMiddleTextTextBox.Text.Trim();

        try
        {
            var created = await PostAndReadAsync<ElementCreateResponse>($"/api/diagram/{_activeBoardId}/line", new
            {
                startBoxId = start.Id,
                endBoxId = end.Id,
                middleText = string.IsNullOrWhiteSpace(middleText) ? null : middleText,
                text1 = (string?)null,
                text2 = (string?)null
            });

            RenderLineVisual(created.Id, start.Id, end.Id, middleText);
            LineMiddleTextTextBox.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri kreiranju linije", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
            StrokeStartLineCap = PenLineCap.Flat,
            StrokeEndLineCap = PenLineCap.Flat,
            X1 = startPt.X,
            Y1 = startPt.Y,
            X2 = endPt.X,
            Y2 = endPt.Y,
            Cursor = Cursors.Hand
        };
        Panel.SetZIndex(line, 5);
        BoardCanvas.Children.Add(line);

        TextBlock? label = null;
        if (!string.IsNullOrWhiteSpace(middleText))
        {
            label = new TextBlock
            {
                Text = middleText,
                FontWeight = FontWeights.Bold,
                FontSize = 11,
                Foreground = Brushes.Black,
                Background = Brushes.White
            };
            Panel.SetZIndex(label, 6);
            Canvas.SetLeft(label, (startPt.X + endPt.X) / 2d - 10);
            Canvas.SetTop(label, (startPt.Y + endPt.Y) / 2d - 8);
            BoardCanvas.Children.Add(label);
        }

        var linkItem = new LineViewItem(id, startBoxId, endBoxId, line, label);
        _lineLinks.Add(linkItem);
    }

    #endregion

    #region Canvas Mouse & Interaction Events

    private void BoardCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var clickedVisual = FindAncestorBorder(e.OriginalSource as DependencyObject);

        // Selection & Moving of ClassBox
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
                BoardCanvas.CaptureMouse();
                e.Handled = true;
            }
            return;
        }

        // Line click selection
        if (e.OriginalSource is Line clickedLine)
        {
            var lineItem = _lineLinks.FirstOrDefault(l => l.Shape == clickedLine);
            if (lineItem is not null)
            {
                SelectCanvasElement(lineItem, clickedLine);
                e.Handled = true;
                return;
            }
        }

        // Click on blank canvas -> Deselect
        ClearSelection();

        if (!_currentUserCanDraw) return;

        // Drag to create ClassBox
        if (_selectedTool == "ClassBox")
        {
            _dragStart = e.GetPosition(BoardCanvas);
            _isDragging = true;
            BoardCanvas.CaptureMouse();

            _previewRectangle = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeDashArray = [3, 3],
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#15000000")),
                StrokeThickness = 1
            };
            Panel.SetZIndex(_previewRectangle, 20);
            BoardCanvas.Children.Add(_previewRectangle);
        }
    }

    private void BoardCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isMovingBox && _movingBoxVisual is not null && _movingBox is not null)
        {
            var movePoint = e.GetPosition(BoardCanvas);
            var newLeft = _moveStartLeft + (movePoint.X - _moveStartPoint.X);
            var newTop = _moveStartTop + (movePoint.Y - _moveStartTop);
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

    private async void BoardCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        // 1. Finish Moving Box
        if (_isMovingBox && _movingBoxVisual is not null && _movingBox is not null)
        {
            _isMovingBox = false;
            BoardCanvas.ReleaseMouseCapture();

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
                    await PostAsync($"/api/diagram/{movingId}/move", new { dx, dy });
                }
            }
            catch (Exception ex)
            {
                _pendingLocalMoves.Remove(movingId);
                SetBoxPosition(_movingBox, _moveStartLeft, _moveStartTop);
                UpdateConnectedLines(movingId);
                MessageBox.Show(ex.Message, "Greška pri pomeranju", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                _movingBoxVisual = null;
                _movingBox = null;
            }
            return;
        }

        // 2. Finish Drag to Create ClassBox
        if (!_isDragging || _previewRectangle is null) return;

        _isDragging = false;
        BoardCanvas.ReleaseMouseCapture();

        var pLeft = (int)Canvas.GetLeft(_previewRectangle);
        var pTop = (int)Canvas.GetTop(_previewRectangle);
        var pWidth = (int)Math.Round(_previewRectangle.Width);
        var pHeight = (int)Math.Round(_previewRectangle.Height);

        BoardCanvas.Children.Remove(_previewRectangle);
        _previewRectangle = null;

        if (pWidth < 40 || pHeight < 40) return;

        var className = NewClassNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(className)) className = "NovaKlasa";

        var attributes = ParseList(NewClassAttributesTextBox.Text);
        var methods = ParseList(NewClassMethodsTextBox.Text);

        await CreateClassBoxAsync(pLeft, pTop, pLeft + pWidth, pTop + pHeight, className, attributes, methods);
    }

    private void SelectCanvasElement(object element, DependencyObject visual)
    {
        ClearSelection();

        _selectedCanvasElement = element;
        if (element is BoxViewItem box && visual is Border border)
        {
            _selectedBoxVisual = border;
            border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000EE"));
            border.BorderThickness = new Thickness(2.5);
            StartBoxComboBox.SelectedItem = box;
        }
        else if (element is LineViewItem lineLink && visual is Line line)
        {
            _selectedLineVisual = line;
            line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0000EE"));
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

    private Border? FindAncestorBorder(DependencyObject? obj)
    {
        while (obj is not null && obj != BoardCanvas)
        {
            if (obj is Border b && _visualToBox.ContainsKey(b))
            {
                return b;
            }
            obj = VisualTreeHelper.GetParent(obj);
        }
        return null;
    }

    private async void DeleteSelectedElementButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_currentUserCanDraw)
        {
            MessageBox.Show("Nemate dozvolu za izmenu table.", "Brisanje", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_selectedCanvasElement is null)
        {
            MessageBox.Show("Prvo kliknite na element (ClassBox ili Line) koji želite da obrišete.", "Brisanje", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        Guid elementId;
        if (_selectedCanvasElement is BoxViewItem box)
        {
            elementId = box.Id;
        }
        else if (_selectedCanvasElement is LineViewItem line)
        {
            elementId = line.Id;
        }
        else return;

        try
        {
            using var client = CreateClient();
            var response = await client.DeleteAsync($"/api/diagram/{elementId}");
            await EnsureSuccess(response);

            RemoveElementLocally(elementId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri brisanju elementa", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RemoveElementLocally(Guid elementId)
    {
        // 1. If Box
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

            // Also remove connected lines
            var connected = _lineLinks.Where(l => l.StartBoxId == elementId || l.EndBoxId == elementId).ToList();
            foreach (var l in connected)
            {
                BoardCanvas.Children.Remove(l.Shape);
                if (l.Label is not null) BoardCanvas.Children.Remove(l.Label);
                _lineLinks.Remove(l);
            }

            StartBoxComboBox.Items.Refresh();
            EndBoxComboBox.Items.Refresh();
        }

        // 2. If Line
        var lineItem = _lineLinks.FirstOrDefault(l => l.Id == elementId);
        if (lineItem is not null)
        {
            BoardCanvas.Children.Remove(lineItem.Shape);
            if (lineItem.Label is not null) BoardCanvas.Children.Remove(lineItem.Label);
            _lineLinks.Remove(lineItem);
        }

        ClearSelection();
    }

    private async void ClearBoardButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_currentUserCanDraw)
        {
            MessageBox.Show("Nemate dozvolu za brisanje table.", "Čišćenje table", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_activeBoardId is null)
        {
            MessageBox.Show("Nema aktivnog board-a.", "Čišćenje table", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show("Da li ste sigurni da želite da OBRIŠETE SVE elemente sa table?",
            "Čišćenje table", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await PostAsync($"/api/diagram/{_activeBoardId}/clear", new { });
            ResetLocalBoard();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Greška pri čišćenju table", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void RefreshBoardButton_Click(object sender, RoutedEventArgs e)
    {
        if (_activeBoardId is null) return;
        ResetLocalBoard();
        await LoadExistingElementsAsync(_activeBoardId.Value);
    }

    private async Task LoadExistingElementsAsync(Guid boardId)
    {
        try
        {
            var response = await GetAsync<BoardElementsResponse>($"/api/diagram/{boardId}/elements");

            // Load ClassBoxes
            foreach (var cb in response.ClassBoxes)
            {
                if (_boxesById.ContainsKey(cb.Id)) continue;
                var box = new BoxViewItem(cb.Id, cb.X1, cb.Y1, cb.X2, cb.Y2, "ClassBox", cb.Attributes ?? [], cb.Methods ?? []);
                _boxes.Add(box);
                _boxesById[box.Id] = box;
                RenderClassBoxVisual(box);
            }

            // Load plain Boxes
            foreach (var b in response.Boxes)
            {
                if (_boxesById.ContainsKey(b.Id)) continue;
                var box = new BoxViewItem(b.Id, b.X1, b.Y1, b.X2, b.Y2, "Box", [], []);
                _boxes.Add(box);
                _boxesById[box.Id] = box;
                RenderClassBoxVisual(box);
            }

            // Load Lines
            foreach (var line in response.Lines)
            {
                RenderLineVisual(line.Id, line.StartBoxId, line.EndBoxId, line.MiddleText);
            }

            StartBoxComboBox.Items.Refresh();
            EndBoxComboBox.Items.Refresh();
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

    private void SetBoxPosition(BoxViewItem box, double left, double top)
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

    private void UpdateLineGeometry(LineViewItem link)
    {
        if (!_boxesById.TryGetValue(link.StartBoxId, out var start) ||
            !_boxesById.TryGetValue(link.EndBoxId, out var end)) return;

        var (startPt, endPt) = CalculateEdgeConnectionPoints(start, end);

        link.Shape.X1 = startPt.X;
        link.Shape.Y1 = startPt.Y;
        link.Shape.X2 = endPt.X;
        link.Shape.Y2 = endPt.Y;

        if (link.Label is not null)
        {
            Canvas.SetLeft(link.Label, (startPt.X + endPt.X) / 2d - 10);
            Canvas.SetTop(link.Label, (startPt.Y + endPt.Y) / 2d - 8);
        }
    }

    private static (Point Start, Point End) CalculateEdgeConnectionPoints(BoxViewItem start, BoxViewItem end)
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

    #region HTTP Client & Helpers

    private async Task PostAsync(string path, object payload)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync(path, payload);
        await EnsureSuccess(response);
    }

    private async Task<T> PostAndReadAsync<T>(string path, object payload)
    {
        using var client = CreateClient();
        using var response = await client.PostAsJsonAsync(path, payload);
        await EnsureSuccess(response);
        var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return data ?? throw new InvalidOperationException("Prazan odgovor sa servera.");
    }

    private async Task<T> GetAsync<T>(string path)
    {
        using var client = CreateClient();
        using var response = await client.GetAsync(path);
        await EnsureSuccess(response);
        var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return data ?? throw new InvalidOperationException("Prazan odgovor sa servera.");
    }

    private async Task<T?> GetAsyncNullable<T>(string path) where T : class
    {
        using var client = CreateClient();
        using var response = await client.GetAsync(path);
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent || response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        await EnsureSuccess(response);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    private HttpClient CreateClient()
    {
        var baseUrl = ApiBaseUrlTextBox.Text.Trim().TrimEnd('/');
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("API URL je obavezan.");
        }

        var client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute)
        };

        if (!string.IsNullOrWhiteSpace(_jwtToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
        }

        return client;
    }

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var body = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("error", out var err))
                {
                    throw new InvalidOperationException(err.GetString());
                }
            }
            catch (JsonException) { }

            throw new InvalidOperationException($"API error ({(int)response.StatusCode}): {body}");
        }

        throw new InvalidOperationException($"API error ({(int)response.StatusCode}): {response.ReasonPhrase}");
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    #endregion

    #region View Models & DTO Records

    public sealed class TeamViewItem(Guid id, string teamName, string teamCode, Guid ownerId, int memberCount)
    {
        public Guid Id { get; init; } = id;
        public string TeamName { get; set; } = teamName;
        public string TeamCode { get; set; } = teamCode;
        public Guid OwnerId { get; set; } = ownerId;
        public int MemberCount { get; set; } = memberCount;
        public string MemberCountText => $"Članova: {MemberCount}";
    }

    public sealed class TeamMemberViewItem(Guid userId, string username, string email, string firstName, string lastName, string role, DateTime joinedAt)
    {
        public Guid UserId { get; init; } = userId;
        public string Username { get; init; } = username;
        public string Email { get; init; } = email;
        public string FirstName { get; init; } = firstName;
        public string LastName { get; init; } = lastName;
        public string FullName => $"{FirstName} {LastName}";
        public string Role { get; set; } = role;
        public DateTime JoinedAt { get; init; } = joinedAt;

        public Brush RoleBadgeBackground => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
        public Brush RoleBadgeForeground => Brushes.Black;
    }

    public sealed class MeetingViewItem(Guid id, Guid teamId, Guid boardId, DateTime startedAtUtc)
    {
        public Guid Id { get; init; } = id;
        public Guid TeamId { get; init; } = teamId;
        public Guid BoardId { get; init; } = boardId;
        public DateTime StartedAtUtc { get; init; } = startedAtUtc;
        public string Display => $"Sastanak: {Id.ToString()[..8]}... (Board: {BoardId.ToString()[..8]}...)";
        public string TimeText => $"Početak: {StartedAtUtc.ToLocalTime():HH:mm:ss}";
    }

    public sealed class MeetingParticipantViewItem(Guid userId, string username, string role, bool canDraw, DateTime joinedAtUtc)
    {
        public Guid UserId { get; init; } = userId;
        public string Username { get; init; } = username;
        public string Role { get; init; } = role;
        public bool CanDraw { get; set; } = canDraw;
        public DateTime JoinedAtUtc { get; init; } = joinedAtUtc;

        public string DrawBadgeText => CanDraw ? "Crtanje: Dozvoljeno" : "Samo pregled";
        public Brush DrawBadgeBackground => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
        public Brush DrawBadgeForeground => Brushes.Black;
    }
    public sealed class ChatMessageViewItem(Guid id, Guid senderId, string senderUsername, string content, DateTime sentAtUtc)
    {
        public Guid Id { get; init; } = id;
        public Guid SenderId { get; init; } = senderId;
        public string SenderUsername { get; init; } = senderUsername;
        public string Content { get; init; } = content;
        public DateTime SentAtUtc { get; init; } = sentAtUtc;
        public string TimeText => SentAtUtc.ToLocalTime().ToString("HH:mm:ss");
    }

    public sealed class BoxViewItem(Guid id, int x1, int y1, int x2, int y2, string className, IReadOnlyCollection<string> attributes, IReadOnlyCollection<string> methods)
    {
        public Guid Id { get; init; } = id;
        public int X1 { get; set; } = x1;
        public int Y1 { get; set; } = y1;
        public int X2 { get; set; } = x2;
        public int Y2 { get; set; } = y2;
        public string ClassName { get; set; } = className;
        public IReadOnlyCollection<string> Attributes { get; set; } = attributes;
        public IReadOnlyCollection<string> Methods { get; set; } = methods;
        public string Display => $"{ClassName} ({X1},{Y1})";
    }

    public sealed class LineViewItem(Guid id, Guid startBoxId, Guid endBoxId, Line shape, TextBlock? label)
    {
        public Guid Id { get; init; } = id;
        public Guid StartBoxId { get; init; } = startBoxId;
        public Guid EndBoxId { get; init; } = endBoxId;
        public Line Shape { get; init; } = shape;
        public TextBlock? Label { get; init; } = label;
    }

    private sealed record LoginResponse(string Token, LoginUserResponse User);
    private sealed record LoginUserResponse(Guid IdUser, string Username, string Email);
    private sealed record TeamResponse(Guid Id, string TeamName, string TeamCode, Guid OwnerId, int MemberCount);
    private sealed record TeamMemberResponse(Guid UserId, string Username, string Email, string FirstName, string LastName, string Role, DateTime JoinedAt);
    private sealed record MeetingStartedResponse(Guid MeetingId, Guid BoardId, Guid TeamId, DateTime StartedAtUtc);
    private sealed record MeetingParticipantResponse(Guid UserId, string Username, string Role, bool CanDraw, DateTime JoinedAtUtc);
    private sealed record ChatMessageResponse(Guid Id, Guid SenderId, string SenderUsername, string Content, DateTime SentAtUtc);
    private sealed record ElementCreateResponse(Guid Id);

    private sealed record BoardElementsResponse(
        IReadOnlyCollection<BoxData> Boxes,
        IReadOnlyCollection<ClassBoxData> ClassBoxes,
        IReadOnlyCollection<TextData> Texts,
        IReadOnlyCollection<LineData> Lines);

    private sealed record BoxData(Guid Id, int X1, int Y1, int X2, int Y2);
    private sealed record ClassBoxData(Guid Id, int X1, int Y1, int X2, int Y2, IReadOnlyCollection<string>? Attributes, IReadOnlyCollection<string>? Methods);
    private sealed record TextData(Guid Id, int X1, int Y1, int X2, int Y2, string Content, int FontSize, string Color, string? Format);
    private sealed record LineData(Guid Id, Guid StartBoxId, Guid EndBoxId, double X1, double Y1, double X2, double Y2, string? MiddleText, string? Text1, string? Text2);

    #endregion
}
