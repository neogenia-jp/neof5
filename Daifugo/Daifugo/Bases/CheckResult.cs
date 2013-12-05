using System;
using System.Diagnostics;

namespace Daifugo.Bases
{
    public interface ICheckResult
    {
        string Message { get; }
    }

    public sealed class CheckOK : ICheckResult
    {
        public string Message { get { return "OK"; } }
        public override string ToString() { return "Check Result: OK"; }
    }
    public sealed class CheckError : ICheckResult
    {
        public string Message { get; set; }

        public CheckError(string message) { Message = message; }
        public override string ToString() { return "Check Result: "+ Message; }
    }

    public static class CheckResults
    {
        public static readonly CheckOK Ok = new CheckOK();
        public static readonly CheckError UnknownError = new CheckError("Unknown Error");
    }
}
