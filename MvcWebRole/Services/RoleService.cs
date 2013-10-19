using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jp.ne.ghopper.echo.azuredemo.Services
{
    public class RoleService
    {
        private readonly ecocounterEntities1 _context;
        public RoleService() { _context = new ecocounterEntities1(); }

        public enum Role
        {
            SystemAdmin = 10,
            CompanyAdmin = 20,
            GroupAdmin = 30,
            User = 40
        }

        public List<ROLE> GetAll()
        {
            return _context.ROLES.OrderBy(r => r.ID).ToList();
        }
    }
}