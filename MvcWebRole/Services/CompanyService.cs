using System.Collections.Generic;
using System.Linq;
using System;

namespace jp.ne.ghopper.echo.azuredemo.Services
{
    public class CompanyService
    {
        private readonly ecocounterEntities1 _context;
        public CompanyService() { _context = new ecocounterEntities1(); }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public COMPANY Add(COMPANY company)
        {
            _context.COMPANIES.AddObject(company);
            _context.SaveChanges();
            return company;
        }

        /// <summary>
        /// Get all company
        /// </summary>
        /// <returns></returns>
        public List<COMPANY> GetAll()
        {
            return _context.COMPANIES.ToList<COMPANY>();
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public void Update(int id, string name)
        {
            try
            {
                var company = GetById(id);
                if (company == null) throw new InvalidOperationException("更新対象の会社が存在しません。最新の情報に更新してください。");

                company.NAME = name;
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
                var company = GetById(id);
                if (company == null) throw new InvalidOperationException("削除対象の会社が存在しません。最新の情報に更新してください。");
                if (IsExistChildGroup(company.ID)) throw new InvalidOperationException("削除対象の会社に部署またはユーザが存在するため削除できません。");

                _context.DeleteObject(company);
                _context.SaveChanges();
            }
            catch (System.Data.OptimisticConcurrencyException oce)
            {
                throw new ApplicationException("削除処理に失敗しました。", oce);
            }
        }

        /// <summary>
        /// 会社IDから会社エンティティ取得
        /// </summary>
        /// <param name="id"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public COMPANY GetById(int id)
        {
            var results = _context.COMPANIES.Where(com => com.ID == id);
            if (!results.Any()) return null;
            return results.First<COMPANY>();
        }

        /// <summary>
        /// 部門IDから会社エンティティ取得
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public COMPANY GetByGroupId(int groupId)
        {
            var group = _context.GROUPS.Where(grp => grp.ID == groupId);
            if (!group.Any()) return null;
            return GetById(group.First().COMPANY_ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsExistChildGroup(int companyId)
        {
            return _context.GROUPS.Where((grp) => grp.COMPANY_ID == companyId).Any();
        }
    }
}