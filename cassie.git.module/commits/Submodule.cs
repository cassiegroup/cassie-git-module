namespace cassie.git.module.commits
{
    // Submodule contains information of a Git submodule.
    public class Submodule 
    {
        // The name of the submodule.
        public string Name { get; set; }
        // The URL of the submodule.
        public string URL { get; set; }
        // The commit ID of the subproject.
        public string Commit { get; set; }
    }
}