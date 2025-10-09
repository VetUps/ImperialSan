﻿using Microsoft.AspNetCore.Mvc;

namespace ImperialSanAPI.Utils
{
    public class UsualProblemDetails
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? TraceId { get; set; }
    }
}
