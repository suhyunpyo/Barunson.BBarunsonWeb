using Barunson.BBarunsonWeb.Models;
using Barunson.DbContext;
using Barunson.DbContext.DbModels.BarShop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Text;

namespace Barunson.BBarunsonWeb.Controllers
{
    public class HardcodingListController : Controller
    {
        private BarShopContext BarShopContext { get; set; }

        public HardcodingListController(BarShopContext barShopContext)
        {
            this.BarShopContext = barShopContext;
        }

        public IActionResult Index(HardcodingInitialSearchParam Param)
        {
            HardcodingInitialViewModel model = new HardcodingInitialViewModel();

            if (Param.SdYn == null && Param.HgYn == null && Param.SgYn == null)
            {
                Param.SdYn = "Y";
                Param.HgYn = "Y";
                Param.SgYn = "Y";
            }

            model.Param = Param;
            model.HardcodingList = new List<HardcodingInitialModel>();

            using (DbConnection connection = BarShopContext.Database.GetDbConnection())
            {
                try
                {
                    connection.Open();

                    DbCommand command = connection.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "[PROC_SELECT_HARDCODING_INITIAL]";
                    
                    var ParamMap = new Dictionary<string, object>();
                    ParamMap.Add("@P_JUMUN", 1);
                    ParamMap.Add("@P_DISPLAY", 1);
                    ParamMap.Add("@P_COMPANY_SEQ", "5000,5001,5005");
                    ParamMap.Add("@P_SD_YN", Param.SdYn == null ? "" : Param.SdYn);
                    ParamMap.Add("@P_HG_YN", Param.HgYn == null ? "" : Param.HgYn);
                    ParamMap.Add("@P_SG_YN", Param.SgYn == null ? "" : Param.SgYn);

                    for(int pIdx = 0; pIdx < ParamMap.Count; pIdx++)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = ParamMap.ToArray()[pIdx].Key;
                        parameter.Value = ParamMap.ToArray()[pIdx].Value;
                        command.Parameters.Add(parameter);
                    }
                    
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        HardcodingInitialModel initial;

                        while (reader.Read())
                        {
                            initial = new HardcodingInitialModel();

                            initial.CardSeq = reader.GetInt32(0);
                            initial.CardCode = reader.GetString(1);
                            initial.PrintMethod = reader.GetString(2);
                            initial.PmValue = reader.GetString(3);
                            initial.Pm = reader.GetString(4);
                            initial.Pm1 = reader.GetString(5);
                            initial.Pm2 = reader.GetString(6);
                            initial.SdUse = reader.GetString(7);
                            initial.HgUse = reader.GetString(8);
                            initial.SgUse = reader.GetString(9);
                            initial.Barunson = reader.GetString(10);
                            initial.Mall = reader.GetString(11);
                            initial.Premier = reader.GetString(12);

                            model.HardcodingList.Add(initial);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception.Message: {0}", ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInitial(HardcodingInitialParam Param)
        {
            int result = 0;

            HardCodingList? hardCoding1 = new HardCodingList();
            HardCodingList? hardCoding2 = new HardCodingList();

            hardCoding1.HardID = "HYUNDAI_GOLDFOIL";
            hardCoding1.HardCode = Param.CardCode;
            hardCoding1.HardUse = "Y";
            hardCoding1.HardDescr = "frmCorelPhoto2: 광일(현대금박) 외주건";
            hardCoding1.MultiUse = "Y";


            hardCoding2.HardID = "SAMSUNG_GOLDFOIL";
            hardCoding2.HardCode = Param.CardCode;
            hardCoding2.HardUse = "Y";
            hardCoding2.HardDescr = "frmCorelPhoto2: 삼성금박 외주건";
            hardCoding2.MultiUse = "Y";

            if (Param.Factory == "sd")
            {
                hardCoding1 = await BarShopContext.HardCodingList.Where(x => x.HardID == "HYUNDAI_GOLDFOIL" && x.HardCode == Param.CardCode).FirstOrDefaultAsync();
                if (hardCoding1 != null)
                {
                    BarShopContext.HardCodingList.Remove(hardCoding1);
                    result++;
                }

                hardCoding2 = await BarShopContext.HardCodingList.Where(x => x.HardID == "SAMSUNG_GOLDFOIL" && x.HardCode == Param.CardCode).FirstOrDefaultAsync();
                if (hardCoding2 != null)
                {
                    BarShopContext.HardCodingList.Remove(hardCoding2);
                    result++;
                }

            }
            else if (Param.Factory == "hg")
            {
                if (BarShopContext.HardCodingList.Where(x => x.HardID == hardCoding1.HardID && x.HardCode == hardCoding1.HardCode).ToList().Count() == 0)
                {
                    BarShopContext.HardCodingList.Add(hardCoding1);
                    result++;
                }

                hardCoding2 = BarShopContext.HardCodingList.Where(x => x.HardID == "SAMSUNG_GOLDFOIL" && x.HardCode == Param.CardCode).FirstOrDefault();
                if (hardCoding2 != null)
                {
                    BarShopContext.HardCodingList.Remove(hardCoding2);
                    result++;
                }
            }
            else if (Param.Factory == "sg")
            {
                hardCoding1 = BarShopContext.HardCodingList.Where(x => x.HardID == "HYUNDAI_GOLDFOIL" && x.HardCode == Param.CardCode).FirstOrDefault();
                if (hardCoding1 != null)
                {
                    BarShopContext.HardCodingList.Remove(hardCoding1);
                    result++;
                }

                if (BarShopContext.HardCodingList.Where(x => x.HardID == hardCoding2.HardID && x.HardCode == hardCoding2.HardCode).ToList().Count() == 0)
                {
                    BarShopContext.HardCodingList.Add(hardCoding2);
                    result++;
                }
            }

            await BarShopContext.SaveChangesAsync();

            return Json(new { result = result });
        }

        private HardcodingAuthViewModel GetHardcodingAuthView(string type)
        {
            HardcodingAuthViewModel model = new HardcodingAuthViewModel();

            AuthType auth = model.AuthTypes.Where(x => x.Type == type).First();

            auth = auth == null ? model.AuthTypes.First() : auth;

            model.AuthType = auth;

            type = auth.Type;

            List<HardCodingList> HardcodingAdminList = BarShopContext.HardCodingList.Where(x => x.HardID == type).ToList();

            List<string> hardcodingAdminList = new List<string>();

            foreach (HardCodingList hardcoding in HardcodingAdminList)
            {
                hardcodingAdminList.Add(hardcoding.HardCode);
            }

            model.HardcodingAdminLsts = BarShopContext.ADMIN_LST.Where(x => hardcodingAdminList.Contains(x.ADMIN_ID)).OrderBy(x => x.ADMIN_NAME).ToList();

            List<ADMIN_LST> adminList = BarShopContext.ADMIN_LST.Where(x => x.COMPANY_TYPE_CODE == null).Where(x => x.NState == "1").OrderBy(x => x.ADMIN_NAME).ToList();

            List<string> removeList = new List<string>();

            foreach (ADMIN_LST admin in model.HardcodingAdminLsts)
            {
                removeList.Add(admin.ADMIN_ID);
            }

            adminList.RemoveAll(a => removeList.Contains(a.ADMIN_ID));

            model.AdminLsts = adminList;

            return model;
        }

        [HttpPost]
        public IActionResult SaveAuth(string type, List<string> adminId)
        {
            int result = 0;

            List<HardCodingList> HardcodingAdminList = BarShopContext.HardCodingList.Where(x => x.HardID == type).ToList();

            bool isSave = false;
            if (adminId != null && adminId.Count > 0)
            {
                List<HardCodingList> adminList = new List<HardCodingList>();
                string descr = string.Empty;

                // 삭제 대상 삭제
                foreach (HardCodingList hardCoding in HardcodingAdminList)
                {
                    descr = hardCoding.HardDescr == null ? "" : hardCoding.HardDescr;

                    if (adminId.Contains(hardCoding.HardCode))
                    {
                        adminId.Remove(hardCoding.HardCode);
                    }
                    else
                    {
                        BarShopContext.HardCodingList.Remove(hardCoding);
                        isSave = true;
                    }
                }

                foreach (string id in adminId)
                {
                    HardCodingList hardCoding = new HardCodingList();
                    hardCoding.HardCode = id;
                    hardCoding.HardID = type;
                    hardCoding.HardDescr = descr;
                    hardCoding.HardUse = "Y";
                    hardCoding.MultiUse = "Y";

                    BarShopContext.HardCodingList.Add(hardCoding);
                    isSave = true;
                }

                if (isSave)
                {
                    result = BarShopContext.SaveChanges();
                }
            }

            return Json(new { result = result });
        }
        public IActionResult AuthManage(string type = "AUTH_TRUBLE")
        {
            HardcodingAuthViewModel model = GetHardcodingAuthView(type.ToUpper());

            return View(model);
        }
        public IActionResult DigitalParts(string type)
        {
            HardcodingDigitalViewModel model = GetHardcodingDigitalList(type);

            return View(model);
        }

        private HardcodingDigitalViewModel GetHardcodingDigitalList(string HardId)
        {
            HardcodingDigitalViewModel model = new HardcodingDigitalViewModel();

            DigitalType digital = model.DigitalTypes.Where(x => x.Type == HardId).First();

            digital = digital == null ? model.DigitalTypes.First() : digital;

            model.DigitalType = digital;

            HardId = digital.Type;

            List<HardcodingDigitalModel> result = new List<HardcodingDigitalModel>();

            DbConnection connection = BarShopContext.Database.GetDbConnection();

            try
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "[PROC_SELECT_HARDCODING_DIGITAL]";

                var ParamMap = new Dictionary<string, object>();
                ParamMap.Add("@P_HARD_ID", HardId);
                ParamMap.Add("@P_CARD_CODE", "");

                for (int pIdx = 0; pIdx < ParamMap.Count; pIdx++)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = ParamMap.ToArray()[pIdx].Key;
                    parameter.Value = ParamMap.ToArray()[pIdx].Value;
                    command.Parameters.Add(parameter);
                }

                using (DbDataReader reader = command.ExecuteReader())
                {
                    HardcodingDigitalModel _digital;

                    while (reader.Read())
                    {
                        _digital = new HardcodingDigitalModel();

                        _digital.CardCode = reader.GetString(0);
                        _digital.CardSeq = reader.GetInt32(1);
                        _digital.CardType = reader.GetString(2);
                        _digital.IsDigit = reader.GetString(3);
                        _digital.CardImage = reader.GetString(4);

                        result.Add(_digital);
                    }

                    model.HardcodingDigitalList = result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }

            return model;
        }

        private HardcodingSearchCardModel GetHardcodingDigital(string HardId, string CardCode)
        {
            HardcodingSearchCardModel result = new HardcodingSearchCardModel();

            DbConnection connection = BarShopContext.Database.GetDbConnection();

            try
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "[PROC_SELECT_HARDCODING_DIGITAL]";

                var ParamMap = new Dictionary<string, object>();
                ParamMap.Add("@P_HARD_ID", HardId);
                ParamMap.Add("@P_CARD_CODE", CardCode);

                for (int pIdx = 0; pIdx < ParamMap.Count; pIdx++)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = ParamMap.ToArray()[pIdx].Key;
                    parameter.Value = ParamMap.ToArray()[pIdx].Value;
                    command.Parameters.Add(parameter);
                }

                using (DbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result.CardSeq = reader.GetInt32(0);
                        result.CardCode = reader.GetString(1);  
                        result.CardDiv = reader.GetString(2);
                        result.CardType = reader.GetString(3);
                        result.CardImage = reader.GetString(4);
                        result.HardCode = reader.GetString(5);
                        result.CardKind = reader.GetInt32(6);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.Message: {0}", ex.Message);
            }

            return result;
        }

        public JsonResult GetHardcodingSearchCard(string HardId, string CardCode)
        {
            HardcodingDigitalSearchViewModel model = new HardcodingDigitalSearchViewModel();
            model.SearchCardModel = GetHardcodingDigital(HardId, CardCode);
            model.result = model.SearchCardModel != null ? 1 : 0;


            JsonResult json = new JsonResult(model);

            return json;
        }

        [HttpPost]
        public JsonResult AddHardcodingDigitalCard(string HardId, string CardCode)
        {
            int result = 0;
            
            HardcodingSearchCardModel card = GetHardcodingDigital(HardId, CardCode);

            if (card.HardCode == null)
            {
                // 하드코딩 미등록 카드
                HardCodingList HardCodingList = new HardCodingList();
                HardCodingList.HardID = HardId;
                HardCodingList.HardCode = CardCode;
                HardCodingList.HardDescr = "";
                HardCodingList.HardUse = "Y";
                HardCodingList.MultiUse = "Y";

                BarShopContext.HardCodingList.Add(HardCodingList);
                result = BarShopContext.SaveChanges();
            }

            return Json(new { result = result });
        }

        [HttpPost]
        public JsonResult RemoveHardcodingDigitalCard(string HardId, string CardCode)
        {
            int result = 0;
            HardCodingList? HardCodingList = BarShopContext.HardCodingList.Where(x => x.HardID == HardId && x.HardCode == CardCode).FirstOrDefault();

            if (HardCodingList != null)
            {
                BarShopContext.HardCodingList.Remove(HardCodingList);
                result = BarShopContext.SaveChanges();
            }

            return Json(new { result = result });
        }
        public IActionResult ShareInpaper()
        {
            return View();
        }
    }
}
