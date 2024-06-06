namespace SRNetwork.Common.Protocol;

public enum DecodeResult
{
    Success,
    InvalidHeader,
    InvalidSequence,
    InvalidChecksum,
    InvalidMsgSize,
    Unknown,
}