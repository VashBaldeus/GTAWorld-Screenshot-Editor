namespace GTAWorld_Screenshot_Editor.Controllers.Interfaces
{
    public interface IDialogService
    {
        string OpenFile(string caption, string filter = @"All files (*.*)|*.*");
    }
}
