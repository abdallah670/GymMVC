using System;

namespace GymBLL.ModelVM.Member
{
    public class WeightLogVM
    {
        public int Id { get; set; }
        public string MemberId { get; set; }
        public double Weight { get; set; }
        public DateTime DateRecorded { get; set; }
        public string? Notes { get; set; }
    }
}
