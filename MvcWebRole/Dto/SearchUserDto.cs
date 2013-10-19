using System.Linq;
using System.Web.Mvc;

namespace jp.ne.ghopper.echo.azuredemo.Dto
{
    public class SearchUserDto
    {
        public int searchCompanyId { get; set; }

        public int? searchGroupId { get; set; }

        public string searchUser { get; set; }

        public string searchName { get; set; }

        public int? searchRoleId { get; set; }
    }
}