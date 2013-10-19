using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace jp.ne.ghopper.echo.azuredemo.Dto
{
    public class LoginDto
    {
        public string userid { get; set; }

        public string password { get; set; }
    }
}