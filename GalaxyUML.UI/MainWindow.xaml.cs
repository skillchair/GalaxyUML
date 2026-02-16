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

namespace GalaxyUML.UI;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<TeamViewItem> _teams = [];
    private readonly ObservableCollection<MeetingViewItem> _meetings = [];
    private readonly ObservableCollection<BoxViewItem> _boxes = [];

    private Guid? _currentUserId;
    private string? _jwtToken;
    private Guid? _activeBoardId;
    private Guid? _activeMeetingId;

    private string _selectedTool = "ClassBox";
    private Point _dragStart;
    private Rectangle? _previewRectangle;
    private bool _isDragging;

    public MainWindow()
    {
        InitializeComponent();

        TeamsListBox.ItemsSource = _teams;
        MeetingTeamComboBox.ItemsSource = _teams;
        MeetingsListBox.ItemsSource = _meetings;
        StartBoxComboBox.ItemsSource = _boxes;
        EndBoxComboBox.ItemsSource = _boxes;

        TeamStatusText.Text = "Nema akcije.";
        MeetingStatusText.Text = "Nema aktivnih mitinga.";
        BoardStatusText.Text = "Pokreni miting da dobijes board (dijagram).";
        BoardInfoText.Text = "Board nije aktivan.";

        SetLineControlsVisibility();
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var payload = new
            {
                firstName = RegisterFirstNameTextBox.Text.Trim(),
                lastName = RegisterLastNameTextBox.Text.Trim(),
                username = RegisterUsernameTextBox.Text.Trim(),
                email = RegisterEmailTextBox.Text.Trim(),
                password = RegisterPasswordBox.Password
            };

            await PostAsync("/api/auth/register", payload);
            RegisterFirstNameTextBox.Clear();
            RegisterLastNameTextBox.Clear();
            RegisterUsernameTextBox.Clear();
            RegisterEmailTextBox.Clear();
            RegisterPasswordBox.Clear();
            MessageBox.Show("Registracija uspesna.", "Auth", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Register", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var payload = new
            {
                username = LoginUsernameTextBox.Text.Trim(),
                password = LoginPasswordBox.Password
            };

            var response = await PostAndReadAsync<LoginResponse>("/api/auth/login", payload);
            _jwtToken = response.Token;
            _currentUserId = response.User.IdUser;
            SessionStatusText.Text = $"Ulogovan: {response.User.Username} ({response.User.IdUser})";
            TeamStatusText.Text = "Spreman za tim operacije.";
            MeetingStatusText.Text = "Spreman za pokretanje mitinga.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void CreateTeamButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentUserId is null)
        {
            MessageBox.Show("Prvo se uloguj.", "Tim", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var teamName = CreateTeamNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(teamName))
        {
            MessageBox.Show("Unesi naziv tima.", "Tim", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var created = await PostAndReadAsync<TeamResponse>("/api/teams", new { teamName, ownerId = _currentUserId.Value });
            UpsertTeam(created);
            CreateTeamNameTextBox.Clear();
            TeamStatusText.Text = $"Kreiran tim '{created.TeamName}'. Kod: {created.TeamCode}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Create team", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void FindTeamByCodeButton_Click(object sender, RoutedEventArgs e)
    {
        var code = JoinCodeTextBox.Text.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code))
        {
            MessageBox.Show("Unesi team code.", "Tim", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var found = await GetAsync<TeamResponse>($"/api/teams/by-code/{code}");
            UpsertTeam(found);
            TeamStatusText.Text = $"Pronadjen tim '{found.TeamName}' (owner: {found.OwnerId}, clanova: {found.MemberCount}).";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Find team", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void JoinByCodeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentUserId is null)
        {
            MessageBox.Show("Prvo se uloguj.", "Tim", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var code = JoinCodeTextBox.Text.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code))
        {
            MessageBox.Show("Unesi team code.", "Tim", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var joined = await PostAndReadAsync<TeamResponse>("/api/teams/join-by-code", new { userId = _currentUserId.Value, joinCode = code });
            UpsertTeam(joined);
            TeamStatusText.Text = $"Uclanjen u tim '{joined.TeamName}' po kodu '{joined.TeamCode}'.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Join team", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void StartMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentUserId is null)
        {
            MessageBox.Show("Prvo se uloguj.", "Miting", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MeetingTeamComboBox.SelectedItem is not TeamViewItem selectedTeam)
        {
            MessageBox.Show("Izaberi tim.", "Miting", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var created = await PostAndReadAsync<MeetingStartedResponse>("/api/meetings", new
            {
                teamId = selectedTeam.Id,
                organizerId = _currentUserId.Value
            });

            _activeBoardId = created.BoardId;
            _activeMeetingId = created.MeetingId;
            BoardInfoText.Text = $"Board ID: {created.BoardId}";
            MeetingStatusText.Text = $"Miting pokrenut. Meeting: {created.MeetingId}";
            BoardStatusText.Text = "Aktivan board spreman. Prevuci misem da dodas kvadrat (ClassBox).";

            var meetingItem = new MeetingViewItem(created.MeetingId, created.TeamId, created.BoardId, created.StartedAtUtc);
            _meetings.Add(meetingItem);
            MeetingsListBox.SelectedItem = meetingItem;
            ResetLocalBoard();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Start meeting", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void EndMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        if (_currentUserId is null)
        {
            MessageBox.Show("Prvo se uloguj.", "Miting", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MeetingsListBox.SelectedItem is not MeetingViewItem selectedMeeting)
        {
            MessageBox.Show("Izaberi miting koji zelis da zavrsis.", "Miting", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await PostAsync($"/api/meetings/{selectedMeeting.Id}/end", new { userId = _currentUserId.Value });
            _meetings.Remove(selectedMeeting);

            if (_activeMeetingId == selectedMeeting.Id)
            {
                _activeMeetingId = null;
                _activeBoardId = null;
                ResetLocalBoard();
                BoardInfoText.Text = "Board nije aktivan.";
            }

            MeetingStatusText.Text = $"Miting {selectedMeeting.Id} je zavrsen.";
            BoardStatusText.Text = "Miting je zavrsen. Pokreni novi miting za crtanje.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "End meeting", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ToolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ToolComboBox.SelectedItem is ComboBoxItem item && item.Content is string tool)
        {
            _selectedTool = tool;
            if (BoardStatusText is not null)
            {
                BoardStatusText.Text = $"Aktivan alat: {_selectedTool}";
            }

            SetLineControlsVisibility();
        }
    }

    private void SetLineControlsVisibility()
    {
        if (LineControlsWrap is null)
        {
            return;
        }

        LineControlsWrap.Visibility = _selectedTool == "Line" ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ClearBoardButton_Click(object sender, RoutedEventArgs e)
    {
        ResetLocalBoard();
        BoardStatusText.Text = "Lokalni prikaz table je ociscen.";
    }

    private void BoardCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_selectedTool != "ClassBox")
        {
            return;
        }

        _dragStart = e.GetPosition(BoardCanvas);
        _isDragging = true;
        BoardCanvas.CaptureMouse();

        _previewRectangle = new Rectangle
        {
            Stroke = Brushes.Black,
            Fill = Brushes.White,
            StrokeThickness = 2
        };
        Panel.SetZIndex(_previewRectangle, 10);
        BoardCanvas.Children.Add(_previewRectangle);
    }

    private void BoardCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging || _previewRectangle is null)
        {
            return;
        }

        var current = e.GetPosition(BoardCanvas);
        var dx = current.X - _dragStart.X;
        var dy = current.Y - _dragStart.Y;
        var side = Math.Max(Math.Abs(dx), Math.Abs(dy));
        var left = dx >= 0 ? _dragStart.X : _dragStart.X - side;
        var top = dy >= 0 ? _dragStart.Y : _dragStart.Y - side;

        _previewRectangle.Width = side;
        _previewRectangle.Height = side;
        Canvas.SetLeft(_previewRectangle, left);
        Canvas.SetTop(_previewRectangle, top);
    }

    private async void BoardCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging || _previewRectangle is null)
        {
            return;
        }

        _isDragging = false;
        BoardCanvas.ReleaseMouseCapture();

        if (_activeBoardId is null)
        {
            BoardStatusText.Text = "Nema aktivnog board-a. Prvo startuj miting.";
            BoardCanvas.Children.Remove(_previewRectangle);
            _previewRectangle = null;
            return;
        }

        var left = (int)Canvas.GetLeft(_previewRectangle);
        var top = (int)Canvas.GetTop(_previewRectangle);
        var side = (int)Math.Round(_previewRectangle.Width);

        if (side < 10)
        {
            BoardCanvas.Children.Remove(_previewRectangle);
            _previewRectangle = null;
            BoardStatusText.Text = "Kvadrat je previse mali.";
            return;
        }

        try
        {
            var created = await PostAndReadAsync<ElementCreateResponse>($"/api/diagram/{_activeBoardId}/class-box", new
            {
                x1 = left,
                y1 = top,
                x2 = left + side,
                y2 = top + side,
                attributes = Array.Empty<string>(),
                methods = Array.Empty<string>()
            });

            var box = new BoxViewItem(created.Id, left, top, left + side, top + side);
            _boxes.Add(box);
            StartBoxComboBox.Items.Refresh();
            EndBoxComboBox.Items.Refresh();
            BoardStatusText.Text = $"Dodat ClassBox: {created.Id}";
        }
        catch (Exception ex)
        {
            BoardCanvas.Children.Remove(_previewRectangle);
            MessageBox.Show(ex.Message, "ClassBox", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            _previewRectangle = null;
        }
    }

    private async void CreateLineButton_Click(object sender, RoutedEventArgs e)
    {
        if (_activeBoardId is null)
        {
            MessageBox.Show("Nema aktivnog board-a.", "Line", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (StartBoxComboBox.SelectedItem is not BoxViewItem start || EndBoxComboBox.SelectedItem is not BoxViewItem end)
        {
            MessageBox.Show("Izaberi start i end box.", "Line", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (start.Id == end.Id)
        {
            MessageBox.Show("Start i end box moraju biti razliciti.", "Line", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var created = await PostAndReadAsync<ElementCreateResponse>($"/api/diagram/{_activeBoardId}/line", new
            {
                startBoxId = start.Id,
                endBoxId = end.Id,
                middleText = (string?)null,
                text1 = (string?)null,
                text2 = (string?)null
            });

            var line = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 3,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                X1 = (start.X1 + start.X2) / 2d,
                Y1 = (start.Y1 + start.Y2) / 2d,
                X2 = (end.X1 + end.X2) / 2d,
                Y2 = (end.Y1 + end.Y2) / 2d
            };
            Panel.SetZIndex(line, 20);
            BoardCanvas.Children.Add(line);
            BoardStatusText.Text = $"Dodat Line: {created.Id}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Line", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void UpsertTeam(TeamResponse team)
    {
        var existing = _teams.FirstOrDefault(t => t.Id == team.Id);
        if (existing is not null)
        {
            existing.TeamName = team.TeamName;
            existing.TeamCode = team.TeamCode;
            existing.OwnerId = team.OwnerId;
            existing.MemberCount = team.MemberCount;
            TeamsListBox.Items.Refresh();
            MeetingTeamComboBox.Items.Refresh();
            return;
        }

        _teams.Add(new TeamViewItem(team.Id, team.TeamName, team.TeamCode, team.OwnerId, team.MemberCount));
    }

    private void ResetLocalBoard()
    {
        BoardCanvas.Children.Clear();
        _boxes.Clear();
    }

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
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(body))
        {
            throw new InvalidOperationException($"API error ({(int)response.StatusCode}): {body}");
        }

        throw new InvalidOperationException($"API error ({(int)response.StatusCode}): {response.ReasonPhrase}");
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class TeamViewItem(Guid id, string teamName, string teamCode, Guid ownerId, int memberCount)
    {
        public Guid Id { get; init; } = id;
        public string TeamName { get; set; } = teamName;
        public string TeamCode { get; set; } = teamCode;
        public Guid OwnerId { get; set; } = ownerId;
        public int MemberCount { get; set; } = memberCount;
        public string Display => $"{TeamName} | code: {TeamCode} | owner: {OwnerId} | clanova: {MemberCount}";
    }

    private sealed class MeetingViewItem(Guid id, Guid teamId, Guid boardId, DateTime startedAtUtc)
    {
        public Guid Id { get; init; } = id;
        public Guid TeamId { get; init; } = teamId;
        public Guid BoardId { get; init; } = boardId;
        public DateTime StartedAtUtc { get; init; } = startedAtUtc;
        public string Display => $"{Id} | team: {TeamId} | board: {BoardId} | utc: {StartedAtUtc:HH:mm:ss}";
    }

    private sealed class BoxViewItem(Guid id, int x1, int y1, int x2, int y2)
    {
        public Guid Id { get; init; } = id;
        public int X1 { get; init; } = x1;
        public int Y1 { get; init; } = y1;
        public int X2 { get; init; } = x2;
        public int Y2 { get; init; } = y2;
        public string Display => $"{Id} ({X1},{Y1})-({X2},{Y2})";
    }

    private sealed record LoginResponse(string Token, LoginUserResponse User);
    private sealed record LoginUserResponse(Guid IdUser, string Username, string Email);
    private sealed record TeamResponse(Guid Id, string TeamName, string TeamCode, Guid OwnerId, int MemberCount);
    private sealed record MeetingStartedResponse(Guid MeetingId, Guid BoardId, Guid TeamId, DateTime StartedAtUtc);
    private sealed record ElementCreateResponse(Guid Id);
}
