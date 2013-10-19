using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jp.ne.ghopper.echo.azuredemo.Dto
{
    public class EditUserDto
    {
        public int companyId { get; set; }

        public int groupId { get; set; }

        public int id { get; set; }

        public string user { get; set; }

        public string name { get; set; }

        public string password { get; set; }

        public int roleId { get; set; }
    }
}