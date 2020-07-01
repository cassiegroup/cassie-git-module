using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cassie.git.module.diffs;
using Xunit;

namespace cassie.git.module.test
{
    public class DiffTests
    {
        [Fact]
        public void DiffSection_True_Line()
        {
            var lineDelete = new DiffLine
            {
                Type = DiffLineType.DiffLineDelete,
                Content = "-  <groupId>com.ambientideas</groupId>",
                LeftLine = 4,
                RightLine = 0
            };
            var lineAdd = new DiffLine
            {
                Type = DiffLineType.DiffLineAdd,
                Content = "+  <groupId>com.github</groupId",
                LeftLine = 0,
                RightLine = 4
            };

            var section = new DiffSection
            {
                Lines = new List<DiffLine>{
                new DiffLine
                {
                    Type=DiffLineType.DiffLineSection,
                    Content="@@ -1,7 +1,7 @@"
                },
                new DiffLine
                {
                    Type=DiffLineType.DiffLinePlain,
                    Content=" <project xmlns=\"http://maven.apache.org/POM/4.0.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"",
                    LeftLine=1,
                    RightLine=1
                },
                new DiffLine
                {
                    Type=DiffLineType.DiffLinePlain,
                    Content="   xsi:schemaLocation=\"http://maven.apache.org/POM/4.0.0 http://maven.apache.org/maven-v4_0_0.xsd\">",
                    LeftLine=2,
                    RightLine=2
                },
                new DiffLine
                {
                    Type=DiffLineType.DiffLinePlain,
                    Content="   <modelVersion>4.0.0</modelVersion>",
                    LeftLine=3,
                    RightLine=3
                },
                lineDelete,
                lineAdd,
                new DiffLine
                {
                    Type=DiffLineType.DiffLinePlain,
                    Content="   <artifactId>egitdemo</artifactId>",
                    LeftLine=5,
                    RightLine=5
                },
                new DiffLine
                {
                    Type=DiffLineType.DiffLinePlain,
                    Content="   <packaging>jar</packaging>",
                    LeftLine=6,
                    RightLine=6
                },
                new DiffLine
                {
                    Type=DiffLineType.DiffLinePlain,
                    Content="   <version>1.0-SNAPSHOT</version>",
                    LeftLine=7,
                    RightLine=7
                }
            }
            };
            var del = section.Line(lineDelete.Type, 4);
            Assert.Equal(lineDelete, del);

            var add = section.Line(lineAdd.Type, 4);
            Assert.Equal(lineAdd, add);
        }

