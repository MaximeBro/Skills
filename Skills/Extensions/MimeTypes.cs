namespace Skills.Extensions;

public static class MimeTypes
{
    public static string GetMimeTypeOf(string ext)
    {
        Dictionary<string, string> mimeTypes = new Dictionary<string, string>
        {
            { ".txt", "text/plain" },
            { ".html", "text/html" },
            { ".jpg", "image/jpeg" },
            { ".pdf", "application/pdf" },
            { ".png", "image/png" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ".json", "application/json" },
            { ".xml", "application/xml" },
            { ".csv", "text/csv" },
            { ".zip", "application/zip" },
            { ".mp3", "audio/mpeg" },
            { ".mp4", "video/mp4" },
            { ".gif", "image/gif" },
            { ".bmp", "image/bmp" },
            { ".svg", "image/svg+xml" },
            { ".ico", "image/x-icon" },
            { ".wav", "audio/wav" },
            { ".ogg", "audio/ogg" },
            { ".avi", "video/x-msvideo" },
            { ".mkv", "video/x-matroska" },
            { ".css", "text/css" },
            { ".js", "application/javascript" },
            { ".php", "application/x-httpd-php" },
            { ".sql", "application/sql" },
            { ".jar", "application/java-archive" },
            { ".tar", "application/x-tar" },
            { ".rar", "application/x-rar-compressed" },
            { ".7z", "application/x-7z-compressed" },
            { ".woff", "font/woff" },
            { ".ttf", "font/ttf" },
            { ".eot", "application/vnd.ms-fontobject" },
            { ".swf", "application/x-shockwave-flash" },
            { ".apk", "application/vnd.android.package-archive" }
        };

        mimeTypes.TryGetValue(ext, out var mimeType);
        return mimeType ?? "application/octet-stream";
    }
}