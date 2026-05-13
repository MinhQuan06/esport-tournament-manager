using System;
using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class RoleBLL
    {
        private readonly RoleDAL _dal = new RoleDAL();

        public List<Role> GetAll()
        {
            try
            {
                return _dal.GetAll();
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.Roles();
                throw;
            }
        }

        public Role GetByID(int id)
        {
            try
            {
                return _dal.GetByID(id);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.RoleById(id);
                throw;
            }
        }

        public Role GetByName(string name)
        {
            try
            {
                return _dal.GetByName(name);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.RoleByName(name);
                throw;
            }
        }

        public int Create(Role role)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
                throw new ArgumentException("Tên vai trò không được trống.");
            if (_dal.GetByName(role.RoleName.Trim()) != null)
                throw new InvalidOperationException("Tên vai trò đã tồn tại.");
            role.RoleName = role.RoleName.Trim();
            return _dal.Insert(role);
        }

        public void Update(Role role)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
                throw new ArgumentException("Tên vai trò không được trống.");
            _dal.Update(role);
        }

        public void Delete(int id)
        {
            // BR-ROLE-02: không xóa 3 vai trò hệ thống
            var role = _dal.GetByID(id);
            if (role == null) throw new InvalidOperationException("Vai trò không tồn tại.");
            if (role.RoleName == "Admin" || role.RoleName == "TeamManager" || role.RoleName == "Viewer")
                throw new InvalidOperationException("Không thể xóa vai trò hệ thống.");
            var n = _dal.Delete(id);
            if (n == -1)
                throw new InvalidOperationException("Vai trò đang được sử dụng - không thể xóa. (BR-ROLE-04)");
        }
    }
}
