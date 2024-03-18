namespace Skills.Components.Layout;

public partial class Header
{

    private bool _docked = false;

    private void Toggle()
    {
        _docked = true;
        StateHasChanged();
    }
}