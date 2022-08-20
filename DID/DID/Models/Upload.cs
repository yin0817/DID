namespace DID.Models
{
    /// <summary>
    /// 图片类型 
    /// </summary>
    public enum ImageType{ 人像面, 国徽面, 手持照片 }
    public class Upload
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UId { get; set; }
        /// <summary>
        /// 图片类型
        /// </summary>
        public ImageType Type { get; set; }
        
    }
}
