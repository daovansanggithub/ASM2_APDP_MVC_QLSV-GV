﻿using System;
using System.Collections.Generic;

namespace WebApplication10.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public virtual Teacher? Teacher { get; set; }
}

