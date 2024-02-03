namespace KHID.UI.KHID.Shared;

public static class HttpClientUtils
{
    public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string fileName, IProgress<double>? progress = null)
    {
        using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength;

        if (totalBytes is not > 0)
        {
            throw new InvalidOperationException("Cannot determine the total size of the content to download.");
        }

        await using var s = await response.Content.ReadAsStreamAsync();
        await using var fs = new FileStream(fileName, FileMode.CreateNew);

        long downloadedBytes = 0;
        var buffer = new byte[8192];

        int bytesRead;
        while ((bytesRead = await s.ReadAsync(buffer)) > 0)
        {
            await fs.WriteAsync(buffer.AsMemory(0, bytesRead));
            downloadedBytes += bytesRead;

            // Calculate and report progress
            var percentage = (double)downloadedBytes / totalBytes.Value * 100;
            progress?.Report(percentage);
        }
    }
}