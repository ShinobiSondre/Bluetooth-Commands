using NAudio.CoreAudioApi;

public class VolumeController
{
    private MMDevice defaultDevice;

    public VolumeController()
    {
        var enumerator = new MMDeviceEnumerator();
        defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }

    public float GetCurrentVolume()
    {
        return defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
    }

    public void SetVolume(float level)
    {
        // level is between 0.0 and 100.0
        defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = level / 100.0f;
    }
}