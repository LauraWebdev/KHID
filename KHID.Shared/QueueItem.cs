namespace KHID.UI.KHID.Shared;

public class QueueItem
{
    public string Title;
    public Uri Url;
    public string SoundtrackTitle;
    public string OutputPath;
    public string Format;
    public double DownloadProgress = 0f;
    public QueueState State = QueueState.Queued;
    public QueueResult? Result;

    public enum QueueState
    {
        Queued,
        Downloading,
        Done,
    }

    public enum QueueResult
    {
        DoneError,
        DoneSucceeded,
    }
}