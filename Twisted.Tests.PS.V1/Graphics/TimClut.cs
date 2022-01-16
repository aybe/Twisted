using System;
using System.Collections;
using System.Collections.Generic;

namespace Twisted.Tests.PS.V1.Graphics;

public sealed class TimClut : IReadOnlyList<TimClutEntry>
{
    public TimClut(IReadOnlyList<TimClutEntry> entries)
    {
        Entries = entries ?? throw new ArgumentNullException(nameof(entries));
    }

    private IReadOnlyList<TimClutEntry> Entries { get; }

    #region IReadOnlyList<TimClutEntry> Members

    public TimClutEntry this[int index] => Entries[index];

    public int Count => Entries.Count;

    public IEnumerator<TimClutEntry> GetEnumerator()
    {
        return Entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Entries).GetEnumerator();
    }

    #endregion
}