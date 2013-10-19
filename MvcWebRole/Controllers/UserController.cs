using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using jp.ne.ghopper.echo.azuredemo.Dto;
using jp.ne.ghopper.echo.azuredemo.Helper;
using jp.ne.ghopper.echo.azuredemo.Services;

namespace jp.ne.ghopper.echo.azuredemo.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly CompanyService _cs;
        private readonly GroupService _gs;
        private readonly UserService _us;
        private readonly RoleService _rs;

        public UserController()
        {
            _cs = new CompanyService();
            _gs = new GroupService();
            _us = new UserService();
            _rs = new RoleService();
        }

        //
        // GET: /User/

        public ActionResult Index()
        {
            try
            {
                InitializeDdl();
                var search = (SearchUserDto)Session[SessionKey.Search];
                var userRole = ((USER)Session[SessionKey.LogonUser]).ROLE_ID;
                ViewBag.Search = search;
                ViewBag.Results = _us.Search(search, userRole);
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        //Search
        [HttpPost]
        public ActionResult Search(FormCollection collection)
        {
            try
            {
                //検索条件を取得
                Session[SessionKey.Search] = (SearchUserDto)MapHelper.Map(collection, new SearchUserDto());
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            try
            {
                InitializeDdl();
                ViewBag.Action = "Create";
                ViewBag.ActionJA = "登録";
                return View("Edit");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                var editUser = (EditUserDto)MapHelper.Map(collection, new EditUserDto());
                _us.Add(editUser);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                InitializeDdl();
                ViewBag.Action = "Create";
                ViewBag.ActionJA = "登録";
                ViewBag.Error = ex.Message; 
                return View("Edit");
            }
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                InitializeDdl();
                var user = _us.GetById(id);
                ViewBag.COMPANYID = _gs.GetById(user.GROUP_ID).COMPANY_ID;
                ViewBag.GROUPID = user.GROUP_ID;
                ViewBag.ID = id;
                ViewBag.USERID = user.USERID;
                ViewBag.NAME = user.NAME;
                ViewBag.ROLEID = user.ROLE_ID;
                ViewBag.Action = "Edit";
                ViewBag.ActionJA = "更新";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
                var editUser = (EditUserDto)MapHelper.Map(collection, new EditUserDto());
                _us.Update(editUser);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                InitializeDdl();
                ViewBag.Action = "Edit";
                ViewBag.ActionJA = "更新";
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                _us.Delete(id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// ドロップダウン用データ取得＆設定
        /// </summary>
        private void InitializeDdl()
        {
            var companies = new List<COMPANY>();
            var groups = new List<GROUP>();
            var loginUser = (USER)Session[SessionKey.LogonUser];
            int? searchGroupId = null;

            //ログインユーザの権限によってドロップダウンの内容が変わる。
            switch (loginUser.ROLE_ID)
            {
                case (int)RoleService.Role.SystemAdmin:
                    companies.AddRange(_cs.GetAll());
                    groups.AddRange(_gs.GetAll());
                    break;
                case (int)RoleService.Role.CompanyAdmin:
                    companies.Add(_cs.GetByGroupId(loginUser.GROUP_ID));
                    groups.AddRange(_gs.GetByCompanyId(companies.First().ID));
                    break;
                default:
                    companies.Add(_cs.GetByGroupId(loginUser.GROUP_ID));
                    groups.Add(_gs.GetById(loginUser.GROUP_ID));
                    searchGroupId = loginUser.GROUP_ID;
                    break;
            }

            //ViewBag
            ViewBag.Companies = companies;
            ViewBag.Groups = groups;
            ViewBag.Roles = _rs.GetAll().Where(role => role.ID > loginUser.ROLE_ID);
            ViewBag.LoginUser = loginUser;

            //Sessionに検索条件がない場合、初期値を設定
            if (Session[SessionKey.Search] == null)
            {
                Session.Add(
                    SessionKey.Search,
                    new SearchUserDto()
                    {
                        searchCompanyId = companies.First().ID,
                        searchGroupId = searchGroupId,
                        searchRoleId = null,
                        searchName = "",
                        searchUser = ""
                    });
            }
        }
    }
}
