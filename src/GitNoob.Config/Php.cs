namespace GitNoob.Config
{
    public class Php
    {
        public string Path { get; set; }

        public string PhpIniTemplateContents { get; set; }

        public void CopyFrom(Php other)
        {
            Path = other.Path;
            PhpIniTemplateContents = other.PhpIniTemplateContents;
        }
    }
}
