using System;

namespace OA.Domain.Enum
{
    public enum Roles
    {
        SuperAdmin,
        Admin,
        Moderator,
        Basic
    }

    public static class Constants
    {
        public static readonly string SuperAdmin = Guid.NewGuid().ToString();
        public static readonly string Admin = Guid.NewGuid().ToString();
        public static readonly string Moderator = Guid.NewGuid().ToString();
        public static readonly string Basic = Guid.NewGuid().ToString();

        public static readonly string SuperAdminUser = Guid.NewGuid().ToString();
        public static readonly string BasicUser = Guid.NewGuid().ToString();
    }


}
