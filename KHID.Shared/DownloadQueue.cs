using HtmlAgilityPack;

namespace KHID.UI.KHID.Shared;

public class DownloadQueue
{
    public static DownloadQueue Instance;

    private List<QueueItem> _queue = new();
    private bool _isRunning;
    public EventHandler<List<QueueItem>> OnQueueUpdatedEventHandler;
    public EventHandler<QueueItem> OnQueueItemProgressEventHandler;

    public DownloadQueue()
    {
        if (Instance != null) throw new Exception("[DownloadQueue] Instance already exists");

        Instance = this;
    }

    public async Task<Soundtrack?> GetSoundtrackMeta(string urlOrSlug)
    {
        Console.WriteLine($"[DownloadQueue] GetSoundtrackMeta: {urlOrSlug}");
        
        var albumUrl = urlOrSlug.StartsWith("https://downloads.khinsider.com") ? urlOrSlug : $"https://downloads.khinsider.com/game-soundtracks/album/{urlOrSlug}";
        var albumWebsite = new HtmlWeb();
        var albumDocument = albumWebsite.Load(albumUrl);
        
        var albumTitleNode = albumDocument.DocumentNode.SelectSingleNode("//head/title");
        if (albumTitleNode.InnerHtml == "Error")
        {
            // TODO: Error Handling
            Console.WriteLine("[DownloadQueue] Soundtrack does not exist!");
            return null;
        }

        var albumNameNode = albumDocument.DocumentNode.SelectSingleNode("//*[@id=\"pageContent\"]/h2[1]");
        Console.WriteLine($"[DownloadQueue] Soundtrack Name: {albumNameNode.GetDirectInnerText()}");
        
        var soundtrack = new Soundtrack
        {
            Url = albumUrl,
            Name = albumNameNode.GetDirectInnerText(),
            Slug = albumUrl.Replace("https://downloads.khinsider.com/game-soundtracks/album/", "")
        };

        // File Formats
        var hasCdHeader = false;
        var songListHeaderItems = albumDocument.DocumentNode.SelectNodes("//*[@id=\"songlist_header\"]/th");
        foreach (var songListHeaderItem in songListHeaderItems)
        {
            // If the table has a column named "CD", the title will be on column 4 and not 3
            if (songListHeaderItem.InnerText == "CD") hasCdHeader = true;
            
            // Filter out non-format table headers
            if (new [] { "", "&nbsp;", "CD", "#", "Song Name" }.Contains(songListHeaderItem.InnerText.ToLower())) continue;
            
            soundtrack.Formats.Add(songListHeaderItem.InnerText);
        }
        
        // Songs
        var songListItems = albumDocument.DocumentNode.SelectNodes($"//*[@id=\"songlist\"]/tr[not(@id)]/td[{(hasCdHeader ? '4' : '3')}]/a");
        foreach (var songListItem in songListItems)
        {
            var href = songListItem.GetAttributeValue("href", "");
            var song = new Song
            {
                Name = songListItem.InnerText,
                Url = $"https://downloads.khinsider.com{href}",
            };
            soundtrack.Songs.Add(song);
        }

        return soundtrack;
    }

    public void AddToQueue(string songUrl)
    {
        Console.WriteLine($"[DownloadQueue] AddToQueue: {songUrl}");
            
        OnQueueUpdatedEventHandler?.Invoke(this, _queue);
    }

    public void ClearQueue()
    {
        // TODO: Stop running downloads
        
        _queue = new();
        OnQueueUpdatedEventHandler?.Invoke(this, _queue);
    }

    public List<QueueItem> GetQueue()
    {
        return _queue;
    }

    private async Task WorkQueue()
    {
        if (_isRunning) return;
        _isRunning = true;
        
        Console.WriteLine("[DownloadQueue] Starting queue.");
        
        while (_queue.Any(x => x.State == QueueItem.QueueState.Queued))
        {
            OnQueueUpdatedEventHandler?.Invoke(this, _queue);
            
            // Initialize Item
            var queueItem = _queue.First(x => x.State == QueueItem.QueueState.Queued);
            queueItem.State = QueueItem.QueueState.Downloading;
            OnQueueItemProgressEventHandler?.Invoke(this, queueItem);

            try
            {
                if (!Directory.Exists(queueItem.OutputPath))
                {
                    Directory.CreateDirectory(queueItem.OutputPath);
                }
                
                Console.WriteLine($"[DownloadQueue] ({queueItem.Title}) - Loading");
                var songWebsite = new HtmlWeb();
                var songDocument = songWebsite.Load(queueItem.Url);
                var downloadLinks = songDocument.DocumentNode.SelectNodes("//a[contains(@href, '/soundtracks/')]");
                
                Console.WriteLine($"[DownloadQueue] ({queueItem.Title}) - Downloading");
                // TODO: Download Selected Format instead of all
                foreach (var downloadLink in downloadLinks)
                {
                    var downloadUrl = new Uri(downloadLink.GetAttributeValue("href", ""));
                    var fileName = Path.GetFileName(downloadUrl.LocalPath);
                    var outputPath = Path.Combine(queueItem.OutputPath, fileName);

                    IProgress<double> progress = new Progress<double>(percentage =>
                    {
                        queueItem.DownloadProgress = percentage;
                        OnQueueItemProgressEventHandler?.Invoke(this, queueItem);
                    });

                    using var client = new HttpClient();
                    await client.DownloadFileTaskAsync(downloadUrl, outputPath, progress);
                }
            
                Console.WriteLine($"[DownloadQueue] ({queueItem.Title}) - DoneSucceeded");
                queueItem.State = QueueItem.QueueState.Done;
                queueItem.Result = QueueItem.QueueResult.DoneSucceeded;
                OnQueueItemProgressEventHandler?.Invoke(this, queueItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DownloadQueue] ({queueItem.Title}) - DoneError");
                queueItem.State = QueueItem.QueueState.Done;
                queueItem.Result = QueueItem.QueueResult.DoneError;
                
                OnQueueItemProgressEventHandler?.Invoke(this, queueItem);
                
                Console.WriteLine($"[DownloadQueue] {ex.Message}");
            }
        }
        
        Console.WriteLine("[DownloadQueue] Queue done.");
        OnQueueUpdatedEventHandler?.Invoke(this, _queue);
        _isRunning = false;
    }

    public static string ToSafeFilename(string input)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return invalidChars.Aggregate(input, (current, c) => current.Replace(c, '_'));
    }
}