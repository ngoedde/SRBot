namespace SRNetwork.Common.Protocol;

public enum EncodeResult : byte
{
    Success,
    InvalidMsgSize,
    InvalidHeader,
}