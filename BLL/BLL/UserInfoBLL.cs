using System.Web.Mvc;
using SqlSugar;
using Entity.Models;
using DAL.DB;
using System.Collections.Generic;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.IO;

namespace BLL.BLL
{
    public class UserInfoBLL : Controller
    {
        private static readonly SqlSugarClient Db = DataBase.CreateClient();
        /// <summary>
        /// 获取用户列表.
        /// </summary>
        /// <param name="page">总页数.</param>
        /// <param name="limit">一页多少行数据.</param>
        /// <param name="search">查询字段.</param>
        /// <returns>管理员列表.</returns>
        public ActionResult GetUser(int page, int limit, string search = null)
        {
            List<UserInfo> list = null;
            int count;
            // 分页操作，Skip()跳过前面数据项
            if (string.IsNullOrEmpty(search))
            {
                // 分页操作，Skip()跳过前面数据项
                count = Db.Queryable<UserInfo>().Count();
                list = Db.Queryable<UserInfo>().Skip((page - 1) * limit).Take(limit).ToList();
            }
            else
            {
                count = Db.Queryable<UserInfo>().Where(it => it.U_Name.Contains(search) || it.Phone.Contains(search)).Count();
                list = Db.Queryable<UserInfo>().Where(it => it.U_Name.Contains(search) || it.Phone.Contains(search)).Skip((page - 1) * limit).Take(limit).ToList();
            }

            // 参数必须一一对应，JsonRequestBehavior.AllowGet一定要加，表单要求code返回0
            return Json(new { code = 0, msg = string.Empty, count, data = list }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新建用户账户.
        /// </summary>
        /// /// <param name="user">传入数据.</param>
        /// <returns>Json.</returns>
        public ActionResult AddUser(UserInfo user)
        {
            user.Create_Time = DateTime.Now.ToString();
            var isExist = Db.Queryable<UserInfo>().Where(it => it.Phone == user.Phone).Single();
            if (isExist != null)
            {
                return Json(new { code = 402 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // 自增列用法
                int userId = Db.Insertable(user).ExecuteReturnIdentity();
                user.Id = userId;
                Db.Updateable(user).ExecuteCommand();
                return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 修改用户.
        /// </summary>
        /// <param name="user">用户.</param>
        /// <returns>Json.</returns>
        public ActionResult EditUser(UserInfo user)
        {
            Db.Updateable(user).ExecuteCommand();
            return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除账户.
        /// </summary>
        /// <param name="userId">账户编号.</param>
        /// <returns>Json.</returns>
        public ActionResult DelUser(int userId)
        {
            Db.Deleteable<UserInfo>().Where(it => it.Id == userId).ExecuteCommand();
            return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 数据备份.
        /// </summary>
        /// <returns>Excel.</returns>
        public ActionResult ExportData()
        {
            // 创建Excel对象 工作薄
            HSSFWorkbook excelBook = new HSSFWorkbook();
            int j = 0;

            // 创建Excel工作表 Sheet
            ISheet sheet = excelBook.CreateSheet("数据备份");

            // 标题
            IRow user = sheet.CreateRow(j);
            user.CreateCell(0).SetCellValue("用户UserInfo");
            List<UserInfo> listUser = Db.Queryable<UserInfo>().ToList();
            IRow rowTitle = sheet.CreateRow(++j);
            rowTitle = sheet.CreateRow(++j);
            rowTitle.CreateCell(0).SetCellValue("id");
            rowTitle.CreateCell(1).SetCellValue("u_name");
            rowTitle.CreateCell(2).SetCellValue("phone");
            rowTitle.CreateCell(3).SetCellValue("level");
            rowTitle.CreateCell(4).SetCellValue("balance");
            rowTitle.CreateCell(5).SetCellValue("create_time");
            rowTitle.CreateCell(6).SetCellValue("message");

            for (int i = 0; i < listUser.Count(); i++)
            {
                IRow row = sheet.CreateRow(++j);
                row.CreateCell(0).SetCellValue(listUser[i].Id);
                row.CreateCell(1).SetCellValue(listUser[i].U_Name);
                row.CreateCell(2).SetCellValue(listUser[i].Phone);
                row.CreateCell(3).SetCellValue(listUser[i].Level);
                row.CreateCell(4).SetCellValue(listUser[i].Balance.ToString());
                row.CreateCell(5).SetCellValue(listUser[i].Create_Time);
                row.CreateCell(6).SetCellValue(listUser[i].Message);
            }

            string fileName = "数据备份-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            MemoryStream stream = new MemoryStream();
            excelBook.Write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.ms-excel", fileName);
        }
    }
}