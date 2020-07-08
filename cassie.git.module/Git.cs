// Copyright 2020 The Cassie Authors. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cassie.git.module
{
    // SHA1 is the SHA-1 hash of a Git object.
    public class Git
    {
        public static Version GitVersion{get;set;}
        public async static Task<Version> BinVersion()
        {
            var cmd = new Command("version");
            var result = await cmd.RunAsync();
            if (!string.IsNullOrEmpty(result.StdErr)) throw new Exception(result.StdErr);
            var fields = Regex.Split(result.StdOut, @"\s+");
            if(fields.Length<3) throw new Exception($"not enough output{result.StdOut}");
            // Handle special case on Windows.
            var i = fields[2].IndexOf("windows");
            if(i >=1){
                GitVersion = new Version(fields[2].Substring(0,i-1));
            }
            GitVersion = new Version(fields[2]);
            return GitVersion;
        }
    }
}