using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchApi.Entity
{
    [Table("tbl_search_goods")]
    public class SearchGoods
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int no { get; set; }

        [Column("goods_no")]        
        public string goodsNo { get; set; }

        [Column("name_kr")]
        public string nameKr { get; set; }

        [Column("name_en")]
        public string nameEn { get; set; }

        public string category1 { get; set; }

        public string category2 { get; set; }

        public string category3 { get; set; }

        public int price { get; set; }

        [Column("view_cnt")]
        public int viewCnt { get; set; }

        [Column("sale_cnt")]
        public int saleCnt { get; set; }

        public string terms { get; set; }

        public string tags { get; set; }

        [Column("url_pc")]
        public string urlPc { get; set; }

        [Column("url_mobile")]
        public string urlMobile { get; set; }

        [Column("image_url1")]
        public string imageUrl1 { get; set; }

        [Column("image_url2")]
        public string imageUrl2 { get; set; }

    }
}
