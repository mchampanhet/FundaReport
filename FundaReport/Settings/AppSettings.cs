namespace FundaReport.Settings
{
    public class AppSettings
    {
        public FundaApiSettings FundaApiSettings { get; set; }
        public MakelaarReportSettings MakelaarReportSettings { get; set; }
        public string CollectMissingConfiguration()
        {
            var result = new List<string>();
            var allSettingsClasses = this.GetType().GetProperties();

            //foreach (var settingsClass in allSettingsClasses)
            //{
            //    var settingsObject = settingsClass.GetValue(this);
            //    var settingsProperties = settingsObject.GetType().GetProperties();
            //    foreach (var property in settingsProperties)
            //    {
            //        var settingsValue = property.GetValue(settingsObject)?.ToString();
            //        if (string.IsNullOrEmpty(settingsValue))
            //        {
            //            result.Add($"{settingsClass.Name}.{property.Name} configuration is missing");
            //        }
            //    }
            //}

            return string.Join(", ", result);
        }
    }
}
