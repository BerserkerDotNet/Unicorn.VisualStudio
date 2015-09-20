// Guids.cs
// MUST match guids.h

using System;

namespace Unicorn.VS
{
    static class GuidList
    {
        public const string guidUnicorn_VSPkgString = "841bbcc7-596f-47ea-a223-94dc6d023562";
        public const string guidUnicorn_VSCmdSetString = "8e4be64c-e72a-4b04-8514-e17f3c5867dd";
        public const string guidToolWindowPersistanceString = "6525e4a1-c458-4ce6-b6e7-c9b405af2300";

        public static readonly Guid guidUnicorn_VSCmdSet = new Guid(guidUnicorn_VSCmdSetString);
    };
}