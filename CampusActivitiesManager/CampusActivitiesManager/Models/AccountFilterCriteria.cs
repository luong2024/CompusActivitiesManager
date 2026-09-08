namespace CampusActivitiesManager.Models
{
    public class AccountFilterCriteria
    {
        public string SearchText { get; set; } = string.Empty;
        public AccountStatus? SelectedStatus { get; set; }
        public string SelectedClass { get; set; } = string.Empty;
        public string SelectedAcademicYear { get; set; } = string.Empty;
        public AccountRole? SelectedRole { get; set; }

        public int ActiveFilterCount
        {
            get
            {
                int count = 0;
                if (!string.IsNullOrWhiteSpace(SelectedClass) && SelectedClass != "Tất cả") count++;
                if (!string.IsNullOrWhiteSpace(SelectedAcademicYear) && SelectedAcademicYear != "Tất cả") count++;
                if (SelectedRole.HasValue) count++;
                return count;
            }
        }

        public bool HasAdvancedFilters => ActiveFilterCount > 0;

        public void Reset()
        {
            SearchText = string.Empty;
            SelectedStatus = null;
            SelectedClass = string.Empty;
            SelectedAcademicYear = string.Empty;
            SelectedRole = null;
        }

        public AccountFilterCriteria Clone()
        {
            return new AccountFilterCriteria
            {
                SearchText = this.SearchText,
                SelectedStatus = this.SelectedStatus,
                SelectedClass = this.SelectedClass,
                SelectedAcademicYear = this.SelectedAcademicYear,
                SelectedRole = this.SelectedRole
            };
        }
    }
}
