using System.Security.Claims;
using BlazorHero.CleanArchitecture.Client.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Communication;
using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Contracts.Identity;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Constants.Storage;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace BlazorHero.CleanArchitecture.Client.Pages.Communication;

public partial class Chat
{
    private List<ChatMessageResponse> _messages = new();

    private bool _open;

    public List<ChatUserResponse> UserList = new();
    [Inject] private IChatManager ChatManager { get; set; }

    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [Parameter] public string CurrentMessage { get; set; }
    [Parameter] public string CurrentUserId { get; set; }
    [Parameter] public string CurrentUserImageUrl { get; set; }
    [Parameter] public string CFullName { get; set; }
    [Parameter] public string CId { get; set; }
    [Parameter] public string CUserName { get; set; }
    [Parameter] public string CImageUrl { get; set; }
    private Anchor ChatDrawer { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender) =>
        await JsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");

    private async Task SubmitAsync()
    {
        if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(CId))
        {
            //Save Message to DB
            var chatMessage = new ChatMessage<IChatUser>
            {
                Message = CurrentMessage, ToUserId = CId, CreatedDate = DateTime.Now
            };
            IResult response = await ChatManager.SaveMessageAsync(chatMessage);
            if (response.Succeeded)
            {
                AuthenticationState state = await StateProvider.GetAuthenticationStateAsync();
                ClaimsPrincipal user = state.User;
                CurrentUserId = user.GetUserId();
                chatMessage.FromUserId = CurrentUserId;
                var userName = $"{user.GetFirstName()} {user.GetLastName()}";
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendMessage, chatMessage, userName);
                CurrentMessage = string.Empty;
            }
            else
            {
                SnackBar.Error(response.Messages);
            }
        }
    }

    private async Task OnKeyPressInChat(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SubmitAsync();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        HubConnection = HubConnection.TryInitialize(NavigationManager, LocalStorage);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }

        HubConnection.On<string>(ApplicationConstants.SignalR.PingResponse,
            userId =>
            {
                ChatUserResponse connectedUser = UserList.Find(x => x.Id.Equals(userId));
                if (connectedUser is not { IsOnline: false })
                {
                    return;
                }

                connectedUser.IsOnline = true;
                SnackBar.Add($"{connectedUser.UserName} {Localizer["Logged In."]}", Severity.Info);
                StateHasChanged();
            });
        HubConnection.On<string>(ApplicationConstants.SignalR.ConnectUser,
            userId =>
            {
                ChatUserResponse connectedUser = UserList.Find(x => x.Id.Equals(userId));
                if (connectedUser is not { IsOnline: false })
                {
                    return;
                }

                connectedUser.IsOnline = true;
                SnackBar.Info($"{connectedUser.UserName} {Localizer["Logged In."]}");
                StateHasChanged();
            });
        HubConnection.On<string>(ApplicationConstants.SignalR.DisconnectUser,
            userId =>
            {
                ChatUserResponse disconnectedUser = UserList.Find(x => x.Id.Equals(userId));
                if (disconnectedUser is not { IsOnline: true })
                {
                    return;
                }

                disconnectedUser.IsOnline = false;
                SnackBar.Info($"{disconnectedUser.UserName} {Localizer["Logged Out."]}");
                StateHasChanged();
            });
        HubConnection.On<ChatMessage<IChatUser>, string>(ApplicationConstants.SignalR.ReceiveMessage,
            async (chatMessage, userName) =>
            {
                if ((CId == chatMessage.ToUserId && CurrentUserId == chatMessage.FromUserId) ||
                    (CId == chatMessage.FromUserId && CurrentUserId == chatMessage.ToUserId))
                {
                    if (CId == chatMessage.ToUserId && CurrentUserId == chatMessage.FromUserId)
                    {
                        _messages.Add(new ChatMessageResponse
                        {
                            Message = chatMessage.Message,
                            FromUserFullName = userName,
                            CreatedDate = chatMessage.CreatedDate,
                            FromUserImageUrl = CurrentUserImageUrl
                        });
                        await HubConnection.SendAsync(ApplicationConstants.SignalR.SendChatNotification,
                            string.Format(Localizer["New Message From {0}"], userName),
                            CId,
                            CurrentUserId);
                    }
                    else if (CId == chatMessage.FromUserId && CurrentUserId == chatMessage.ToUserId)
                    {
                        _messages.Add(new ChatMessageResponse
                        {
                            Message = chatMessage.Message,
                            FromUserFullName = userName,
                            CreatedDate = chatMessage.CreatedDate,
                            FromUserImageUrl = CImageUrl
                        });
                    }

                    await JsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
                    StateHasChanged();
                }
            });
        await GetUsersAsync();
        AuthenticationState state = await StateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal user = state.User;
        CurrentUserId = user.GetUserId();
        CurrentUserImageUrl = await LocalStorage.GetItemAsync<string>(StorageConstants.Local.UserImageUrl);
        if (!string.IsNullOrEmpty(CId))
        {
            await LoadUserChat(CId);
        }

        await HubConnection.SendAsync(ApplicationConstants.SignalR.PingRequest, CurrentUserId);
    }

    private async Task LoadUserChat(string userId)
    {
        _open = false;
        IResult<UserResponse> response = await UserManager.GetAsync(userId);
        if (response.Succeeded)
        {
            UserResponse contact = response.Data;
            CId = contact.Id;
            CFullName = $"{contact.FirstName} {contact.LastName}";
            CUserName = contact.UserName;
            CImageUrl = contact.ProfilePictureDataUrl;
            NavigationManager.NavigateTo($"chat/{CId}");
            //Load messages from db here
            _messages = new List<ChatMessageResponse>();
            IResult<IEnumerable<ChatMessageResponse>> chatHistoryResponse = await ChatManager.GetChatHistoryAsync(CId);
            if (chatHistoryResponse.Succeeded)
            {
                _messages = chatHistoryResponse.Data.ToList();
            }
            else
            {
                SnackBar.Error(response.Messages);
            }
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
    }

    private async Task GetUsersAsync()
    {
        //add get chat history from chat controller / manager
        IResult<IEnumerable<ChatUserResponse>> response = await ChatManager.GetChatUsersAsync();
        if (response.Succeeded)
        {
            UserList = response.Data.ToList();
        }
        else
        {
            SnackBar.Error(response.Messages);
        }
    }

    private void OpenDrawer(Anchor anchor)
    {
        ChatDrawer = anchor;
        _open = true;
    }

    private static Color GetUserStatusBadgeColor(bool isOnline) => isOnline ? Color.Success : Color.Error;
}
