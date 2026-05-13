using System;
using System.Collections.Generic;
using EsportManagement.DAL;
using EsportManagement.DTO;

namespace EsportManagement.BLL
{
    public class AccountBLL
    {
        private readonly AccountDAL _dal = new AccountDAL();
        private readonly RoleDAL _roleDal = new RoleDAL();

        public Account Login(string username, string password)
        {
            throw new ArgumentException("Vui long chon vai tro Admin hoac Team Manager.");
        }

        public Account Login(string username, string password, string requiredRole)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username va mat khau khong duoc trong.");

            var loginRole = NormalizeLoginRole(requiredRole);
            var hash = PasswordHasher.Hash(password);
            try
            {
                var acc = _dal.Login(username.Trim(), hash);
                if (acc == null) return null;
                if (loginRole != null && !acc.HasRole(loginRole)) return null;
                return acc;
            }
            catch (Exception ex)
            {
                if (!DemoData.IsDatabaseUnavailable(ex)) throw;

                var acc = DemoData.Login(username.Trim(), hash);
                if (acc == null) return null;
                if (loginRole != null && !acc.HasRole(loginRole)) return null;
                return acc;
            }
        }

        private static string NormalizeLoginRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return null;
            role = role.Trim();
            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                return "Admin";
            if (string.Equals(role, "TeamManager", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "Team Manager", StringComparison.OrdinalIgnoreCase))
                return "TeamManager";
            if (string.Equals(role, "Viewer", StringComparison.OrdinalIgnoreCase))
                return "Viewer";
            throw new ArgumentException("Vai tro dang nhap khong hop le.");
        }

        public Account Register(string username, string password, string email, string fullName)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(email)    || string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Vui lòng nhập đầy đủ thông tin.");

            if (password.Length < 6)
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự.");

            if (_dal.IsUsernameTaken(username.Trim()))
                throw new InvalidOperationException("Username đã được sử dụng.");

            if (_dal.IsEmailTaken(email.Trim()))
                throw new InvalidOperationException("Email đã được sử dụng.");

            var acc = new Account
            {
                Username     = username.Trim(),
                PasswordHash = PasswordHasher.Hash(password),
                Email        = email.Trim(),
                FullName     = fullName.Trim(),
                IsLocked     = false
            };
            acc.AccountID = _dal.Insert(acc);

            // BR-ACC-02: tài khoản mới mặc định nhận vai trò Viewer
            var viewer = _roleDal.GetByName("Viewer");
            if (viewer != null) _dal.AssignRole(acc.AccountID, viewer.RoleID);

            acc.Roles = _dal.GetRoles(acc.AccountID);
            return acc;
        }

        public List<Account> GetAll()
        {
            try
            {
                return _dal.GetAll();
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.Accounts();
                throw;
            }
        }

        public Account GetByID(int id)
        {
            try
            {
                return _dal.GetByID(id);
            }
            catch (Exception ex)
            {
                if (DemoData.IsDatabaseUnavailable(ex)) return DemoData.AccountById(id);
                throw;
            }
        }

        public void Update(Account acc)
        {
            _dal.Update(acc);
        }

        public void ChangePassword(int accountId, string oldPassword, string newPassword)
        {
            if (newPassword == null || newPassword.Length < 6)
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự.");
            var acc = _dal.GetByID(accountId);
            if (acc == null) throw new InvalidOperationException("Tài khoản không tồn tại.");
            if (acc.PasswordHash != PasswordHasher.Hash(oldPassword))
                throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");
            _dal.UpdatePassword(accountId, PasswordHasher.Hash(newPassword));
        }

        public void ResetPassword(int accountId, string newPassword)
        {
            if (newPassword == null || newPassword.Length < 6)
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự.");
            _dal.UpdatePassword(accountId, PasswordHasher.Hash(newPassword));
        }

        public void Delete(int id)
        {
            _dal.Delete(id);
        }

        public void AssignRole(int accountId, int roleId)
        {
            _dal.AssignRole(accountId, roleId);
        }

        public void RemoveRole(int accountId, int roleId)
        {
            _dal.RemoveRole(accountId, roleId);
        }
    }
}
