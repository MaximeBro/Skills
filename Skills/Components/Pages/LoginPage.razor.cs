using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Skills.Components.Components;
using Skills.Services;

using Timer = System.Timers.Timer;

namespace Skills.Components.Pages;

public partial class LoginPage : FullComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Inject] public ADAuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    
    private LoginModel _model = new();
    private bool _errorMessage;

    private bool _shown;
    public string PasswordIcon => _shown ? Icons.Material.Filled.Visibility : Icons.Material.Filled.VisibilityOff;
    public InputType PasswordInputType => _shown ? InputType.Text : InputType.Password;

    protected override async Task OnInitializedAsync()
    {
        var authenticated = await AuthenticationService.IsAuthenticatedAsync(AuthenticationState);
        if (authenticated)
        {
            NavManager.NavigateTo("/", true);
        }
    }
    
    private async Task TrySignInAsync()
    {
        if (string.IsNullOrWhiteSpace(_model.Username) || string.IsNullOrWhiteSpace(_model.Password)) return;
        var guid = await AuthenticationService.TryLoginAsync(_model.Username, _model.Password);
        if (guid == Guid.Empty)
        {
            Snackbar.Add(
                "Votre compte est enregistré sur l'Active Directory local mais pas sur Skills ! Veuillez contacter un administrateur de la plateforme",
                Severity.Error);
            return;
        }
        
        if (guid != null)
        {
            NavManager.NavigateTo($"/api/login/{guid}", true);
        }
        else
        {
            _model = new();
            _errorMessage = true;
            var timer = new Timer(5000);
            timer.Elapsed += (obj, e) => { _errorMessage = false; _ = InvokeAsync(StateHasChanged); timer.Dispose(); };
            timer.Start();
        }
    }
    
    private sealed class LoginModel
    {
        [Required(ErrorMessage = "Veuillez spécifier votre identifint.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veuillez spécifier votre mot de passe.")]
        public string Password { get; set; } = string.Empty;
    } 
}