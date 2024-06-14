namespace SRCore;

[Flags]
public enum KernelMode : int
{
    Prod = 0,
    Debug = 16,
    CommandLineInterface = 32,
    WindowInterface = 64
}