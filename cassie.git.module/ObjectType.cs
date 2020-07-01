using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassie.git.module
{
    public enum ObjectType
    {
        ObjectCommit,
        ObjectTree,
        ObjectBlob,
        ObjectTag
    } 

    public class ObjectTypeOpt
    {
        public static ObjectType? ToObjectType(string msg)
        {
            switch (msg)
            {
                case "commit":
                    return ObjectType.ObjectCommit;
                case "tree":
                    return ObjectType.ObjectTree;
                case "blob":
                    return ObjectType.ObjectBlob;
                case "tag":
                    return ObjectType.ObjectTag;
            }
            return null;
        }
    }

    public static class ObjectTypeExtensions
    {
        public static string ToTypeString(this ObjectType ot)
        {
            switch (ot)
            {
                case ObjectType.ObjectCommit:
                    return "commit";
                case ObjectType.ObjectTree:
                    return "tree";
                case ObjectType.ObjectBlob:
                    return "blob";
                case ObjectType.ObjectTag:
                    return "tag";
                default:
                    return "";
            }
        }
    }


}