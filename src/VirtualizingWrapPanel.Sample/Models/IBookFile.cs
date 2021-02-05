using System;

namespace VirtualizingWrapPanel.Sample.Models
{
    interface IBookFile : IEntity<int>
    {
        long FileSize { get; set; }
        int Type { get; set; }
        string Key { get; set; } // cipher key
        string Path { get; set; } // path of file
        string Title { get; set; }
        int? Duration { get; set; }
        int SequenceNo { get; set; } // audio track number
        DateTimeOffset? StatusModifiedDate { get; set; }
        DateTimeOffset? CreationDate { get; set; }
    }
}
