namespace TDCAPP_BOXWEB.Server.Models
{
    public class DcOptionDto
    {
        public string value { get; set; } = string.Empty;
        public string label { get; set; } = string.Empty;
    }

    public class BoxBarcodeRequest
    {
        public string? P_DCNO { get; set; }
        public string? P_DIS_NO { get; set; }
        public string? P_DEL_TYPE { get; set; }
        public string? P_DEL_STORE_DATE { get; set; }
        public string? P_CAR_ROUTE { get; set; }
        public string? P_USERID { get; set; }
    }

    public class BoxBarcodeDto
    {
        public string? DISTR_DC_NO { get; set; }
        public string? PACK_DC_NO { get; set; }
        public string? PACK_AREA { get; set; }
        public string? DEL_STORE_DATE { get; set; }
        public string? ROUTE_NO { get; set; }
        public string? CAR_NO { get; set; }
        public string? STORE_NO { get; set; }
        public string? ORIG_STORE_NO { get; set; }
        public string? PACK_BOX_NO { get; set; }
    }

    public class CheckStackRequest
    {
        public string? P_DCNO { get; set; }
        public string? P_DIS_NO { get; set; }
        public string? P_DEL_STORE_DATE { get; set; }
        public string? P_CAR_ROUTE { get; set; }
        public string? P_USERID { get; set; }
    }

    public class CheckStackDto
    {
        public string? DC_NO { get; set; }
        public string? DEL_STORE_DATE { get; set; }
        public string? C_NO { get; set; }
        public string? STORE_NO { get; set; }
        public string? STORE_NAME { get; set; }
        public string? PACK_BOX { get; set; }
        public string? UP_BOX { get; set; }
        public string? DATA_TYPE { get; set; }
    }

    public class BoxStackRequest
    {
        public string? A_DATE { get; set; }
        public string? P_DCNO { get; set; }
        public string? P_CAR_ROUTE { get; set; }
        public string? P_DEL_TYPE { get; set; }
        public string? ROUTE_NO { get; set; }
        public string? CAR_NO { get; set; }
        public string? STORE_NO { get; set; }
        public string? P_BOX_NO { get; set; }
        public string? DATA_TYPE { get; set; }
        public string? SCAN_TIME { get; set; }
        public string? P_USERID { get; set; }
        public string? USER_NAME { get; set; }
        public string? BATCH_ID { get; set; }
    }

    public class BoxUnloadRequest
    {
        public string? A_DATE { get; set; }
        public string? P_DCNO { get; set; }
        public string? P_CAR_ROUTE { get; set; }
        public string? P_DEL_TYPE { get; set; }
        public string? ROUTE_NO { get; set; }
        public string? CAR_NO { get; set; }
        public string? STORE_NO { get; set; }
        public string? P_BOX_NO { get; set; }
        public string? DATA_TYPE { get; set; }
        public string? SCAN_TIME { get; set; }
        public string? P_USERID { get; set; }
        public string? USER_NAME { get; set; }
        public string? BATCH_ID { get; set; }
    }

    public class BoxStoreDto
    {
        public string? A_DATE { get; set; }
        public string? DC_NO { get; set; }
        public string? DIS_NO { get; set; }
        public string? STORE_NO { get; set; }
        public string? ORIG_STORE_NO { get; set; }
    }

    public class AreaDelTypeDto
    {
        public string? Area_Id { get; set; }
        public string? Area_DelType { get; set; }
        public string? PDA_DelType { get; set; }
        public string? SCAN_HISDAY { get; set; }
        public string? ADDSCAN_HISDAY { get; set; }
    }

    public class BoxDeleteRequest
    {
        public string? P_DCNO { get; set; }
        public string? P_DEL_STORE_DATE { get; set; }
        public string? P_CAR_ROUTE { get; set; }
        public string? P_STORE_NO { get; set; }
        public string? P_USERID { get; set; }
    }

    public class BoxDeleteDto
    {
        public string? RESULT_CODE { get; set; }
        public string? RESULT_DESC { get; set; }
    }

    public class StoreRequest
    {
        public string? P_DCNO { get; set; }
        public string? P_DIS_NO { get; set; }
        public string? P_DEL_STORE_DATE { get; set; }
        public string? P_USERID { get; set; }
    }
}