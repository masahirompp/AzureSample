using System;
using System.Collections.Generic;
using System.Linq;

namespace jp.ne.ghopper.echo.azuredemo.Services
{
    public class GroupService
    {
        private readonly ecocounterEntities1 _context;
        public GroupService() { _context = new ecocounterEntities1(); }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public GROUP Add(GROUP group)
        {
            if (!IsExistCompanyId(group.COMPANY_ID)) throw new InvalidOperationException();
            _context.AddObject(typeof(GROUP).Name, group);
            _context.SaveChanges();
            return group;
        }

        /// <summary>
        /// Get by company id
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<GROUP> GetByCompanyId(int companyId)
        {
            return _context.GROUPS.Where((grp) => grp.COMPANY_ID == companyId).ToList<GROUP>();
        }

        /// <summary>
        /// Get all.
        /// </summary>
        /// <returns></returns>
        public List<GROUP> GetAll()
        {
            return _context.GROUPS.ToList<GROUP>();
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="companyId"></param>
        public void Update(int id, string name, int companyId)
        {
            try
            {
                var group = GetById(id);
                if (!IsExistCompanyId(companyId)) throw new ArgumentException("指定された会社は存在しません。最新の情報に更新してください。");
                if (group == null) throw new InvalidOperationException("更新対象の部署が存在しません。最新の情報に更新してください。");

                group.NAME = name;
                group.COMPANY_ID = companyId;

                _context.SaveChanges();
            }
            catch (System.Data.OptimisticConcurrencyException oce)
            {
                throw new ApplicationException("更新処理に失敗しました。", oce);
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void Delete(int id)
        {
            try
            {
                var group = GetById(id);
                if (group == null) throw new InvalidOperationException("削除対象の部署が存在しません。既に削除されている可能性があります。最新の情報に更新してください。");
                if (IsExistChildUser(group.ID)) throw new InvalidOperationException("削除対象の部署に所属するユーザが存在します。ユーザを削除してから部署を削除してください。");

                _context.DeleteObject(group);
                _context.SaveChanges();
            }
            catch (System.Data.OptimisticConcurrencyException oce)
            {
                throw new ApplicationException("削除処理に失敗しました。", oce);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsExistCompanyId(int companyId)
        {
            return _context.COMPANIES.Where((com) => com.ID == companyId).Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public GROUP GetById(int id)
        {
            var results = _context.GROUPS.Where((grp) => grp.ID == id);

            if (results.Any())
            {
                return results.First<GROUP>();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 子ユーザの存在確認
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsExistChildUser(int groupId)
        {
            return _context.USERS.Where((usr) => usr.DEL_FLAG == (int)UserService.DeleteFlag.FALSE && usr.GROUP_ID == groupId).Any();
        }
    }
}