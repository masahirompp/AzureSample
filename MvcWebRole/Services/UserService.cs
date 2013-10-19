using System;
using System.Collections.Generic;
using System.Linq;
using jp.ne.ghopper.echo.azuredemo.Dto;
using jp.ne.ghopper.echo.azuredemo.Helper;

namespace jp.ne.ghopper.echo.azuredemo.Services
{
    public class UserService
    {
        /// <summary>
        /// 削除フラグ
        /// </summary>
        public enum DeleteFlag
        {
            /// <summary>
            /// 有効
            /// </summary>
            FALSE = 0,
            /// <summary>
            /// 無効（論理削除）
            /// </summary>
            TRUE = 1
        }

        private readonly ecocounterEntities1 _context;
        public UserService() { _context = new ecocounterEntities1(); }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private USER Add(USER user)
        {
            _context.USERS.AddObject(user);
            _context.SaveChanges();

            return user;
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="editUser"></param>
        /// <returns></returns>
        public USER Add(EditUserDto editUser)
        {
            if (!IsExistGroupId(editUser.groupId)) throw new InvalidOperationException("指定された部署が存在しません。最新の情報に更新してください。");
            if (IsExistUserId(editUser.user, editUser.companyId)) throw new ArgumentException("指定されたユーザIDは既に存在します。");

            var user = new USER()
            {
                GROUP_ID = editUser.groupId,
                USERID = editUser.user,
                NAME = editUser.name,
                ROLE_ID = editUser.roleId,
                PASSWORD = CommonHelper.HashPassword(editUser.password),
                DEL_FLAG = 0
            };

            return Add(user);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="editUser"></param>
        /// <returns></returns>
        public USER Update(EditUserDto editUser)
        {
            try
            {
                var usr = GetById(editUser.id);
                if (usr == null) throw new InvalidOperationException("更新処理に失敗しました。更新対象が存在しません。既に削除されている可能性があります。最新の情報に更新してください。");
                if (!IsExistGroupId(editUser.groupId)) throw new InvalidOperationException("指定された部署が存在しません。最新の情報に更新してください。");
                if (usr.USERID != editUser.user && IsExistUserId(editUser.user, editUser.companyId)) throw new ArgumentException("指定されたユーザIDは既に存在します。別のユーザIDを指定してください。");

                usr.USERID = editUser.user;
                usr.NAME = editUser.name;
                //usr.PASSWORD = CommonHelper.HashPassword(editUser.password);
                usr.ROLE_ID = editUser.roleId;
                usr.GROUP_ID = editUser.groupId;
                usr.APPLY_DATE = DateTime.Now;
                usr.DEL_FLAG = 0;
                usr.LOGIN_KEY = "";

                _context.SaveChanges();

                return usr;
            }
            catch (System.Data.OptimisticConcurrencyException oce)
            {
                throw new ApplicationException("更新処理に失敗しました。", oce);
            }
        }

        /// <summary>
        /// Get by Group Id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<USER> GetByGroupId(int groupId)
        {
            return _context.USERS.Where<USER>((usr) => usr.DEL_FLAG == (int)DeleteFlag.FALSE && usr.GROUP_ID == groupId).ToList<USER>();
        }

        /// <summary>
        /// Get by Company Id
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public List<USER> GetByCompanyId(int companyId)
        {
            return _context.USERS.Where(usr =>
                usr.DEL_FLAG == (int)DeleteFlag.FALSE &&
                _context.GROUPS.Where(g => g.COMPANY_ID == companyId).Select(g => g.ID).Contains(usr.GROUP_ID)).ToList();
        }

        /// <summary>
        /// Get by User Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public USER GetById(int id)
        {
            var user = _context.USERS.Where(u => u.DEL_FLAG == (int)DeleteFlag.FALSE && u.ID == id);
            if (user.Any()) return user.First(); else return null;
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<ResultUserDto> Search(SearchUserDto search, int userRole)
        {
            var results = new List<ResultUserDto>();

            if (search.searchGroupId == null)
            {
                //部門IDが指定されていない場合
                var groupIds = _context.GROUPS.Where(grp => grp.COMPANY_ID == search.searchCompanyId).Select(grp => grp.ID);
                foreach (var groupId in groupIds)
                {
                    results.AddRange(Search(groupId, search, userRole));
                }
            }
            else
            {
                //部門IDが指定されている場合
                results = Search((int)search.searchGroupId, search, userRole);
            }

            return results;
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        private List<ResultUserDto> Search(int groupId, SearchUserDto search, int userRole)
        {
            var company = _context.COMPANIES.Where(com => com.ID == search.searchCompanyId);
            if (!company.Any()) throw new ApplicationException("会社IDが不正です");

            var group = _context.GROUPS.Where(grp => grp.ID == groupId);
            if (!group.Any()) throw new ApplicationException("部門IDが不正です");

            var users = GetByGroupId(groupId).Where(usr =>
                (search.searchRoleId == null || usr.ROLE_ID == search.searchRoleId) &&
                usr.USERID.Contains(search.searchUser) &&
                usr.NAME.Contains(search.searchName) &&
                usr.ROLE_ID > userRole);

            return users.Select(usr =>
                new ResultUserDto()
                {
                    CompanyName = company.First().NAME,
                    GroupName = group.First().NAME,
                    ID = usr.ID,
                    UserName = usr.NAME,
                    UserId = usr.USERID,
                    RoleName = _context.ROLES.Where(role => role.ID == usr.ROLE_ID).First().NAME_JA
                }
                ).ToList();
        }


        /// <summary>
        /// 論理削除
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            try
            {
                var target = GetById(id);
                if (target == null) throw new InvalidOperationException("削除処理に失敗しました。削除対象が存在しません。既に削除されている可能性があります。最新の情報に更新してください。");

                target.DEL_FLAG = (int)DeleteFlag.TRUE;
                target.APPLY_DATE = DateTime.Now;
                _context.SaveChanges();
            }
            catch (System.Data.OptimisticConcurrencyException oce)
            {
                throw new ApplicationException("削除処理に失敗しました。", oce);
            }
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public USER Login(int companyId, string userId, string password)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password)) throw new ArgumentException("ユーザIDかパスワードが未入力です。");

            var company = _context.COMPANIES.Where(com => com.ID == companyId);
            if (!company.Any()) throw new ArgumentException("会社IDが不正です。管理者に問い合わせてください。");

            var groups = _context.GROUPS.Where(grp => grp.COMPANY_ID == companyId);
            if (!groups.Any()) throw new ArgumentException("部門IDが不正です。管理者に問い合わせてください。");

            var hash = CommonHelper.HashPassword(password);
            var user = _context.USERS.Where(usr =>
                usr.DEL_FLAG == (int)DeleteFlag.FALSE &&                    //ユーザは有効か
                groups.Select(grp => grp.ID).Contains(usr.GROUP_ID) &&      //会社IDの一致確認
                usr.USERID == userId &&                                     //ユーザIDの一致確認
                usr.PASSWORD == hash                                        //パスワードの一致確認
                );

            if (!user.Any()) throw new ArgumentException("ユーザIDまたはパスワードが違います。");

            if (user.First().ROLE_ID == 40) throw new ApplicationException("管理者のみログインできます。利用者はログインできません。");

            return user.First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks>
        /// groupIdはidentity列のため、会社毎にチェックする必要はなし。
        /// </remarks>
        private bool IsExistGroupId(int groupId)
        {
            return _context.GROUPS.Where(grp => grp.ID == groupId).Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="context"></param>
        /// <returns>
        /// ユーザIDは会社毎に一意。
        /// </returns>
        private bool IsExistUserId(string userId, int companyId)
        {
            return GetByCompanyId(companyId).Where(usr => usr.USERID == userId).Any();
        }
    }
}