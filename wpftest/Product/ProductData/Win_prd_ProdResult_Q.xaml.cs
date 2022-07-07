/**
 * 
 * @details     생산실적 등록
 * @author      정승학
 * @date        2022-05-11
 * @version     1.0
 * 
 * @section MODIFYINFO 수정정보
 * - 수정일        - 수정자       : 수정내역
 * - 2022-05-11    - 정승학       : 최초생성
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WizMes_SeongBinFood
{
    /// <summary>
    /// Win_prd_ProdResult_Q.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_prd_ProdResult_Q : UserControl
    {
        Lib lib = new Lib();
        PlusFinder pf = MainWindow.pf;

        int rowNum = 0;
        string getLabelID = string.Empty;

        public Win_prd_ProdResult_Q()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            lib.UiLoading(sender);
            CheckBoxDateSearch.IsChecked = true;

            SetComboBox();

            DatePickerFromDateSearch.SelectedDate = DateTime.Today;
            DatePickerToDateSearch.SelectedDate = DateTime.Today;
        }

        #region 추가, 수정 // 조회, 저장완료, 취소
        private void SaveUpdateMode()
        {
            lib.UiButtonEnableChange_SCControl(this);

            TextBoxNoReworkName.IsEnabled = true;
            ComboBoxDayOrNight.IsEnabled = true;
            TextBoxCycleTime.IsEnabled = true;
            TextBoxWorkQty.IsEnabled = true;
            ComboBoxProcess.IsEnabled = false;
            ComboBoxMachine.IsEnabled = true;
            TextBoxWorker.IsEnabled = true;
            ButtonPlusFinderWorker.IsEnabled = true;
            ComboBoxJobGubun.IsEnabled = true;
            DataGridArticleChild.IsHitTestVisible = true;
            DatePickerProdDate.IsEnabled = true;

        }

        private void SearchMode()
        {
            lib.UiButtonEnableChange_IUControl(this);

            TextBoxNoReworkName.IsEnabled = false;
            ComboBoxDayOrNight.IsEnabled = false;
            TextBoxCycleTime.IsEnabled = false;
            TextBoxWorkQty.IsEnabled = false;
            ComboBoxProcess.IsEnabled = false;
            ComboBoxMachine.IsEnabled = false;
            TextBoxWorker.IsEnabled = false;
            ButtonPlusFinderWorker.IsEnabled = false;
            ComboBoxJobGubun.IsEnabled = false;
            DataGridArticleChild.IsHitTestVisible = false;

            DataGridArticleChild.Items.Clear();
        }

        #endregion

        #region 콤보박스
        private void SetComboBox()
        {
            //작업구분
            List<string[]> strJobGubun = new List<string[]>();
            string[] strVal1 = { "1", "정상" };
            string[] strVal2 = { "2", "무작업" };
            string[] strVal3 = { "3", "재작업" };
            strJobGubun.Add(strVal1);
            strJobGubun.Add(strVal2);
            strJobGubun.Add(strVal3);
            ObservableCollection<CodeView> ovcGugun = ComboBoxUtil.Instance.Direct_SetComboBox(strJobGubun);
            this.ComboBoxJobGubun.ItemsSource = ovcGugun;
            this.ComboBoxJobGubun.DisplayMemberPath = "code_name";
            this.ComboBoxJobGubun.SelectedValuePath = "code_id";

            //공정
            ObservableCollection<CodeView> ovcProcess = GetWorkProcess_SetComboBox(0, "");
            this.ComboBoxProcess.ItemsSource = ovcProcess;
            this.ComboBoxProcess.DisplayMemberPath = "code_name";
            this.ComboBoxProcess.SelectedValuePath = "code_id";

            //호기
            ObservableCollection<CodeView> ovcMachine = ComboBoxUtil.Instance.GetMachine("");
            this.ComboBoxMachine.ItemsSource = ovcMachine;
            this.ComboBoxMachine.DisplayMemberPath = "code_name";
            this.ComboBoxMachine.SelectedValuePath = "code_id";

            //작업조
            ObservableCollection<CodeView> ovcDayOrNight = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "DayOrNight", "Y", "", "");
            this.ComboBoxDayOrNight.ItemsSource = ovcDayOrNight;
            this.ComboBoxDayOrNight.DisplayMemberPath = "code_name";
            this.ComboBoxDayOrNight.SelectedValuePath = "code_id";

        }
        #endregion

        #region 검색조건 컨트롤 이벤트
        private void LabelDateSearch_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CheckBoxDateSearch.IsChecked == true)
            {
                CheckBoxDateSearch.IsChecked = false;
            }
            else
            {
                CheckBoxDateSearch.IsChecked = true;
            }
        }

        private void CheckBoxDateSearch_Checked(object sender, RoutedEventArgs e)
        {
            DatePickerFromDateSearch.IsEnabled = true;
            DatePickerToDateSearch.IsEnabled = true;
            GridDateSetButton.IsEnabled = true;
        }

        private void CheckBoxDateSearch_Unchecked(object sender, RoutedEventArgs e)
        {
            DatePickerFromDateSearch.IsEnabled = false;
            DatePickerToDateSearch.IsEnabled = false;
            GridDateSetButton.IsEnabled = false;
        }

        private void ButtonLastMonth_Click(object sender, RoutedEventArgs e)
        {
            DateTime[] SearchDate = lib.BringLastMonthContinue(DatePickerFromDateSearch.SelectedDate.Value);

            DatePickerFromDateSearch.SelectedDate = SearchDate[0];
            DatePickerToDateSearch.SelectedDate = SearchDate[1];
        }

        private void ButtonYesterDay_Click(object sender, RoutedEventArgs e)
        {
            DateTime[] SearchDate = lib.BringLastDayDateTimeContinue(DatePickerToDateSearch.SelectedDate.Value);

            DatePickerFromDateSearch.SelectedDate = SearchDate[0];
            DatePickerToDateSearch.SelectedDate = SearchDate[1];
        }

        private void ButtonThisMonth_Click(object sender, RoutedEventArgs e)
        {
            DatePickerFromDateSearch.SelectedDate = lib.BringThisMonthDatetimeList()[0];
            DatePickerToDateSearch.SelectedDate = lib.BringThisMonthDatetimeList()[1];
        }

        private void ButtonToday_Click(object sender, RoutedEventArgs e)
        {
            DatePickerFromDateSearch.SelectedDate = DateTime.Today;
            DatePickerToDateSearch.SelectedDate = DateTime.Today;
        }
        #endregion

        #region 상단 버튼
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchMode();
            FillGrid();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            lib.ChildMenuClose(this.ToString());
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CreateLabel())
            {
                if (SaveData(getLabelID))
                {
                    SearchMode();

                    DataGridPlan.IsHitTestVisible = true;
                    ReSearch(rowNum);
                }
            }
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SearchMode();
            this.DataContext = null;
            TextBoxCycleTime.Clear();
            TextBoxWorkQty.Clear();
            DatePickerProdDate.SelectedDate = null;
            DatePickerWorkStartDate.SelectedDate = null;
            DatePickerWorkEndDate.SelectedDate = null;
            TextBoxWorker.Clear();
            TextBoxProdScanTime.Clear();
            TextBoxWorkStartTime.Clear();
            TextBoxWorkEndTime.Clear();
            ComboBoxDayOrNight.SelectedItem = null;
            ComboBoxProcess.SelectedItem = null;
            ComboBoxMachine.SelectedItem = null;
            ComboBoxJobGubun.SelectedItem = null;



        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SaveUpdateMode();

            ComboBoxDayOrNight.SelectedIndex = 0;
            DatePickerProdDate.SelectedDate = DateTime.Now;
            TextBoxProdScanTime.Text = DateTime.Now.ToString("HH:mm:ss");
            ComboBoxProcess.SelectedIndex = 1;
            //DatePickerWorkStartDate.SelectedDate = DatePickerProdDate.SelectedDate;
            TextBoxWorkStartTime.Text = DateTime.Now.ToString("HH:mm:ss");
            //DatePickerWorkEndDate.SelectedDate = DatePickerProdDate.SelectedDate;
            TextBoxWorkEndTime.Text = DateTime.Now.ToString("HH:mm:ss");
            ComboBoxJobGubun.SelectedIndex = 0;

            var selectedPlanData = DataGridPlan.SelectedItem as Win_prd_ProdResult_Q_CodeView;
            if (selectedPlanData != null && selectedPlanData.InstID != null)
            {
                FillGrid_ArticleChild(selectedPlanData.InstID);
            }

            if(DataGridArticleChild.Items.Count <= 0)
            {
                MessageBox.Show("사용 가능한 하위품이 없습니다.");
                btnCancel_Click(null, null);
            }
        }
        #endregion

        #region ReSearch
        private void ReSearch(int selectedIndex)
        {
            FillGrid();

            if(DataGridPlan.Items.Count > 0)
            {
                DataGridPlan.SelectedIndex = selectedIndex;
            }
            else
            {
                MessageBox.Show("조회된 데이터가 없습니다.");
                return;
            }
        }
        #endregion

        #region 작업지시 조회 - DataGridPlan
        private void FillGrid()
        {
            if(DataGridPlan.Items.Count > 0)
            {
                DataGridPlan.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("ChkDate", CheckBoxDateSearch.IsChecked == true ? 1 : 0);
                sqlParameter.Add("SDate", CheckBoxDateSearch.IsChecked == true && DatePickerFromDateSearch.SelectedDate != null ? DatePickerFromDateSearch.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("EDate", CheckBoxDateSearch.IsChecked == true && DatePickerToDateSearch.SelectedDate != null ? DatePickerToDateSearch.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("ChkCustomID", 0);
                sqlParameter.Add("CustomID", "");
                sqlParameter.Add("ChkArticleID", 0);
                sqlParameter.Add("ArticleID", "");
                sqlParameter.Add("ChkOrder", 0);
                sqlParameter.Add("Order", "");
                sqlParameter.Add("ChkIncPlanComplete", 1);
                sqlParameter.Add("ChkTheEnd", 0);
                sqlParameter.Add("ChkBuyerArticleNo", 0);
                sqlParameter.Add("BuyerArticleNoID", "");

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_prd_sProdResult_Plan", sqlParameter, false);

                if(ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;

                    if(dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;

                        foreach(DataRow dr in drc)
                        {
                            i++;

                            var WPPQC = new Win_prd_ProdResult_Q_CodeView()
                            {
                                Num = i,
                                //cls = dr["cls"].ToString(),
                                KCustom = dr["KCustom"].ToString(),
                                CustomID = dr["CustomID"].ToString(),
                                Article = dr["Article"].ToString(),
                                Spec = dr["Spec"].ToString(),
                                OrderID = dr["OrderID"].ToString(),
                                OrderNo = dr["OrderNo"].ToString(),
                                OrderQty = Convert.ToDouble(dr["OrderQty"]),

                                TotOrderInstQty = Convert.ToDouble(dr["TotOrderinstqty"]),
                                notOrderInstQty = Convert.ToDouble(dr["notOrderInstQty"]),
                                OrderInstQty = Convert.ToDouble(dr["OrderInstQy"]),
                                p1WorkQty = Convert.ToDouble(dr["p1WorkQty"]),
                                p1ProcessID = dr["p1ProcessID"].ToString(),

                                p1ProcessName = dr["p1ProcessName"].ToString(),
                                InspectQty = Convert.ToDouble(dr["InspectQty"]),
                                OutQty = Convert.ToDouble(dr["OutQty"]),
                                PatternID = dr["PatternID"].ToString(),
                                ArticleGrpID = dr["ArticleGrpID"].ToString(),

                                BuyerModel = dr["BuyerModel"].ToString(),
                                BuyerModelID = dr["BuyerModelID"].ToString(),
                                BuyerArticleNo = dr["BuyerArticleNo"].ToString(),
                                Remark = dr["Remark"].ToString(),
                                PlanComplete = dr["PlanComplete"].ToString(),

                                ArticleID = dr["ArticleID"].ToString(),
                                InstID = dr["InstID"].ToString(),
                                InstDate = DatePickerFormat(dr["InstDate"].ToString()),
                                ProcPattern = dr["ProcPattern"].ToString(),
                                MtrExceptYN = dr["MtrExceptYN"].ToString(),

                                OutwareExceptYN = dr["OutwareExceptYN"].ToString(),
                                LotID = dr["LotID"].ToString(),
                                PlanTheEnd = dr["PlanTheEnd"].ToString(),
                                OrderSeq = dr["OrderSeq"].ToString(),
                            };

                            if(WPPQC.cls != "9")
                            {
                                DataGridPlan.Items.Add(WPPQC);
                            }

                        }
                    }
                }
            }
            catch(Exception ee)
            {
                MessageBox.Show("예외처리 - " + ee.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

        }
        #endregion

        #region 하위품 조회 - DataGridArticle
        private void FillGrid_ArticleChild(string strInstID)
        {
            if(DataGridArticleChild.Items.Count > 0)
            {
                DataGridArticleChild.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("InstID", strInstID);
                sqlParameter.Add("nchkWeek", 1);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_prd_sWkResultForJasook", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;

                    if (dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {
                            i++;

                            var WPPQCA = new Win_prd_ProdResult_Q_CodeView_ArticleChild()
                            {
                                OutwareID = dr["Outwareid"].ToString(),
                                LabelID = dr["LabelID"].ToString(),
                                ArticleID = dr["ArticleID"].ToString(),
                                Article = dr["Article"].ToString(),
                                BuyerArticleNo = dr["BuyerArticleNo"].ToString(),
                                Outdate = dr["Outdate"].ToString(),
                                OutQty = dr["OutQty"].ToString(),
                            };

                            DataGridArticleChild.Items.Add(WPPQCA);
                        }
                    }
                    else
                    {
                        MessageBox.Show("출고된 하위품이 없습니다.");
                        return;
                    }
                }

            }
            catch(Exception ee)
            {
                MessageBox.Show("예외처리 - " + ee.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

        }

        #endregion

        #region 라벨 생성
        private bool CreateLabel()
        {
            getLabelID = string.Empty;
            bool result = false;

            try
            {
                List<Procedure> prolist = new List<Procedure>();
                List<Dictionary<string, object>> listParameter = new List<Dictionary<string, object>>();

                if (CheckData_CreateLabel())
                {
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();
                    sqlParameter.Add("LabelID", "");
                    sqlParameter.Add("LabelGubun", "1");
                    sqlParameter.Add("ProcessID", ComboBoxProcess.SelectedValue == null ? "" : ComboBoxProcess.SelectedValue.ToString());
                    sqlParameter.Add("ArticleID", TextBoxBuyerArticleNo.Text == string.Empty ? "" : (TextBoxBuyerArticleNo.Tag == null ? "" : TextBoxBuyerArticleNo.Tag.ToString()));
                    sqlParameter.Add("PrintDate", DatePickerProdDate.SelectedDate == null ? "" : DatePickerProdDate.SelectedDate.Value.ToString("yyyyMMdd"));
                    sqlParameter.Add("ReprintDate", "");
                    sqlParameter.Add("ReprintQty", 0);
                    sqlParameter.Add("InstID", TextBoxInstID.Text == string.Empty ? "" : TextBoxInstID.Text);
                    sqlParameter.Add("InstDetSeq", 1);
                    sqlParameter.Add("CardID", "");
                    sqlParameter.Add("OrderID", TextBoxOrderID.Text == string.Empty ? "" : TextBoxOrderID.Text);
                    sqlParameter.Add("PrintQty", 1); // 0 넣으면 라벨 생성안됨,  1 고정
                    sqlParameter.Add("LabelPrintQty", 0);
                    sqlParameter.Add("nQtyPerBox", 0);
                    sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                    Procedure pro1 = new Procedure();
                    pro1.Name = "xp_prd_iwkLabelPrint_C";
                    pro1.OutputUseYN = "Y";
                    pro1.OutputName = "LabelID";
                    pro1.OutputLength = "12";

                    prolist.Add(pro1);
                    listParameter.Add(sqlParameter);

                    List<KeyValue> list_Result = new List<KeyValue>();
                    list_Result = DataStore.Instance.ExecuteAllProcedureOutputGetCS(prolist, listParameter);
                    string sGetID = string.Empty;

                    if (list_Result[0].key.ToLower() == "success")
                    {
                        list_Result.RemoveAt(0);
                        for (int i = 0; i < list_Result.Count; i++)
                        {
                            KeyValue kv = list_Result[i];
                            if (kv.key == "LabelID")
                            {
                                sGetID = kv.value;
                                getLabelID = kv.value;

                                prolist.RemoveAt(0);
                                listParameter.Clear();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("[라벨 생성 실패]\r\n" + list_Result[0].value.ToString());
                    }

                    string[] confirm = new string[2];
                    confirm = DataStore.Instance.ExecuteAllProcedureOutputNew(prolist, listParameter);

                    if (confirm[0] != "success")
                    {
                        MessageBox.Show("[라벨 생성 실패]\r\n" + confirm[1].ToString());
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }

                }
            }
            catch(Exception ee)
            {
                MessageBox.Show("예외처리 - " + ee.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return result;
        }
        #endregion

        #region 저장
        private bool SaveData(string strLabelID)
        {
            bool result = false;

            List<Procedure> prolist = new List<Procedure>();
            List<Dictionary<string, object>> listParameter = new List<Dictionary<string, object>>();

            try
            {
                if (CheckData())
                {
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();
                    sqlParameter.Add("JobID", 0);
                    sqlParameter.Add("InstID", TextBoxInstID.Text == string.Empty ? "" : TextBoxInstID.Text);
                    sqlParameter.Add("InstDetSeq", 1);
                    sqlParameter.Add("LabelID", strLabelID);
                    sqlParameter.Add("StartSaveLabelID", "");

                    sqlParameter.Add("LabelGubun", "1");
                    sqlParameter.Add("ProcessID", ComboBoxProcess.SelectedValue == null ? "" : ComboBoxProcess.SelectedValue.ToString());
                    sqlParameter.Add("MachineID", ComboBoxMachine.SelectedValue == null ? "" : ComboBoxMachine.SelectedValue.ToString());
                    sqlParameter.Add("ScanDate", DatePickerProdDate.SelectedDate == null ? "" : DatePickerProdDate.SelectedDate.Value.ToString("yyyyMMdd"));
                    sqlParameter.Add("ScanTime", TextBoxProdScanTime.Text == string.Empty ? "" : TextBoxProdScanTime.Text.Replace(":", ""));

                    sqlParameter.Add("ArticleID", TextBoxBuyerArticleNo.Text == string.Empty ? "" : (TextBoxBuyerArticleNo.Tag == null ? "" : TextBoxBuyerArticleNo.Tag.ToString()));
                    sqlParameter.Add("WorkQty", TextBoxWorkQty.Text == string.Empty ? "0" : TextBoxWorkQty.Text.Replace(",", ""));
                    sqlParameter.Add("Comments", "사무실 직접 실적 저장");
                    sqlParameter.Add("ReworkOldYN", "");
                    sqlParameter.Add("ReworkLinkProdID", "");

                    sqlParameter.Add("WorkStartDate", DatePickerWorkStartDate.SelectedDate == null ? "" : DatePickerWorkStartDate.SelectedDate.Value.ToString("yyyyMMdd"));
                    sqlParameter.Add("WorkStartTime", TextBoxWorkStartTime.Text == string.Empty ? "" : TextBoxWorkStartTime.Text.Replace(":", ""));
                    sqlParameter.Add("WorkEndDate", DatePickerWorkEndDate.SelectedDate == null ? "" : DatePickerWorkEndDate.SelectedDate.Value.ToString("yyyyMMdd"));
                    sqlParameter.Add("WorkEndTime", TextBoxWorkEndTime.Text == string.Empty ? "" : TextBoxWorkEndTime.Text.Replace(":", ""));
                    sqlParameter.Add("JobGbn", ComboBoxJobGubun.SelectedValue == null ? "" : ComboBoxJobGubun.SelectedValue.ToString());

                    sqlParameter.Add("NoReworkCode", "");
                    sqlParameter.Add("WDNO", "");
                    sqlParameter.Add("WDID", "");
                    sqlParameter.Add("WDQty", 0);
                    sqlParameter.Add("LogID", 0);

                    sqlParameter.Add("s4MID", "");
                    sqlParameter.Add("DayOrNightID", ComboBoxDayOrNight.SelectedValue == null ? "" : ComboBoxDayOrNight.SelectedValue.ToString());
                    sqlParameter.Add("SplitYNGBN", "NC");
                    sqlParameter.Add("CycleTime", TextBoxCycleTime.Text == string.Empty ? "0" : TextBoxCycleTime.Text.Replace(",", ""));
                    sqlParameter.Add("NoReworkInstYN", "");

                    sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);
                    sqlParameter.Add("PackSeq", 0);

                    Procedure pro1 = new Procedure();
                    pro1.Name = "xp_prd_wkResult_iWkResult";
                    pro1.OutputUseYN = "Y";
                    pro1.OutputName = "JobID";
                    pro1.OutputLength = "19";

                    prolist.Add(pro1);
                    listParameter.Add(sqlParameter);

                    List<KeyValue> list_Result = new List<KeyValue>();
                    list_Result = DataStore.Instance.ExecuteAllProcedureOutputGetCS(prolist, listParameter);
                    decimal sGetID = 0;

                    if (list_Result[0].key.ToLower() == "success")
                    {
                        list_Result.RemoveAt(0);
                        for (int i = 0; i < list_Result.Count; i++)
                        {
                            KeyValue kv = list_Result[i];
                            if (kv.key == "JobID")
                            {
                                sGetID = Convert.ToDecimal(kv.value);

                                //GetKey = kv.value;

                                prolist.RemoveAt(0);
                                listParameter.Clear();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
                    }

                    for(int i = 0; i < DataGridArticleChild.Items.Count; i++)
                    {
                        var articleChild = DataGridArticleChild.Items[i] as Win_prd_ProdResult_Q_CodeView_ArticleChild;

                        if(articleChild.Flag == true)
                        {
                            sqlParameter = new Dictionary<string, object>();
                            sqlParameter.Clear();

                            sqlParameter.Add("JobID", sGetID);
                            sqlParameter.Add("ChildLabelID", articleChild.LabelID);
                            sqlParameter.Add("ChildLabelGubun", "1");
                            sqlParameter.Add("ChildArticleID", articleChild.ArticleID);
                            sqlParameter.Add("ChildUseQty", 0);

                            sqlParameter.Add("ReworkOldYN", "");
                            sqlParameter.Add("ReworkLinkChildProdID", "");
                            sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                            Procedure pro2 = new Procedure();
                            pro2.Name = "xp_prd_wkResult_iWkResultArticleChild";
                            pro2.OutputUseYN = "N";
                            pro2.OutputName = "JobID";
                            pro2.OutputLength = "9";

                            prolist.Add(pro2);
                            listParameter.Add(sqlParameter);

                            //----
                           
                        }
                    }
                    //하위품만큼 StuffIN, StuffINSub테이블에 반제품 재고가 두번씩 들어가서 for문에서 빼냄 
                    sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();
                    sqlParameter.Add("JobID", sGetID);
                    sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);
                    sqlParameter.Add("sRtnMsg", "");

                    Procedure pro3 = new Procedure();
                    pro3.Name = "xp_prd_wkResult_iWkResultStuffInOut";
                    pro3.OutputUseYN = "N";
                    pro3.OutputName = "JobID";
                    pro3.OutputLength = "9";
                    prolist.Add(pro3);
                    listParameter.Add(sqlParameter);

                    string[] confirm = new string[2];
                    confirm = DataStore.Instance.ExecuteAllProcedureOutputNew(prolist, listParameter);

                    if(confirm[0] != "success")
                    {
                        MessageBox.Show("[저장실패]\r\n" + confirm[1].ToString());
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                
            }
            catch(Exception ee)
            {
                MessageBox.Show("예외처리 - " + ee.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return result;
        }

        #endregion

        #region 유효성 검사
        private bool CheckData()
        {
            bool flag = true;

            if(ComboBoxDayOrNight.SelectedValue == null)
            {
                MessageBox.Show("작업조를 선택하세요.");
                flag = false;
            }

            return flag;
        }

        private bool CheckData_CreateLabel()
        {
            bool flag = true;

            if(TextBoxOrderID.Text == string.Empty)
            {
                MessageBox.Show("관리번호 정보가 없습니다.");
                flag = false;
            }

            if(ComboBoxProcess.SelectedValue == null)
            {
                MessageBox.Show("공정을 선택하세요.");
                flag = false;
            }

            if (TextBoxBuyerArticleNo.Text == string.Empty && TextBoxBuyerArticleNo.Tag == null)
            {
                MessageBox.Show("품번 정보가 없습니다.");
                flag = false;
            }

            if(TextBoxInstID.Text == string.Empty)
            {
                MessageBox.Show("작업지시번호 정보가 없습니다.");
                flag = false;
            }

            return flag;
        }

        #endregion


        #region etc
        private string DatePickerFormat(string str)
        {
            string result = "";

            if (str.Length == 8)
            {
                if (!str.Trim().Equals(""))
                {
                    result = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
                }
            }
            return result;
        }

        private void TextBox_CheckIsNumeric(object sender, TextCompositionEventArgs e)
        {
            lib.CheckIsNumeric((TextBox)sender, e);
        }
        #endregion

        #region 공정 세팅
        /// <summary>
        /// 공정ID 가져오기
        /// </summary>
        /// <param name="num"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ObservableCollection<CodeView> GetWorkProcess_SetComboBox(int num, string value)
        {
            ObservableCollection<CodeView> retunCollection = new ObservableCollection<CodeView>();
            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Add("@nchkProc", num);
            sqlParameter.Add("@ProcessID", value);

            try
            {
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Work_sProcess", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count == 0)
                    {

                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;

                        CodeView mCodeView = new CodeView()
                        {
                            //code_id = "",
                            //code_name = "전체"
                        };
                        retunCollection.Add(mCodeView);

                        foreach (DataRow item in drc)
                        {

                            mCodeView = new CodeView()
                            {
                                code_id = item[0].ToString().Trim(),
                                code_name = item[1].ToString().Trim()
                            };
                            retunCollection.Add(mCodeView);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("콤보박스 생성 중 오류 발생 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return retunCollection;
        }
        #endregion

        #region mt_Machine - 호기 세팅
        /// <summary>
        /// 호기ID 가져오기
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ObservableCollection<CodeView> GetMachineByProcessID(string value)
        {
            if (value.Equals(""))
            {
                ComboBoxMachine.IsEnabled = false;
            }
            else
            {
                ComboBoxMachine.IsEnabled = true;
            }

            ObservableCollection<CodeView> ovcMachine = new ObservableCollection<CodeView>();

            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Add("sProcessID", value);

            DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Process_sMachineForComboBoxAndUsing", sqlParameter, false);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    CodeView CV = new CodeView();
                    //CV.code_id = "";
                    //CV.code_name = "전체";
                    //ovcMachine.Add(CV);

                    DataRowCollection drc = dt.Rows;

                    foreach (DataRow dr in drc)
                    {
                        CodeView mCodeView = new CodeView()
                        {
                            code_id = dr["Code"].ToString().Trim(),
                            code_name = dr["Name"].ToString().Trim()
                        };

                        ovcMachine.Add(mCodeView);
                    }
                }
            }

            return ovcMachine;
        }

        #endregion // mt_Machine - 호기 세팅




        private void ButtonPlusFinderWorker_Click(object sender, RoutedEventArgs e)
        {
            pf.ReturnCode(TextBoxWorker, (int)Defind_CodeFind.DCF_PERSON, "");
        }

        private void TextBoxWorker_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                pf.ReturnCode(TextBoxWorker, (int)Defind_CodeFind.DCF_PERSON, "");
            }
        }

        private void DatePickerProdDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePickerWorkStartDate.SelectedDate = DatePickerProdDate.SelectedDate;
            TextBoxWorkStartTime.Text = TextBoxProdScanTime.Text;
            DatePickerWorkEndDate.SelectedDate = DatePickerProdDate.SelectedDate;
            TextBoxWorkEndTime.Text = TextBoxProdScanTime.Text;
        }



        private void DataGridPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPlanData = DataGridPlan.SelectedItem as Win_prd_ProdResult_Q_CodeView;

            if(selectedPlanData != null)
            {
                this.DataContext = selectedPlanData;
            }
        }

        private void DataGridCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var currentItemArticleChild = DataGridArticleChild.CurrentItem as Win_prd_ProdResult_Q_CodeView_ArticleChild;

            if(currentItemArticleChild != null)
            {
                if (currentItemArticleChild.Flag)
                {
                    currentItemArticleChild.Flag = false;
                }
                else
                {
                    currentItemArticleChild.Flag = true;
                }
            }
        }

        private void ComboBoxProcess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBoxProcess.SelectedValue != null)
            {
                ObservableCollection<CodeView> ovcMachine = GetMachineByProcessID(ComboBoxProcess.SelectedValue.ToString());
                this.ComboBoxMachine.ItemsSource = ovcMachine;
                this.ComboBoxMachine.DisplayMemberPath = "code_name";
                this.ComboBoxMachine.SelectedValuePath = "code_id";

                ComboBoxMachine.SelectedIndex = 0;
            }
        }

        
    }

    #region CodeView
    class Win_prd_ProdResult_Q_CodeView : BaseView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }
        public string cls { get; set; }
        public string KCustom { get; set; }
        public string CustomID { get; set; }
        public string Article { get; set; }
        public string Spec { get; set; }
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public double OrderQty { get; set; }
        public double TotOrderInstQty { get; set; }
        public double notOrderInstQty { get; set; }
        public double OrderInstQty { get; set; }
        public double p1WorkQty { get; set; }
        public string p1ProcessID { get; set; }
        public string p1ProcessName { get; set; }
        public double InspectQty { get; set; }
        public double OutQty { get; set; }
        public string PatternID { get; set; }
        public string ArticleGrpID { get; set; }
        public string BuyerModel { get; set; }
        public string BuyerModelID { get; set; }
        public string BuyerArticleNo { get; set; }
        public string Remark { get; set; }
        public string PlanComplete { get; set; }
        public string ArticleID { get; set; }
        public string InstID { get; set; }
        public string InstDate { get; set; }
        public string ProcPattern { get; set; }
        public string MtrExceptYN { get; set; }
        public string OutwareExceptYN { get; set; }
        public string LotID { get; set; }
        public string PlanTheEnd { get; set; }
        public string OrderSeq { get; set; }
    }



    class Win_prd_ProdResult_Q_CodeView_ArticleChild : BaseView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }
        public bool Flag { get; set; }
        public string OutwareID { get; set; }
        public string LabelID { get; set; }
        public string ArticleID { get; set; }
        public string Article { get; set; }
        public string BuyerArticleNo { get; set; }
        public string Outdate { get; set; }
        public string OutQty { get; set; }

    }
    #endregion

}