        [Fact]
        public void DiffParser_True_StreamParseDiff()
        {
            var lines = @"diff --git a/.gitmodules b/.gitmodules
new file mode 100644
index 0000000..6abde17
--- /dev/null
+++ b/.gitmodules
@@ -0,0 +1,3 @@
+[submodule ""gogs/docs-api""]
+	path = gogs/docs-api
+	url = https://github.com/gogs/docs-api.git
diff --git a/gogs/docs-api b/gogs/docs-api
new file mode 160000
index 0000000..6b08f76
--- /dev/null
+++ b/gogs/docs-api
@@ -0,0 +1 @@
+Subproject commit 6b08f76a5313fa3d26859515b30aa17a5faa2807";
            var dp = new DiffParser(0, 0, 0);
            var result = dp.StreamParseDiff(lines, '\n');

            lines = @"diff --git a/pom.xml b/pom.xml
index ee791be..9997571 100644
--- a/pom.xml
+++ b/pom.xml
@@ -1,7 +1,7 @@
 <project xmlns=""http://maven.apache.org/POM/4.0.0"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
   xsi:schemaLocation=""http://maven.apache.org/POM/4.0.0 http://maven.apache.org/maven-v4_0_0.xsd"">
   <modelVersion>4.0.0</modelVersion>
-  <groupId>com.ambientideas</groupId>
+  <groupId>com.github</groupId>
   <artifactId>egitdemo</artifactId>
   <packaging>jar</packaging>
   <version>1.0-SNAPSHOT</version>";
            result = dp.StreamParseDiff(lines, '\n');

            lines = @"diff --git a/img/sourcegraph.png b/img/sourcegraph.png
new file mode 100644
index 0000000..2ce9188
Binary files /dev/null and b/img/sourcegraph.png differ";

            result = dp.StreamParseDiff(lines, '\n');

            lines = @"diff --git a/fix.txt b/fix.txt
deleted file mode 100644
index e69de29..0000000";

            result = dp.StreamParseDiff(lines, '\n');

            lines = @"diff --git a/runme.sh b/run.sh
similarity index 100%
rename from runme.sh
rename to run.sh";

            result = dp.StreamParseDiff(lines, '\n');

            lines = @"diff --git a/dir/file.txt b/dir/file.txt
index b6fc4c620b67d95f953a5c1c1230aaab5db5a1b0..ab80bda5dd90d8b42be25ac2c7a071b722171f09 100644
--- a/dir/file.txt
+++ b/dir/file.txt
@@ -1 +1,3 @@
-hello
\ No newline at end of file
+hello
+
+fdsfdsfds
\ No newline at end of file";

            result = dp.StreamParseDiff(lines, '\n');

            lines = @"diff --git a/src/app/tabs/teacher/teacher.module.ts b/src/app/tabs/friends/friends.module.ts
similarity index 69%
rename from src/app/tabs/teacher/teacher.module.ts
rename to src/app/tabs/friends/friends.module.ts
index ce53c7e..56a156b 100644
--- a/src/app/tabs/teacher/teacher.module.ts
+++ b/src/app/tabs/friends/friends.module.ts
@@ -2,9 +2,9 @@ import { IonicModule } from '@ionic/angular'
 import { RouterModule } from '@angular/router'
 import { NgModule } from '@angular/core'
 import { CommonModule } from '@angular/common'
-import { FormsModule } from '@angular/forms'
-import { TeacherPage } from './teacher.page'
 import { ComponentsModule } from '@components/components.module'
+import { FormsModule } from '@angular/forms'
+import { FriendsPage } from './friends.page'";

            result = dp.StreamParseDiff(lines, '\n');

            lines = @"diff --git a/.travis.yml b/.travis.yml
index 335db7ea..51d7543e 100644
--- a/.travis.yml
+++ b/.travis.yml
@@ -1,9 +1,6 @@
 sudo: false
 language: go
 go:
-  - 1.4.x
-  - 1.5.x
-  - 1.6.x
   - 1.7.x
   - 1.8.x
   - 1.9.x
@@ -12,6 +9,7 @@ go:
   - 1.12.x
   - 1.13.x
 
+install: go get -v ./...
 script: 
   - go get golang.org/x/tools/cmd/cover
   - go get github.com/smartystreets/goconvey";
            dp = new DiffParser(0, 2, 0);
            result = dp.StreamParseDiff(lines, '\n');

            lines = @"diff --git a/.gitmodules b/.gitmodules
new file mode 100644
index 0000000..6abde17
--- /dev/null
+++ b/.gitmodules
@@ -0,0 +1,3 @@
+[submodule ""gogs/docs-api""]
+	path = gogs/docs-api
+	url = https://github.com/gogs/docs-api.git";
            dp = new DiffParser(0, 0, 30);
            result = dp.StreamParseDiff(lines, '\n');

            lines = @"diff --git a/.gitmodules b/.gitmodules
new file mode 100644
index 0000000..6abde17
--- /dev/null
+++ b/.gitmodules
@@ -0,0 +1,3 @@
+[submodule ""gogs/docs-api""]
+	path = gogs/docs-api
+	url = https://github.com/gogs/docs-api.git
diff --git a/gogs/docs-api b/gogs/docs-api
new file mode 160000
index 0000000..6b08f76
--- /dev/null
+++ b/gogs/docs-api
@@ -0,0 +1 @@
+Subproject commit 6b08f76a5313fa3d26859515b30aa17a5faa2807";
            dp = new DiffParser(1, 2, 30);
            result = dp.StreamParseDiff(lines, '\n');
        }
    }
}