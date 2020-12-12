// Interface for day-to-day dialogs
public interface DialogContent
{
    Dialog Get(string key);
    bool ContainsKey(string key);
}
