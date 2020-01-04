namespace WarWolfWorks.Maps
{
    public class MapLoadProgress
    {
        internal float CurrentProgress;
        public float Progress => CurrentProgress;
        public float Progress01 => CurrentProgress / 100;

        internal string Loading;
        public string LoadingMessage => Loading;

        internal LoadState loadState;
        public LoadState LoadState => loadState;
    }
}
