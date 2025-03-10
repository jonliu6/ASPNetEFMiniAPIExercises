﻿namespace MinimalAPIsWithASPNetEF.Entities
{
    public class Error
    {
        public Guid Id { get; set; }
        public string ErrorMessage { get; set; } = null!;

        public string? StackTrace { get; set; }

        public DateTime ErrorDate { get; set; }
    }
}
