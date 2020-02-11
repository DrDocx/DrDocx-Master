using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DrDocx_Core.Models;
using System.Text.Json;
using ReportGen;

namespace DrDocx_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public ReportController(DatabaseContext context)
        {
            _context = context;
        }
    }
}