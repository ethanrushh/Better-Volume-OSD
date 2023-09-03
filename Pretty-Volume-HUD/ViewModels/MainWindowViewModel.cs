using ReactiveUI;
using Avalonia.Controls;
using Avalonia.Threading;
using NAudio.CoreAudioApi;
using System.Threading.Tasks;

namespace Pretty_Volume_HUD.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private static readonly MMDeviceEnumerator deviceEnumerator = new();
    private readonly MMDevice defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    private readonly Window? _parentWindow;


    private int _volume;
    public int Volume
    {
        get => _volume;
        set
        {
            VolumeText = VolumeText = $"{value}%".PadLeft(4);
            this.RaiseAndSetIfChanged(ref _volume, value);
        }
    }
    private string _volumeText = "0%";
    public string VolumeText
    {
        get => _volumeText;
        set => this.RaiseAndSetIfChanged(ref _volumeText, value);
    }

    public MainWindowViewModel() : this(null) {}
    public MainWindowViewModel(Window? parentWindow)
    {
        // Window setup
        _parentWindow = parentWindow;
        
        if (_parentWindow is not null)
            _parentWindow.Loaded += (_, _) => _parentWindow.Hide();
        
        // Audio setup
        defaultDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
    }


    private uint _mostRecentHandle = 0;
    private async Task HideAfterTime(int ms)
    {
        var thisHandle = ++_mostRecentHandle;
        
        await Task.Delay(ms);

        // We don't want to hide if there is a more recent show
        if (_mostRecentHandle == thisHandle)
            _parentWindow?.Hide();
    }

    private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            _parentWindow?.Show();
            Volume = (int)(data.MasterVolume * 100);
            HideAfterTime(2000).ConfigureAwait(false);
        });
    }
}
