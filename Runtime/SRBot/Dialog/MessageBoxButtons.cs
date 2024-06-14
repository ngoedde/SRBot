using System;

namespace SRBot.Dialog;

[Flags]
public enum MessageBoxButtons : byte
{
    None = 0,
    Ok = 2,
    Cancel = 4,
    Retry = 8,
    Yes = 16,
    No = 32
}