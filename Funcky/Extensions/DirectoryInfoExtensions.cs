namespace Funcky.Extensions;

public static class DirectoryInfoExtensions
{
    extension(DirectoryInfo directory)
    {
        /// <summary>The parent directory, or <c>None</c> for a root directory, where <see cref="DirectoryInfo.Parent"/> is
        /// <see langword="null"/>.</summary>
        public Option<DirectoryInfo> ParentOrNone
            => Option.FromNullable(directory.Parent);
    }
}
