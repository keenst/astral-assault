namespace astral_assault;

public interface IKeyboardEventListener
{
    void OnKeyboardEvent(object sender, KeyboardEventArgs e);
    void OnKeyboardReleasedEvent(object sender, KeyboardEventArgs e);
}