﻿using System.ComponentModel.DataAnnotations;

namespace APIWelfareProcedures.Models;

public record ResponsePostParameters
{
    public string access_token { get; init; }
}