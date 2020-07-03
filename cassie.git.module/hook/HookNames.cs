using System;

namespace cassie.git.module.hook
{ 
    // HookName is the name of a Git hook.
    public enum HookName
    {
        HookPreReceive,
        HookUpdate,
        HookPostReceive
    }
    public static class HookNamesExtensions
    {
        public static string ToTypeString(this HookName ot)
        {
            switch (ot)
            {
                case HookName.HookPreReceive:
                    return "pre-receive";
                case HookName.HookUpdate:
                    return "update";
                case HookName.HookPostReceive:
                    return "post-receive";
                default:
                    return "";
            }
        }
    }

}