using SqlSugar;

namespace Entity.Models
{
    /// <summary>
    /// 用户实体类.
    /// </summary>
    [SugarTable("userinfo")]
    public partial class UserInfo
    {
        // 指定主键和自增列
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

       
        public string U_Name { get; set; }

        
        public string Phone { get; set; }

        
        public int Level { get; set; }

        
        public decimal Balance { get; set; }

        
        public string Create_Time { get; set; }

        
        public string Message { get; set; }
    }
}
