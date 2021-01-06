namespace TheTenantManager
{
    public class TenantsInfo
    {
        public int ID { get; set; }
        public int RoomNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string LeaseStart { get; set; }
        public string LeaseEnd { get; set; }
        public string FreeWeek { get; set; }
        public string FreeWeekDate { get; set; }
        public string OperatorLicense { get; set; }
        public string OperatorLicEXPDate { get; set; }
        public string MiniSalonLicense { get; set; }
        public string MiniSalonLicEXPDate { get; set; }
    }
}