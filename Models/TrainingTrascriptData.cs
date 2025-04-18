﻿namespace LMS_Project_APIs.Models
{
    public class TrainingTrascriptData
    {
        public int? transcript_id {  get; set; }
        public string? training_name { get; set; }
        public string? summary { get; set; } 
        public string? training_hours { get; set; }
        public string? training_code{ get; set; }
        public string? external_link_URL { get; set; }
        public string? trainingtype_name { get; set; }
        public string? status_name { get; set; }
        public DateTime? enroll_date { get; set; }
        public DateTime? completion_date { get; set; }


    }
}
