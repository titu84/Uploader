﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Uploader.Models
{
    public class DTO
    {      
        public IFormFile File { get; set; }
        public string Path { get; set; }
    }
}
