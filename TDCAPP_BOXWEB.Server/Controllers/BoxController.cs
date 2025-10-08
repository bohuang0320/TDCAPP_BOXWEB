using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using TDCAPP_BOXWEB.Server.Models;

[ApiController]
[Route("api/[controller]")]
public class BoxController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<BoxController> _logger;

    public BoxController(IConfiguration config, ILogger<BoxController> logger)
    {
        _config = config;
        _logger = logger;
    }

    // ========== 1. dclist API ==========
    [HttpGet("dclist")]
    public async Task<IActionResult> GetDcList()
    {
        string apiName = nameof(GetDcList);
        _logger.LogInformation("{ApiName} 呼叫成功", apiName);

        var connStr = _config.GetConnectionString("c2c");
        var resultList = new List<DcOptionDto>();
        string resultCode = "";
        string resultDesc = "";

        using var conn = new OracleConnection(connStr);
        await conn.OpenAsync();
        using var cmd = new OracleCommand("tdcapp_box.dclist", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        var cursor = new OracleParameter("P_RESULT1", OracleDbType.RefCursor, ParameterDirection.Output);
        var code = new OracleParameter("RESULT_CODE", OracleDbType.Varchar2, 5) { Direction = ParameterDirection.Output };
        var desc = new OracleParameter("RESULT_DESC", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };

        cmd.Parameters.Add(cursor);
        cmd.Parameters.Add(code);
        cmd.Parameters.Add(desc);

        await cmd.ExecuteNonQueryAsync();

        resultCode = code.Value?.ToString() ?? string.Empty;
        resultDesc = desc.Value?.ToString() ?? string.Empty;

        if (cursor.Value != null && resultCode == "00")
        {
            using var reader = ((OracleRefCursor)cursor.Value).GetDataReader();
            while (reader.Read())
            {
                resultList.Add(new DcOptionDto
                {
                    value = reader["dc_no"]?.ToString() ?? string.Empty,
                    label = reader["dc_name"]?.ToString() ?? string.Empty
                });
            }
        }

        _logger.LogInformation("{ApiName} 呼叫結束", apiName);

        return Ok(new
        {
            resultCode,
            resultDesc,
            dcList = resultList
        });
    }

    // ========== 2. boxbarcode ==========
    [HttpPost("boxbarcode")]
    public async Task<IActionResult> BoxBarcode([FromBody] BoxBarcodeRequest req)
    {
        string apiName = nameof(BoxBarcode);
        _logger.LogInformation("{ApiName} 呼叫成功，UserId={UserId}", apiName, req.P_USERID);

        var connStr = _config.GetConnectionString("c2c");
        var resultList = new List<BoxBarcodeDto>();
        string resultCode = "";
        string resultDesc = "";

        using var conn = new OracleConnection(connStr);
        await conn.OpenAsync();
        using var cmd = new OracleCommand("tdcapp_box.EC_BOX_BARCODE", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("P_DCNO", OracleDbType.Varchar2).Value = req.P_DCNO ?? "";
        cmd.Parameters.Add("P_DIS_NO", OracleDbType.Varchar2).Value = req.P_DIS_NO ?? "";
        cmd.Parameters.Add("P_DEL_TYPE", OracleDbType.Varchar2).Value = req.P_DEL_TYPE ?? "";
        cmd.Parameters.Add("P_DEL_STORE_DATE", OracleDbType.Varchar2).Value = req.P_DEL_STORE_DATE ?? "";
        cmd.Parameters.Add("P_CAR_ROUTE", OracleDbType.Varchar2).Value = req.P_CAR_ROUTE ?? "";
        cmd.Parameters.Add("P_USERID", OracleDbType.Varchar2).Value = req.P_USERID ?? "";

        var cursor = new OracleParameter("P_RESULT1", OracleDbType.RefCursor, ParameterDirection.Output);
        var code = new OracleParameter("RESULT_CODE", OracleDbType.Varchar2, 5) { Direction = ParameterDirection.Output };
        var desc = new OracleParameter("RESULT_DESC", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(cursor);
        cmd.Parameters.Add(code);
        cmd.Parameters.Add(desc);

        await cmd.ExecuteNonQueryAsync();

        resultCode = code.Value?.ToString() ?? string.Empty;
        resultDesc = desc.Value?.ToString() ?? string.Empty;

        if (cursor.Value != null && resultCode == "00")
        {
            using var reader = ((OracleRefCursor)cursor.Value).GetDataReader();
            while (reader.Read())
            {
                resultList.Add(new BoxBarcodeDto
                {
                    DISTR_DC_NO = reader["DISTR_DC_NO"]?.ToString(),
                    PACK_DC_NO = reader["PACK_DC_NO"]?.ToString(),
                    PACK_AREA = reader["PACK_AREA"]?.ToString(),
                    DEL_STORE_DATE = reader["DEL_STORE_DATE"]?.ToString(),
                    ROUTE_NO = reader["ROUTE_NO"]?.ToString(),
                    CAR_NO = reader["CAR_NO"]?.ToString(),
                    STORE_NO = reader["STORE_NO"]?.ToString(),
                    ORIG_STORE_NO = reader["ORIG_STORE_NO"]?.ToString(),
                    PACK_BOX_NO = reader["PACK_BOX_NO"]?.ToString()
                });
            }
        }

        _logger.LogInformation("{ApiName} 呼叫結束", apiName);

        return Ok(new { RESULT_CODE = resultCode, RESULT_DESC = resultDesc, P_RESULT1 = resultList });
    }

    // ========== 3. 疊貨明細查詢 ==========
    [HttpPost("checkstack")]
    public async Task<IActionResult> CheckStack([FromBody] CheckStackRequest req)
    {
        string apiName = nameof(CheckStack);
        _logger.LogInformation("{ApiName} 呼叫成功", apiName);

        var connStr = _config.GetConnectionString("c2c");
        var resultList = new List<CheckStackDto>();
        string resultCode = "";
        string resultDesc = "";

        using var conn = new OracleConnection(connStr);
        await conn.OpenAsync();
        using var cmd = new OracleCommand("tdcapp_box.EC_BOX_CHECK", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("P_DCNO", OracleDbType.Varchar2).Value = req.P_DCNO ?? "";
        cmd.Parameters.Add("P_DIS_NO", OracleDbType.Varchar2).Value = req.P_DIS_NO ?? "";
        cmd.Parameters.Add("P_DEL_STORE_DATE", OracleDbType.Varchar2).Value = req.P_DEL_STORE_DATE ?? "";
        cmd.Parameters.Add("P_CAR_ROUTE", OracleDbType.Varchar2).Value = req.P_CAR_ROUTE ?? "";
        cmd.Parameters.Add("P_USERID", OracleDbType.Varchar2).Value = req.P_USERID ?? "";
        cmd.Parameters.Add("P_DATATYPE", OracleDbType.Varchar2).Value = "S"; // 疊貨

        var cursor = new OracleParameter("P_RESULT1", OracleDbType.RefCursor, ParameterDirection.Output);
        var code = new OracleParameter("RESULT_CODE", OracleDbType.Varchar2, 5) { Direction = ParameterDirection.Output };
        var desc = new OracleParameter("RESULT_DESC", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(cursor);
        cmd.Parameters.Add(code);
        cmd.Parameters.Add(desc);

        await cmd.ExecuteNonQueryAsync();

        resultCode = code.Value?.ToString() ?? string.Empty;
        resultDesc = desc.Value?.ToString() ?? string.Empty;

        if (cursor.Value != null && resultCode == "00")
        {
            using var reader = ((OracleRefCursor)cursor.Value).GetDataReader();
            while (reader.Read())
            {
                resultList.Add(new CheckStackDto
                {
                    DC_NO = reader["DC_NO"]?.ToString(),
                    DEL_STORE_DATE = reader["DEL_STORE_DATE"]?.ToString(),
                    C_NO = reader["C_NO"]?.ToString(),
                    STORE_NO = reader["STORE_NO"]?.ToString(),
                    STORE_NAME = reader["STORE_NAME"]?.ToString(),
                    PACK_BOX = reader["PACK_BOX"]?.ToString(),
                    UP_BOX = reader["UP_BOX"]?.ToString(),
                    DATA_TYPE = reader["DATA_TYPE"]?.ToString()
                });
            }
        }

        _logger.LogInformation("{ApiName} 呼叫結束", apiName);

        return Ok(new { RESULT_CODE = resultCode, RESULT_DESC = resultDesc, P_RESULT1 = resultList });
    }

    // ========== 4. 卸貨明細查詢 ==========
    [HttpPost("checkunload")]
    public async Task<IActionResult> CheckUnload([FromBody] CheckStackRequest req)
    {
        string apiName = nameof(CheckUnload);
        _logger.LogInformation("{ApiName} 呼叫成功", apiName);

        var connStr = _config.GetConnectionString("c2c");
        var resultList = new List<CheckStackDto>();
        string resultCode = "";
        string resultDesc = "";

        using var conn = new OracleConnection(connStr);
        await conn.OpenAsync();
        using var cmd = new OracleCommand("tdcapp_box.EC_BOX_CHECK", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("P_DCNO", OracleDbType.Varchar2).Value = req.P_DCNO ?? "";
        cmd.Parameters.Add("P_DIS_NO", OracleDbType.Varchar2).Value = req.P_DIS_NO ?? "";
        cmd.Parameters.Add("P_DEL_STORE_DATE", OracleDbType.Varchar2).Value = req.P_DEL_STORE_DATE ?? "";
        cmd.Parameters.Add("P_CAR_ROUTE", OracleDbType.Varchar2).Value = req.P_CAR_ROUTE ?? "";
        cmd.Parameters.Add("P_USERID", OracleDbType.Varchar2).Value = req.P_USERID ?? "";
        cmd.Parameters.Add("P_DATATYPE", OracleDbType.Varchar2).Value = "U"; // 卸貨

        var cursor = new OracleParameter("P_RESULT1", OracleDbType.RefCursor, ParameterDirection.Output);
        var code = new OracleParameter("RESULT_CODE", OracleDbType.Varchar2, 5) { Direction = ParameterDirection.Output };
        var desc = new OracleParameter("RESULT_DESC", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(cursor);
        cmd.Parameters.Add(code);
        cmd.Parameters.Add(desc);

        await cmd.ExecuteNonQueryAsync();

        resultCode = code.Value?.ToString() ?? string.Empty;
        resultDesc = desc.Value?.ToString() ?? string.Empty;

        if (cursor.Value != null && resultCode == "00")
        {
            using var reader = ((OracleRefCursor)cursor.Value).GetDataReader();
            while (reader.Read())
            {
                resultList.Add(new CheckStackDto
                {
                    DC_NO = reader["DC_NO"]?.ToString(),
                    DEL_STORE_DATE = reader["DEL_STORE_DATE"]?.ToString(),
                    C_NO = reader["C_NO"]?.ToString(),
                    STORE_NO = reader["STORE_NO"]?.ToString(),
                    STORE_NAME = reader["STORE_NAME"]?.ToString(),
                    PACK_BOX = reader["PACK_BOX"]?.ToString(),
                    UP_BOX = reader["UP_BOX"]?.ToString(),
                    DATA_TYPE = reader["DATA_TYPE"]?.ToString()
                });
            }
        }

        _logger.LogInformation("{ApiName} 呼叫結束", apiName);

        return Ok(new { RESULT_CODE = resultCode, RESULT_DESC = resultDesc, P_RESULT1 = resultList });
    }

    // ========== 5. 疊貨資料上傳（EC_BOX_STACK）==========
    [HttpPost("stack")]
    public async Task<IActionResult> BoxStack([FromBody] BoxStackRequest req)
    {
        string apiName = nameof(BoxStack);
        _logger.LogInformation("{ApiName} 呼叫成功，UserId={UserId}", apiName, req.P_USERID);

        string prod = _config["AppSettings:PROD"] ?? "1";
        string SCANTable = (prod == "1") ? "EC_BOX_STACK" : "EC_BOX_STACK_TEST";
        string SCANLogTable = (prod == "1") ? "EC_BOX_STACK_LOG" : "EC_BOX_STACK_LOG_TEST";        
        string? connStr = _config.GetConnectionString((prod == "1") ? "c2c" : "c2ctest");
        if (string.IsNullOrEmpty(connStr))
            throw new InvalidOperationException("Connection string not found.");

        string checkBatchIdSql = $"SELECT COUNT(0) FROM {SCANLogTable} WHERE BATCH_ID = :BATCH_ID";
        string checkBoxNoSql = $"SELECT COUNT(0) FROM {SCANLogTable} WHERE DC_NO = :DC_NO AND A_DATE = TO_DATE(:A_DATE,'YYYYMMDD') AND CAR_ROUTE = :CAR_ROUTE AND PACK_BOX_NO = :PACK_BOX_NO";
        string updateScanSql = $@"
UPDATE {SCANTable}
SET DEL_TYPE = :DEL_TYPE,
    ROUTE_NO = :ROUTE_NO,
    CAR_NO = :CAR_NO,
    STORE_NO = :STORE_NO,
    DATA_TYPE = :DATA_TYPE,
    SCAN_TIME = :SCAN_TIME,
    USER_ID = :USER_ID,
    USER_NAME = :USER_NAME,
    UPDATE_DATE = :UPDATE_DATE
WHERE DC_NO = :DC_NO
  AND A_DATE = TO_DATE(:A_DATE,'YYYYMMDD')
  AND CAR_ROUTE = :CAR_ROUTE
  AND PACK_BOX_NO = :PACK_BOX_NO";
        string insertScanSql = $@"
INSERT INTO {SCANTable}
    (A_DATE, DC_NO, CAR_ROUTE, DEL_TYPE, ROUTE_NO, CAR_NO, STORE_NO, PACK_BOX_NO, DATA_TYPE, SCAN_TIME, USER_ID, USER_NAME, UPDATE_DATE)
VALUES
    (TO_DATE(:A_DATE,'YYYYMMDD'), :DC_NO, :CAR_ROUTE, :DEL_TYPE, :ROUTE_NO, :CAR_NO, :STORE_NO, :PACK_BOX_NO, :DATA_TYPE, TO_DATE(:SCAN_TIME, 'YYYYMMDD HH24MISS'), :USER_ID, :USER_NAME, :UPDATE_DATE)";
        string insertScanLogSql = $@"
INSERT INTO {SCANLogTable}
    (A_DATE, DC_NO, CAR_ROUTE, DEL_TYPE, ROUTE_NO, CAR_NO, STORE_NO, PACK_BOX_NO, DATA_TYPE, SCAN_TIME, USER_ID, USER_NAME, UPDATE_DATE, BATCH_ID, INPUT)
VALUES
    (TO_DATE(:A_DATE,'YYYYMMDD'), :DC_NO, :CAR_ROUTE, :DEL_TYPE, :ROUTE_NO, :CAR_NO, :STORE_NO, :PACK_BOX_NO, :DATA_TYPE, TO_DATE(:SCAN_TIME, 'YYYYMMDD HH24MISS'), :USER_ID, :USER_NAME, :UPDATE_DATE, :BATCH_ID, :INPUT)";
        string batchId = req.BATCH_ID ?? Guid.NewGuid().ToString("N");
        string resultCode = "00";
        string resultDesc = "";
        try
        {
            using var conn = new OracleConnection(connStr);
            await conn.OpenAsync();
            using var trans = conn.BeginTransaction();
            try
            {
                // 檢查 BatchId
                using (var cmd = new OracleCommand(checkBatchIdSql, conn))
                {
                    cmd.Transaction = trans;
                    cmd.Parameters.Add(":BATCH_ID", OracleDbType.Varchar2).Value = batchId;
                    if (Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0)
                        return Ok(new { RESULT_CODE = "01", RESULT_DESC = "重複上傳", BATCH_ID = batchId });
                }
                // 檢查 BoxNo
                using (var cmd = new OracleCommand(checkBoxNoSql, conn))
                {
                    cmd.Transaction = trans;
                    cmd.Parameters.Add(":DC_NO", OracleDbType.Varchar2).Value = req.P_DCNO ?? "";
                    cmd.Parameters.Add(":A_DATE", OracleDbType.Varchar2).Value = req.A_DATE ?? "";
                    cmd.Parameters.Add(":CAR_ROUTE", OracleDbType.Varchar2).Value = req.P_CAR_ROUTE ?? "";
                    cmd.Parameters.Add(":PACK_BOX_NO", OracleDbType.Varchar2).Value = req.P_BOX_NO ?? "";
                    if (Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0)
                        return Ok(new { RESULT_CODE = "01", RESULT_DESC = "此籃重複過刷上傳", BATCH_ID = batchId });
                }

                // update，如果沒資料再 insert
                using (var cmd = new OracleCommand(updateScanSql, conn))
                {
                    cmd.Transaction = trans;
                    AddStackParams(cmd, req, batchId, true);
                    int affected = await cmd.ExecuteNonQueryAsync();
                    if (affected == 0)
                    {
                        cmd.CommandText = insertScanSql;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                // insert log
                using (var cmd = new OracleCommand(insertScanLogSql, conn))
                {
                    cmd.Transaction = trans;
                    AddStackParams(cmd, req, batchId, false);
                    cmd.Parameters.Add(":INPUT", OracleDbType.Varchar2).Value = "BOXAPP";
                    await cmd.ExecuteNonQueryAsync();
                }
                await trans.CommitAsync();
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                _logger.LogError(ex, "{ApiName} 寫入失敗", apiName);
                resultCode = "99";
                resultDesc = ex.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ApiName} 連線失敗", apiName);
            resultCode = "99";
            resultDesc = ex.Message;
        }
        _logger.LogInformation("{ApiName} 呼叫結束", apiName);
        return Ok(new { RESULT_CODE = resultCode, RESULT_DESC = resultDesc, BATCH_ID = batchId });
    }

    // ========== 6. 卸貨資料上傳（EC_BOX_UNLOAD）==========
    [HttpPost("unload")]
    public async Task<IActionResult> BoxUnload([FromBody] BoxUnloadRequest req)
    {
        string apiName = nameof(BoxUnload);
        _logger.LogInformation("{ApiName} 呼叫成功，UserId={UserId}", apiName, req.P_USERID);

        string prod = _config["AppSettings:PROD"] ?? "1";
        string SCANTable = (prod == "1") ? "EC_BOX_UNLOAD" : "EC_BOX_UNLOAD_TEST";
        string SCANLogTable = (prod == "1") ? "EC_BOX_UNLOAD_LOG" : "EC_BOX_UNLOAD_LOG_TEST";
        string? connStr = _config.GetConnectionString((prod == "1") ? "c2c" : "c2ctest");
        if (string.IsNullOrEmpty(connStr))
            throw new InvalidOperationException("Connection string not found.");
        string checkBatchIdSql = $"SELECT COUNT(0) FROM {SCANLogTable} WHERE BATCH_ID = :BATCH_ID";
        string checkBoxNoSql = $"SELECT COUNT(0) FROM {SCANLogTable} WHERE DC_NO = :DC_NO AND A_DATE = TO_DATE(:A_DATE,'YYYYMMDD') AND CAR_ROUTE = :CAR_ROUTE AND PACK_BOX_NO = :PACK_BOX_NO";
        string updateScanSql = $@"
UPDATE {SCANTable}
SET DEL_TYPE = :DEL_TYPE,
    ROUTE_NO = :ROUTE_NO,
    CAR_NO = :CAR_NO,
    STORE_NO = :STORE_NO,
    DATA_TYPE = :DATA_TYPE,
    SCAN_TIME = :SCAN_TIME,
    USER_ID = :USER_ID,
    USER_NAME = :USER_NAME,
    UPDATE_DATE = :UPDATE_DATE
WHERE DC_NO = :DC_NO
  AND A_DATE = TO_DATE(:A_DATE,'YYYYMMDD')
  AND CAR_ROUTE = :CAR_ROUTE
  AND PACK_BOX_NO = :PACK_BOX_NO";
        string insertScanSql = $@"
INSERT INTO {SCANTable}
    (A_DATE, DC_NO, CAR_ROUTE, DEL_TYPE, ROUTE_NO, CAR_NO, STORE_NO, PACK_BOX_NO, DATA_TYPE, SCAN_TIME, USER_ID, USER_NAME, UPDATE_DATE)
VALUES
    (TO_DATE(:A_DATE,'YYYYMMDD'), :DC_NO, :CAR_ROUTE, :DEL_TYPE, :ROUTE_NO, :CAR_NO, :STORE_NO, :PACK_BOX_NO, :DATA_TYPE, TO_DATE(:SCAN_TIME, 'YYYYMMDD HH24MISS'), :USER_ID, :USER_NAME, :UPDATE_DATE)";
        string insertScanLogSql = $@"
INSERT INTO {SCANLogTable}
    (A_DATE, DC_NO, CAR_ROUTE, DEL_TYPE, ROUTE_NO, CAR_NO, STORE_NO, PACK_BOX_NO, DATA_TYPE, SCAN_TIME, USER_ID, USER_NAME, UPDATE_DATE, BATCH_ID, INPUT)
VALUES
    (TO_DATE(:A_DATE,'YYYYMMDD'), :DC_NO, :CAR_ROUTE, :DEL_TYPE, :ROUTE_NO, :CAR_NO, :STORE_NO, :PACK_BOX_NO, :DATA_TYPE, TO_DATE(:SCAN_TIME, 'YYYYMMDD HH24MISS'), :USER_ID, :USER_NAME, :UPDATE_DATE, :BATCH_ID, :INPUT)";
        string batchId = req.BATCH_ID ?? Guid.NewGuid().ToString("N");
        string resultCode = "00";
        string resultDesc = "";
        try
        {
            using var conn = new OracleConnection(connStr);
            await conn.OpenAsync();
            using var trans = conn.BeginTransaction();
            try
            {
                // 檢查 BatchId
                using (var cmd = new OracleCommand(checkBatchIdSql, conn))
                {
                    cmd.Transaction = trans;
                    cmd.Parameters.Add(":BATCH_ID", OracleDbType.Varchar2).Value = batchId;
                    if (Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0)
                        return Ok(new { RESULT_CODE = "01", RESULT_DESC = "重複上傳", BATCH_ID = batchId });
                }
                // 檢查 BoxNo
                using (var cmd = new OracleCommand(checkBoxNoSql, conn))
                {
                    cmd.Transaction = trans;
                    cmd.Parameters.Add(":DC_NO", OracleDbType.Varchar2).Value = req.P_DCNO ?? "";
                    cmd.Parameters.Add(":A_DATE", OracleDbType.Varchar2).Value = req.A_DATE ?? "";
                    cmd.Parameters.Add(":CAR_ROUTE", OracleDbType.Varchar2).Value = req.P_CAR_ROUTE ?? "";
                    cmd.Parameters.Add(":PACK_BOX_NO", OracleDbType.Varchar2).Value = req.P_BOX_NO ?? "";
                    if (Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0)
                        return Ok(new { RESULT_CODE = "01", RESULT_DESC = "此籃重複過刷上傳", BATCH_ID = batchId });
                }

                // update，如果沒資料再 insert
                using (var cmd = new OracleCommand(updateScanSql, conn))
                {
                    cmd.Transaction = trans;
                    AddUnloadParams(cmd, req, batchId, true);
                    int affected = await cmd.ExecuteNonQueryAsync();
                    if (affected == 0)
                    {
                        cmd.CommandText = insertScanSql;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                // insert log
                using (var cmd = new OracleCommand(insertScanLogSql, conn))
                {
                    cmd.Transaction = trans;
                    AddUnloadParams(cmd, req, batchId, false);
                    cmd.Parameters.Add(":INPUT", OracleDbType.Varchar2).Value = "BOXAPP";
                    await cmd.ExecuteNonQueryAsync();
                }
                await trans.CommitAsync();
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                _logger.LogError(ex, "{ApiName} 寫入失敗", apiName);
                resultCode = "99";
                resultDesc = ex.Message;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ApiName} 連線失敗", apiName);
            resultCode = "99";
            resultDesc = ex.Message;
        }
        _logger.LogInformation("{ApiName} 呼叫結束", apiName);
        return Ok(new { RESULT_CODE = resultCode, RESULT_DESC = resultDesc, BATCH_ID = batchId });
    }

    // ========== 7. 原始店號下載 ==========
    [HttpPost("store")]
    public async Task<IActionResult> BoxStore([FromBody] StoreRequest req)
    {
        string apiName = nameof(BoxStore);
        _logger.LogInformation("{ApiName} 呼叫成功", apiName);

        var connStr = _config.GetConnectionString("c2c");
        var resultList = new List<BoxStoreDto>();
        string resultCode = "";
        string resultDesc = "";

        using var conn = new OracleConnection(connStr);
        await conn.OpenAsync();
        using var cmd = new OracleCommand("tdcapp_box.EC_BOX_STORE", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("P_DCNO", OracleDbType.Varchar2).Value = req.P_DCNO ?? "";
        cmd.Parameters.Add("P_DIS_NO", OracleDbType.Varchar2).Value = req.P_DIS_NO ?? "";
        cmd.Parameters.Add("P_DEL_STORE_DATE", OracleDbType.Varchar2).Value = req.P_DEL_STORE_DATE ?? "";
        cmd.Parameters.Add("P_USERID", OracleDbType.Varchar2).Value = req.P_USERID ?? "";

        var cursor = new OracleParameter("P_RESULT1", OracleDbType.RefCursor, ParameterDirection.Output);
        var code = new OracleParameter("RESULT_CODE", OracleDbType.Varchar2, 5) { Direction = ParameterDirection.Output };
        var desc = new OracleParameter("RESULT_DESC", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(cursor);
        cmd.Parameters.Add(code);
        cmd.Parameters.Add(desc);

        await cmd.ExecuteNonQueryAsync();

        resultCode = code.Value?.ToString() ?? string.Empty;
        resultDesc = desc.Value?.ToString() ?? string.Empty;

        if (cursor.Value != null && resultCode == "00")
        {
            using var reader = ((OracleRefCursor)cursor.Value).GetDataReader();
            while (reader.Read())
            {
                resultList.Add(new BoxStoreDto
                {
                    A_DATE = reader["A_DATE"]?.ToString(),
                    DC_NO = reader["DC_NO"]?.ToString(),
                    DIS_NO = reader["DIS_NO"]?.ToString(),
                    STORE_NO = reader["STORE_NO"]?.ToString(),
                    ORIG_STORE_NO = reader["ORIG_STORE_NO"]?.ToString()
                });
            }
        }

        _logger.LogInformation("{ApiName} 呼叫結束", apiName);

        return Ok(new { RESULT_CODE = resultCode, RESULT_DESC = resultDesc, P_RESULT1 = resultList });
    }

    // ========== 8. 溫層對照檔 ==========
    [HttpGet("areadeltype")]
    public async Task<IActionResult> AreaDelType()
    {
        string apiName = nameof(AreaDelType);
        _logger.LogInformation("{ApiName} 呼叫成功", apiName);

        var connStr = _config.GetConnectionString("c2c");
        var resultList = new List<AreaDelTypeDto>();
        string resultCode = "";
        string resultDesc = "";

        using var conn = new OracleConnection(connStr);
        await conn.OpenAsync();
        using var cmd = new OracleCommand("tdcapp_box.EC_BOX_AREADELTYPE", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        var cursor = new OracleParameter("P_RESULT1", OracleDbType.RefCursor, ParameterDirection.Output);
        var code = new OracleParameter("RESULT_CODE", OracleDbType.Varchar2, 5) { Direction = ParameterDirection.Output };
        var desc = new OracleParameter("RESULT_DESC", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(cursor);
        cmd.Parameters.Add(code);
        cmd.Parameters.Add(desc);

        await cmd.ExecuteNonQueryAsync();

        resultCode = code.Value?.ToString() ?? string.Empty;
        resultDesc = desc.Value?.ToString() ?? string.Empty;

        if (cursor.Value != null && resultCode == "00")
        {
            using var reader = ((OracleRefCursor)cursor.Value).GetDataReader();
            while (reader.Read())
            {
                resultList.Add(new AreaDelTypeDto
                {
                    Area_Id = reader["AREA_ID"]?.ToString(),
                    Area_DelType = reader["AREA_DELTYPE"]?.ToString(),
                    PDA_DelType = reader["PDA_DELTYPE"]?.ToString(),
                    SCAN_HISDAY = reader["SCAN_HISDAY"]?.ToString(),
                    ADDSCAN_HISDAY = reader["ADDSCAN_HISDAY"]?.ToString()
                });
            }
        }

        _logger.LogInformation("{ApiName} 呼叫結束", apiName);

        return Ok(new { RESULT_CODE = resultCode, RESULT_DESC = resultDesc, areaList = resultList });
    }

    // ========== 9. 取消疊貨條碼 ==========
    [HttpPost("delete")]
    public async Task<IActionResult> BoxDelete([FromBody] BoxDeleteRequest req)
    {
        string apiName = nameof(BoxDelete);
        _logger.LogInformation("{ApiName} 呼叫成功", apiName);

        var connStr = _config.GetConnectionString("c2c");
        var resultList = new List<BoxDeleteDto>();
        string resultCode = "";
        string resultDesc = "";

        using var conn = new OracleConnection(connStr);
        await conn.OpenAsync();
        using var cmd = new OracleCommand("tdcapp_box.EC_BOX_DELETE", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("P_DCNO", OracleDbType.Varchar2).Value = req.P_DCNO ?? "";
        cmd.Parameters.Add("P_DEL_STORE_DATE", OracleDbType.Varchar2).Value = req.P_DEL_STORE_DATE ?? "";
        cmd.Parameters.Add("P_CAR_ROUTE", OracleDbType.Varchar2).Value = req.P_CAR_ROUTE ?? "";
        cmd.Parameters.Add("P_STORE_NO", OracleDbType.Varchar2).Value = req.P_STORE_NO ?? "";
        cmd.Parameters.Add("P_USERID", OracleDbType.Varchar2).Value = req.P_USERID ?? "";

        var cursor = new OracleParameter("P_RESULT1", OracleDbType.RefCursor, ParameterDirection.Output);
        var code = new OracleParameter("RESULT_CODE", OracleDbType.Varchar2, 5) { Direction = ParameterDirection.Output };
        var desc = new OracleParameter("RESULT_DESC", OracleDbType.Varchar2, 500) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(cursor);
        cmd.Parameters.Add(code);
        cmd.Parameters.Add(desc);

        await cmd.ExecuteNonQueryAsync();

        resultCode = code.Value?.ToString() ?? string.Empty;
        resultDesc = desc.Value?.ToString() ?? string.Empty;

        if (cursor.Value != null && resultCode == "00")
        {
            using var reader = ((OracleRefCursor)cursor.Value).GetDataReader();
            while (reader.Read())
            {
                resultList.Add(new BoxDeleteDto
                {
                    RESULT_CODE = reader["RESULT_CODE"]?.ToString(),
                    RESULT_DESC = reader["RESULT_DESC"]?.ToString()
                });
            }
        }

        _logger.LogInformation("{ApiName} 呼叫結束", apiName);

        return Ok(new { RESULT_CODE = resultCode, RESULT_DESC = resultDesc, DEL = resultList });
    }


    // ========== Private Methods ==========
    // 疊貨寫入參數
    private static void AddStackParams(OracleCommand cmd, BoxStackRequest req, string batchId, bool skipBatch)
    {
        cmd.Parameters.Add(":A_DATE", OracleDbType.Varchar2).Value = req.A_DATE ?? "";
        cmd.Parameters.Add(":DC_NO", OracleDbType.Varchar2).Value = req.P_DCNO ?? "";
        cmd.Parameters.Add(":CAR_ROUTE", OracleDbType.Varchar2).Value = req.P_CAR_ROUTE ?? "";
        cmd.Parameters.Add(":DEL_TYPE", OracleDbType.Varchar2).Value = req.P_DEL_TYPE ?? "";
        cmd.Parameters.Add(":ROUTE_NO", OracleDbType.Varchar2).Value = req.ROUTE_NO ?? "";
        cmd.Parameters.Add(":CAR_NO", OracleDbType.Varchar2).Value = req.CAR_NO ?? "";
        cmd.Parameters.Add(":STORE_NO", OracleDbType.Varchar2).Value = req.STORE_NO ?? "";
        cmd.Parameters.Add(":PACK_BOX_NO", OracleDbType.Varchar2).Value = req.P_BOX_NO ?? "";
        cmd.Parameters.Add(":DATA_TYPE", OracleDbType.Varchar2).Value = req.DATA_TYPE ?? "";
        cmd.Parameters.Add(":SCAN_TIME", OracleDbType.Varchar2).Value = req.SCAN_TIME ?? "";
        cmd.Parameters.Add(":USER_ID", OracleDbType.Varchar2).Value = req.P_USERID ?? "";
        cmd.Parameters.Add(":USER_NAME", OracleDbType.Varchar2).Value = req.USER_NAME ?? "";
        cmd.Parameters.Add(":UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
        if (!skipBatch)
            cmd.Parameters.Add(":BATCH_ID", OracleDbType.Varchar2).Value = batchId;
    }

    // 卸貨寫入參數
    private static void AddUnloadParams(OracleCommand cmd, BoxUnloadRequest req, string batchId, bool skipBatch)
    {
        cmd.Parameters.Add(":A_DATE", OracleDbType.Varchar2).Value = req.A_DATE ?? "";
        cmd.Parameters.Add(":DC_NO", OracleDbType.Varchar2).Value = req.P_DCNO ?? "";
        cmd.Parameters.Add(":CAR_ROUTE", OracleDbType.Varchar2).Value = req.P_CAR_ROUTE ?? "";
        cmd.Parameters.Add(":DEL_TYPE", OracleDbType.Varchar2).Value = req.P_DEL_TYPE ?? "";
        cmd.Parameters.Add(":ROUTE_NO", OracleDbType.Varchar2).Value = req.ROUTE_NO ?? "";
        cmd.Parameters.Add(":CAR_NO", OracleDbType.Varchar2).Value = req.CAR_NO ?? "";
        cmd.Parameters.Add(":STORE_NO", OracleDbType.Varchar2).Value = req.STORE_NO ?? "";
        cmd.Parameters.Add(":PACK_BOX_NO", OracleDbType.Varchar2).Value = req.P_BOX_NO ?? "";
        cmd.Parameters.Add(":DATA_TYPE", OracleDbType.Varchar2).Value = req.DATA_TYPE ?? "";
        cmd.Parameters.Add(":SCAN_TIME", OracleDbType.Varchar2).Value = req.SCAN_TIME ?? "";
        cmd.Parameters.Add(":USER_ID", OracleDbType.Varchar2).Value = req.P_USERID ?? "";
        cmd.Parameters.Add(":USER_NAME", OracleDbType.Varchar2).Value = req.USER_NAME ?? "";
        cmd.Parameters.Add(":UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
        if (!skipBatch)
            cmd.Parameters.Add(":BATCH_ID", OracleDbType.Varchar2).Value = batchId;
    }

    // ========== End Private Methods ==========

}