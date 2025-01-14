﻿using System;
using System.Collections.Generic;
using System.IO;

using FlyleafLib.MediaFramework.MediaStream;
using FlyleafLib.MediaFramework.MediaPlaylist;

namespace FlyleafLib.Plugins;

public class OpenSubtitles : PluginBase, IOpenSubtitles, ISearchLocalSubtitles
{
    public new int Priority { get; set; } = 3000;

    public OpenSubtitlesResults Open(string url)
    {
        /* TODO
         * 1) Identify language
         */

        foreach(var extStream in Selected.ExternalSubtitlesStreams)
            if (extStream.Url == url || extStream.DirectUrl == url)
                return new OpenSubtitlesResults(extStream);

        string title;
        bool converted = false;

        if (url.StartsWith("srt://"))
        {
            title = url;
            converted = true;
        }
        else
        {
            try
            {
                FileInfo fi = new(url);
                title = fi.Extension == null ? fi.Name : fi.Name[..^fi.Extension.Length];
            }
            catch { title = url; }
        }
        
        ExternalSubtitlesStream newExtStream = new()
        {
            Url         = url,
            Title       = title,
            Downloaded  = true,
            Converted   = converted,
        };

        AddExternalStream(newExtStream);

        return new OpenSubtitlesResults(newExtStream);
    }

    public OpenSubtitlesResults Open(Stream iostream) => null;

    public void SearchLocalSubtitles()
    {
        /* TODO
         * 1) Subs folder could exist under Season X (it will suggest another season's subtitle)
         * 2) Identify language
         * 3) Confidence
         */

        List<string> files = new();

        try
        {
            foreach (var lang in Config.Subtitles.Languages)
            {
                //FileInfo fi = new FileInfo(Handler.Playlist.Url);
                string prefix = Selected.Title[..Math.Min(Selected.Title.Length, 4)];

                string folder = Path.Combine(Playlist.FolderBase, Selected.Folder, "Subs");
                if (!Directory.Exists(folder))
                    return;

                string[] filesCur = Directory.GetFiles(Path.Combine(Playlist.FolderBase, Selected.Folder, "Subs"), $"{prefix}*{lang.IdSubLanguage}.utf8*.srt");
                foreach(string file in filesCur)
                {
                    bool exists = false;
                    foreach(var extStream in Selected.ExternalSubtitlesStreams)
                        if (extStream.Url == file)
                            { exists = true; break; }
                    if (exists) continue;

                    var mp = Utils.GetMediaParts(file);
                    if (Selected.Season > 0 && (Selected.Season != mp.Season || Selected.Episode != mp.Episode))
                        continue;

                    Log.Debug($"Adding [{lang}] {file}");

                    string title;
                    try
                    {
                        FileInfo fi = new(file);
                        title = fi.Extension == null ? fi.Name : fi.Name[..^fi.Extension.Length];
                    }
                    catch { title = file; }

                    AddExternalStream(new ExternalSubtitlesStream()
                    {
                        Url         = file,
                        Title       = title,
                        Converted   = true,
                        Downloaded  = true,
                        Language    = lang
                    });
                }
            }
            
        } catch { }
    }
}
