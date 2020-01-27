namespace DatabaseBuilder
{
    public class DatabaseSettings
    {
        public string Collation              { get; set; }
        public bool   AllowSnapshotIsolation { get; set; }
        public bool   ReadCommittedSnapshot  { get; set; }
        public int    Compatibilitylevel     { get; set; }

    }


    public static class DatabaseSettingsExtensions
    {
        public static DatabaseSettings SetCompatibilityLevel(this DatabaseSettings settings, int compatibilityLevel)
        {
            settings.Compatibilitylevel = compatibilityLevel;
            return settings;
        }

        public static DatabaseSettings SetCollation(this DatabaseSettings settings, string collation)
        {
            settings.Collation = collation;
            return settings;
        }

        public static DatabaseSettings SetAllowSnapshotIsolation(this DatabaseSettings settings, bool allowSnapshotIsolation)
        {
            settings.AllowSnapshotIsolation = allowSnapshotIsolation;
            return settings;
        }
    }

}
