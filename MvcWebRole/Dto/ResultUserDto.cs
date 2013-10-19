using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jp.ne.ghopper.echo.azuredemo.Dto
{
    public class ResultUserDto
    {
        public int ID {get;set;}

        public string CompanyName { get; set; }

        public string GroupName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string RoleName { get; set; }
    }
}