using System;
using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class RoleBLL
    {
        private readonly RoleDAL _dal = new RoleDAL();

        public List<Role> GetAll() => _dal.GetAll();
        public Role GetByID(int id) => _dal.GetByID(id);
        public Role GetByName(string name) => _dal.GetByName(name);

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
